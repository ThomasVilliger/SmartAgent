using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.System.Threading;
using DataStorageLibrary;




namespace PiFace_II
{
  public  class Machine 
    {
        public int DailyCycleCounter { get; set; } // TODO Reset at 24:00:00
        public int CyclesInThisPeriod { get; set; }
        public long LastCycleTime { get; set; }

        public TimeSpan StateDuration { get; set; }

        private DateTime MachineStateStarted;

        public MachineConfiguration MachineConfiguration { get; set; }

        public List<MachineStateHistory> MachineStateHistory { get; set; }


        public MachineStateHistory.State CurrentMachineState { get; set; }
        public MachineStateHistory.State LastMachineStateInMachineStateHistory { get; set; }

        // private DispatcherTimer timerCycleTimeOut;

        private ThreadPoolTimer timerCycleTimeOut;

        private ThreadPoolTimer timerMachineDataPublish;

        Stopwatch cycleTimeStopWatch = new Stopwatch();

        private GatewayHubHandler _gatewayHubCommunication;

        private IDevice _device;
        

        public Machine ( IDevice device  , GatewayHubHandler gatewayHubCommunication, MachineConfiguration machineConfiguration)
        {
            _device = device;
            MachineStateStarted = DateTime.Now;

            MachineConfiguration = machineConfiguration;
            _gatewayHubCommunication = gatewayHubCommunication;

            device.Inputs[machineConfiguration.CycleInputPin].InputChanged += InputInterpretation;
            CurrentMachineState = DataStorageLibrary.MachineStateHistory.State.Stopped;
            LastMachineStateInMachineStateHistory = DataStorageLibrary.MachineStateHistory.State.Stopped;
            

            MachineStateHistory = new List<MachineStateHistory>();

     timerCycleTimeOut =     ThreadPoolTimer.CreatePeriodicTimer(CycleTimeOut,
                                        TimeSpan.FromMilliseconds(machineConfiguration.MachineStateTimeout));

            timerMachineDataPublish = ThreadPoolTimer.CreatePeriodicTimer(PublishActualMachineData,
                                        TimeSpan.FromMilliseconds(machineConfiguration.PublishingIntervall));




        }

        private void PublishActualMachineData(ThreadPoolTimer timer)
        {
            StateDuration = DateTime.Now - MachineStateStarted;
            _gatewayHubCommunication.PublishActualMachineData(this);
        }

        private void CycleTimeOut(ThreadPoolTimer timer)
        {
            if (LastMachineStateInMachineStateHistory != DataStorageLibrary.MachineStateHistory.State.Stopped)
            {
                MachineStateStarted = DateTime.Now;

                CurrentMachineState = DataStorageLibrary.MachineStateHistory.State.Stopped;
                feedMachineStateHistory();
            }
        }

        private void InputInterpretation(IInput input) 
        {
            if (input.State == false) // after each falling flank..
            {
                DailyCycleCounter++;
                CyclesInThisPeriod++;
                cycleTimeStopWatch.Stop();
                LastCycleTime = cycleTimeStopWatch.ElapsedMilliseconds;

            }

            else  // after each rising flank..
            {
                cycleTimeStopWatch.Restart();
            }

            
            ResetTimerCycleTimeOut();

            if(LastMachineStateInMachineStateHistory != DataStorageLibrary.MachineStateHistory.State.Running)
            {

                MachineStateStarted = DateTime.Now;

                CurrentMachineState = DataStorageLibrary.MachineStateHistory.State.Running;
                feedMachineStateHistory();

                
            }
        }


        public void CycleTimeOut (object sender, object e)
        {    
            if (LastMachineStateInMachineStateHistory != DataStorageLibrary.MachineStateHistory.State.Stopped)
            {
                CurrentMachineState = DataStorageLibrary.MachineStateHistory.State.Stopped;
                feedMachineStateHistory();

                MachineStateStarted = DateTime.Now;
            }
        }


        public  void ResetTimerCycleTimeOut()
        {
            timerCycleTimeOut.Cancel();
            timerCycleTimeOut = ThreadPoolTimer.CreatePeriodicTimer(CycleTimeOut,
                                        TimeSpan.FromMilliseconds(MachineConfiguration.MachineStateTimeout));
        }


        public void StopMachineDataGeneration()
        {
            timerMachineDataPublish.Cancel();
            timerCycleTimeOut.Cancel();
            _device.Inputs[MachineConfiguration.CycleInputPin].InputChanged -= InputInterpretation;
        }


        private void feedMachineStateHistory()
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

                    DataAccess.AddDataMachineStateHistory(lastEntity);



                }
            }



            








            LastMachineStateInMachineStateHistory = CurrentMachineState;
            CyclesInThisPeriod = 0;

            int index;
            if(MachineStateHistory.Count()>20)
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
