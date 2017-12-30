using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorageLibrary
{
   public class MachineStateHistory
    {
        public enum State {Stopped, Running }


        public int SmartAgentHistoryId { get; set; }
        public State MachineState { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int DailyCycleCounter { get; set; }
        public int CyclesInThisPeriod { get; set; }
        public int MachineId { get; set; }

        public MachineStateHistory(State machineState, int machineId)
        {
            StartDateTime = DateTime.Now;
            MachineState = machineState;
            EndDateTime = DateTime.MinValue;
            Duration = TimeSpan.Zero;
            MachineId = machineId;
        }

        public MachineStateHistory()
        {
        }

    }



}
