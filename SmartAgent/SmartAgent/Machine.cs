using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.System.Threading;
using DataAccess;




namespace SmartAgent
{
    // this class processes the machine signal
    public class Machine
    {

        public int DailyCycleCounter { get; set; } // TODO Reset at 24:00:00
        public int CyclesInThisPeriod { get; set; }
        public long LastCycleTime { get; set; }
        public TimeSpan StateDuration { get; set; }
        private DateTime _machineStateStarted;
        public MachineConfiguration MachineConfiguration { get; set; }
        public List<MachineStateHistory> MachineStateHistory { get; set; }
        public MachineStateHistory.State CurrentMachineState { get; set; }
        public MachineStateHistory.State LastMachineStateInMachineStateHistory { get; set; }
        private ThreadPoolTimer _timerCycleTimeOut;
        private ThreadPoolTimer _timerMachineDataPublish;
        private Stopwatch _cycleTimeStopWatch = new Stopwatch();
        private GatewayHubHandler _gatewayHubCommunication;
        private IDevice _device;

        // Constructor
        public Machine(IDevice device, GatewayHubHandler gatewayHubCommunication, MachineConfiguration machineConfiguration)
        {
            _device = device;
            _machineStateStarted = DateTime.Now;

            MachineConfiguration = machineConfiguration;
            _gatewayHubCommunication = gatewayHubCommunication;

            device.Inputs[machineConfiguration.CycleInputPin].InputChanged += InputInterpretation;
            CurrentMachineState = DataAccess.MachineStateHistory.State.Stopped;
            LastMachineStateInMachineStateHistory = DataAccess.MachineStateHistory.State.Stopped;

            MachineStateHistory = new List<MachineStateHistory>();

            _timerCycleTimeOut = ThreadPoolTimer.CreatePeriodicTimer(CycleTimeOut,
                                               TimeSpan.FromMilliseconds(machineConfiguration.MachineStateTimeout));

            _timerMachineDataPublish = ThreadPoolTimer.CreatePeriodicTimer(PublishActualMachineData,
                                        TimeSpan.FromMilliseconds(machineConfiguration.PublishingIntervall));
        }

        // publishes the actual machine data to the hub
        private void PublishActualMachineData(ThreadPoolTimer timer)
        {
            StateDuration = DateTime.Now - _machineStateStarted;
            _gatewayHubCommunication.PublishActualMachineData(this);
        }


        private void CycleTimeOut(ThreadPoolTimer timer)
        {
            if (LastMachineStateInMachineStateHistory != DataAccess.MachineStateHistory.State.Stopped)
            {
                _machineStateStarted = DateTime.Now;

                CurrentMachineState = DataAccess.MachineStateHistory.State.Stopped;
                FeedMachineStateHistory();
            }
        }

        private void InputInterpretation(IInput input)
        {
            if (input.State == false) // after each falling flank..
            {
                DailyCycleCounter++;
                CyclesInThisPeriod++;
                _cycleTimeStopWatch.Stop();
                LastCycleTime = _cycleTimeStopWatch.ElapsedMilliseconds;
            }

            else  // after each rising flank..
            {
                _cycleTimeStopWatch.Restart();
            }

            ResetTimerCycleTimeOut();

            if (LastMachineStateInMachineStateHistory != DataAccess.MachineStateHistory.State.Running)
            {

                _machineStateStarted = DateTime.Now;

                CurrentMachineState = DataAccess.MachineStateHistory.State.Running;
                FeedMachineStateHistory();
            }
        }


        private void CycleTimeOut(object sender, object e)
        {
            if (LastMachineStateInMachineStateHistory != DataAccess.MachineStateHistory.State.Stopped)
            {
                CurrentMachineState = DataAccess.MachineStateHistory.State.Stopped;
                FeedMachineStateHistory();
                _machineStateStarted = DateTime.Now;
            }
        }


        private void ResetTimerCycleTimeOut()
        {
            _timerCycleTimeOut.Cancel();
            _timerCycleTimeOut = ThreadPoolTimer.CreatePeriodicTimer(CycleTimeOut,
                                        TimeSpan.FromMilliseconds(MachineConfiguration.MachineStateTimeout));
        }


        public void StopMachineDataGeneration()
        {
            _timerMachineDataPublish.Cancel();
            _timerCycleTimeOut.Cancel();
            _device.Inputs[MachineConfiguration.CycleInputPin].InputChanged -= InputInterpretation;
        }

        // feeds the last machinestate history entity to the database
        private void FeedMachineStateHistory()
        {
            if (MachineStateHistory.Count > 0 && MachineStateHistory.Last().EndDateTime == DateTime.MinValue)  // conclude last machine state..
            {
                MachineStateHistory lastEntity = MachineStateHistory.Last();
                if (LastMachineStateInMachineStateHistory == lastEntity.MachineState)
                {
                    lastEntity.EndDateTime = DateTime.Now;
                    lastEntity.Duration = lastEntity.EndDateTime - lastEntity.StartDateTime;

                    lastEntity.DailyCycleCounter = DailyCycleCounter;
                    lastEntity.CyclesInThisPeriod = CyclesInThisPeriod;

                    DataAccess.DataAccess.AddDataMachineStateHistory(lastEntity);
                }
            }


            LastMachineStateInMachineStateHistory = CurrentMachineState;
            CyclesInThisPeriod = 0;

            int index;
            if (MachineStateHistory.Count() > 20)
            {
                index = MachineStateHistory.Count() - 20;
            }
            else
            {
                index = 0;
            }

            MachineStateHistory.RemoveRange(0, index);
            MachineStateHistory.Add(new MachineStateHistory(CurrentMachineState, MachineConfiguration.MachineId));
            _gatewayHubCommunication.NewHistoryDataNotification();
        }
    }
}
