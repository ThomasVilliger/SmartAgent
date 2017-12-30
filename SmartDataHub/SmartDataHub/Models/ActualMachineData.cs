using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartDataHub.Models
{
    public class ActualMachineData
    {
        public string MachineState { get; set; }
        public string StateDuration { get; set; }
        public int DailyCycleCounter { get; set; }
        public int CyclesInThisPeriod { get; set; }
        public int MachineId { get; set; }
        public long LastCycleTime { get; set; }
    }
}