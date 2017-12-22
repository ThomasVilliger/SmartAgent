using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartDataHub.Models
{
    public class ActualCycleMachineData
    {
        public string MachineState { get; set; }
        public string DailyCycleCounter { get; set; }
        public string CycleTime { get; set; }
        public string CyclesInThisPeriod { get; set; }
        public string StateDuration { get; set; }

    }
}