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
  public  class CycleMachine 
    {
        public int DailyCycleCounter { get; set; } // TODO Reset at 24:00:00
        public int CyclesInThisPeriod { get; set; }
        public long LastCycleTime { get; set; }

        public TimeSpan StateDuration { get; set; }

        private DateTime MachineStateStarted;

        public CycleMachineConfiguration CycleMachineConfiguration { get; set; }

        public List<MachineStateHistoryEntity> MachineStateHistory { get; set; }

        public enum MachineState  { Running, Stopped}
        public MachineState CurrentMachineState { get; set; }
        public MachineState LastMachineStateInMachineStateHistory { get; set; }

        // private DispatcherTimer timerCycleTimeOut;

        private ThreadPoolTimer timerCycleTimeOut;

        private ThreadPoolTimer timerCycleMachineDataPublish;

        Stopwatch cycleTimeStopWatch = new Stopwatch();

        private GatewayHubHandler _gatewayHubCommunication;

        private IDevice _device;
        

        public CycleMachine ( IDevice device  , GatewayHubHandler gatewayHubCommunication, CycleMachineConfiguration cycleMachineConfiguration)
        {
            _device = device;
            MachineStateStarted = DateTime.Now;

            CycleMachineConfiguration = cycleMachineConfiguration;
            _gatewayHubCommunication = gatewayHubCommunication;

            device.Inputs[cycleMachineConfiguration.CycleInputPin].InputChanged += InputInterpretation;
            CurrentMachineState = MachineState.Stopped;
            LastMachineStateInMachineStateHistory = MachineState.Stopped;
            

            MachineStateHistory = new List<MachineStateHistoryEntity>();

     timerCycleTimeOut =     ThreadPoolTimer.CreatePeriodicTimer(CycleTimeOut,
                                        TimeSpan.FromMilliseconds(cycleMachineConfiguration.MachineStateTimeOut));

            timerCycleMachineDataPublish = ThreadPoolTimer.CreatePeriodicTimer(PublishActualMachineData,
                                        TimeSpan.FromMilliseconds(cycleMachineConfiguration.PublishingIntervall));




        }

        private void PublishActualMachineData(ThreadPoolTimer timer)
        {
            StateDuration = DateTime.Now - MachineStateStarted;
            _gatewayHubCommunication.PublishActualCycleMachineData(this);
        }

        private void CycleTimeOut(ThreadPoolTimer timer)
        {
            if (LastMachineStateInMachineStateHistory != MachineState.Stopped)
            {
                MachineStateStarted = DateTime.Now;

                CurrentMachineState = MachineState.Stopped;
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

            if(LastMachineStateInMachineStateHistory != MachineState.Running)
            {

                MachineStateStarted = DateTime.Now;

                CurrentMachineState = MachineState.Running;
                feedMachineStateHistory();

                
            }
        }


        public void CycleTimeOut (object sender, object e)
        {    
            if (LastMachineStateInMachineStateHistory != MachineState.Stopped)
            {
                CurrentMachineState = MachineState.Stopped;
                feedMachineStateHistory();

                MachineStateStarted = DateTime.Now;
            }
        }


        public  void ResetTimerCycleTimeOut()
        {
            timerCycleTimeOut.Cancel();
            timerCycleTimeOut = ThreadPoolTimer.CreatePeriodicTimer(CycleTimeOut,
                                        TimeSpan.FromMilliseconds(CycleMachineConfiguration.MachineStateTimeOut));
        }


        public void StopMachineDataGeneration()
        {
            timerCycleMachineDataPublish.Cancel();
            timerCycleTimeOut.Cancel();
            _device.Inputs[CycleMachineConfiguration.CycleInputPin].InputChanged -= InputInterpretation;
        }


        private void feedMachineStateHistory()
        {


            if (MachineStateHistory.Count > 0 && MachineStateHistory.Last().EndDateTime == DateTime.MinValue)  // conclude last machine state..
            {
                MachineStateHistoryEntity lastEntity = MachineStateHistory.Last();
                if (LastMachineStateInMachineStateHistory == lastEntity.MachineState)
                {
                    lastEntity.EndDateTime = DateTime.Now;
                    lastEntity.Duration = lastEntity.EndDateTime - lastEntity.StartDateTime;

                    lastEntity.DailyCycleCoutner = DailyCycleCounter;
                    lastEntity.CyclesInThisPeriod = CyclesInThisPeriod;

                    DataStorageLibrary.DataStorageLibrary.AddDataMachineStateHistory(

              new Dictionary<string, string> {


           { "MachineState", lastEntity.MachineState.ToString() },
           { "DailyCycleCounter", lastEntity.DailyCycleCoutner.ToString() },
           { "CyclesInThisPeriod", lastEntity.CyclesInThisPeriod.ToString()},
           { "StartDateTime", lastEntity.StartDateTime.ToString() },
           { "EndDateTime", lastEntity.EndDateTime.ToString()},
           { "MachineId", CycleMachineConfiguration.MachineId.ToString() },
           { "Duration", lastEntity.Duration.ToString(@"dd\.hh\:mm\:ss") }
     });



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

            MachineStateHistory.Add(new MachineStateHistoryEntity(CurrentMachineState));
        }
    }
}
