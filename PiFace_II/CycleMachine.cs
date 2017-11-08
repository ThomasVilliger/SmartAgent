using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.System.Threading;

namespace PIFace_II
{
  public  class CycleMachine 
    {
        public int DailyCycleCounter { get; set; } // TODO Reset at 24:00:00
        public int CycleCounterPerMachineState { get; set; }
        public long LastCycleTime { get; set; }

        public CycleMachineConfiguration CycleMachineConfiguration { get; set; }

        public List<MachineStateHistoryEntity> MachineStateHistory { get; set; }

        public enum MachineState  { Running, Stopped}
        public MachineState CurrentMachineState { get; set; }
        public MachineState LastMachineStateInMachineStateHistory { get; set; }

        // private DispatcherTimer timerCycleTimeOut;

        private ThreadPoolTimer timerCycleTimeOut;

        Stopwatch cycleTimeStopWatch = new Stopwatch();
        

        public CycleMachine (IDevice iDevice, CycleMachineConfiguration cycleMachineConfiguration)
        {
            this.CycleMachineConfiguration = cycleMachineConfiguration;


            iDevice.Inputs[cycleMachineConfiguration.CycleInputPin].InputChanged += InputInterpretation;
            CurrentMachineState = MachineState.Stopped;
            LastMachineStateInMachineStateHistory = MachineState.Stopped;
            

            MachineStateHistory = new List<MachineStateHistoryEntity>();

     timerCycleTimeOut =     ThreadPoolTimer.CreatePeriodicTimer(CycleTimeOut,
                                        TimeSpan.FromMilliseconds(cycleMachineConfiguration.MachineStateTimeOut));


            //timerCycleTimeOut = new DispatcherTimer();
            //timerCycleTimeOut.Interval = TimeSpan.FromMilliseconds(MachineStateTimeOut); //after xSeconds no Cycle MachineState = Stopped
            //timerCycleTimeOut.Tick += CycleTimeOut;
            //timerCycleTimeOut.Start();

        }

        private void CycleTimeOut(ThreadPoolTimer timer)
        {
            if (LastMachineStateInMachineStateHistory != MachineState.Stopped)
            {
                CurrentMachineState = MachineState.Stopped;
                feedMachineStateHistory();
            }
        }

        private void InputInterpretation(IInput input) 
        {
            if (input.InputValue == false) // after each falling flank..
            {
                DailyCycleCounter++;
                CycleCounterPerMachineState++;
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
            }
        }


        public  void ResetTimerCycleTimeOut()
        {
            timerCycleTimeOut.Cancel();
            timerCycleTimeOut = ThreadPoolTimer.CreatePeriodicTimer(CycleTimeOut,
                                        TimeSpan.FromMilliseconds(CycleMachineConfiguration.MachineStateTimeOut));
        }


        private void feedMachineStateHistory()
        {
            
            
                if (MachineStateHistory.Count > 0 && MachineStateHistory.Last().EndDateTime == DateTime.MinValue)  // conclude last machine state..
                {
                     MachineStateHistoryEntity lastMachineStateHistoryEntity = MachineStateHistory.Last();
                if (LastMachineStateInMachineStateHistory == lastMachineStateHistoryEntity.MachineState)
                    {
                    lastMachineStateHistoryEntity.EndDateTime = DateTime.Now;
                    lastMachineStateHistoryEntity.Duration = lastMachineStateHistoryEntity.EndDateTime - lastMachineStateHistoryEntity.StartDateTime;

                    lastMachineStateHistoryEntity.DailyCycleCoutner = DailyCycleCounter;
                    lastMachineStateHistoryEntity.CycleCounterPerMachineState = CycleCounterPerMachineState;
                }
                }

            MachineStateHistory.Add(new MachineStateHistoryEntity(CurrentMachineState));

            LastMachineStateInMachineStateHistory = CurrentMachineState;
            CycleCounterPerMachineState = 0;
        }
    }
}
