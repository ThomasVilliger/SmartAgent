using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIFace_Digital_II
{
   public class MachineStateHistoryEntity
    {
        public CycleMachine.MachineState MachineState { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan Duration { get; set; }

        public int DailyCycleCoutner { get; set; }
        public int CycleCounterPerMachineState { get; set; }

        public  MachineStateHistoryEntity(CycleMachine.MachineState machineState)
        {
            StartDateTime = DateTime.Now;
            MachineState = machineState;
            EndDateTime = DateTime.MinValue;
            Duration = TimeSpan.Zero;
        }

        public MachineStateHistoryEntity()
        {
        }
    }
}
