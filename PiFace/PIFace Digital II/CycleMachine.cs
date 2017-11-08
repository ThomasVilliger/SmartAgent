using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace PIFace_Digital_II
{
  public  class CycleMachine : CycleMachineConfiguration 
    {
        public int DailyCycleCounter; // TODO Reset at 24:00:00
        public int CycleCounterPerMachineState;
        public long CycleTime;

        public List<MachineStateHistoryEntity> machineStateHistory;

        public enum MachineState  { Running, Stopped}
        public MachineState currentMachineState;
        public MachineState lastMachineStateInMachineStateHistory;

        private DispatcherTimer timerCycleTimeOut;

        Stopwatch cycleTimeStopWatch = new Stopwatch();
        

        public CycleMachine (IDevice iDevice, CycleMachineConfiguration cycleMachineConfiguration)
        {
            MachineName = cycleMachineConfiguration.MachineName;
            MachineId = cycleMachineConfiguration.MachineId;
            MachineStateTimeOut = cycleMachineConfiguration.MachineStateTimeOut;
            CycleInputPin = cycleMachineConfiguration.CycleInputPin;


            iDevice.Inputs[CycleInputPin].InputChanged += InputInterpretation;
            currentMachineState = MachineState.Stopped;
            lastMachineStateInMachineStateHistory = MachineState.Stopped;
            

            machineStateHistory = new List<MachineStateHistoryEntity>();

            timerCycleTimeOut = new DispatcherTimer();
            timerCycleTimeOut.Interval = TimeSpan.FromMilliseconds(MachineStateTimeOut); //after xSeconds no Cycle MachineState = Stopped
            timerCycleTimeOut.Tick += CycleTimeOut;
            timerCycleTimeOut.Start();

        }


        private void InputInterpretation(IInput input) 
        {
            if (input.InputValue == false) // after each falling flank..
            {
                DailyCycleCounter++;
                CycleCounterPerMachineState++;
                cycleTimeStopWatch.Stop();
                CycleTime = cycleTimeStopWatch.ElapsedMilliseconds;

            }

            else  // after each rising flank..
            {
                cycleTimeStopWatch.Restart();
            }

            
            ResetTimerCycleTimeOut();

            if(lastMachineStateInMachineStateHistory != MachineState.Running)
            {
                currentMachineState = MachineState.Running;
                feedMachineStateHistory();
            }

        }


        public void CycleTimeOut (object sender, object e)
        {    
            if (lastMachineStateInMachineStateHistory != MachineState.Stopped)
            {
                currentMachineState = MachineState.Stopped;
                feedMachineStateHistory();
            }
        }


        public  void ResetTimerCycleTimeOut()
        {
            timerCycleTimeOut.Stop();
            timerCycleTimeOut.Start();
        }


        private void feedMachineStateHistory()
        {
            
            
                if (machineStateHistory.Count > 0 && machineStateHistory.Last().EndDateTime == DateTime.MinValue)  // conclude last machine state..
                {
                     MachineStateHistoryEntity lastMachineStateHistoryEntity = machineStateHistory.Last();
                if (lastMachineStateInMachineStateHistory == lastMachineStateHistoryEntity.MachineState)
                    {
                    lastMachineStateHistoryEntity.EndDateTime = DateTime.Now;
                    lastMachineStateHistoryEntity.Duration = lastMachineStateHistoryEntity.EndDateTime - lastMachineStateHistoryEntity.StartDateTime;

                    lastMachineStateHistoryEntity.DailyCycleCoutner = DailyCycleCounter;
                    lastMachineStateHistoryEntity.CycleCounterPerMachineState = CycleCounterPerMachineState;
                }
                }

            machineStateHistory.Add(new MachineStateHistoryEntity(currentMachineState));

            lastMachineStateInMachineStateHistory = currentMachineState;
            CycleCounterPerMachineState = 0;
        }
    }
}
