using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiFace_II
{
    public  class CycleMachineConfiguration
    {
        public string MachineName { get; set; }
        public int MachineId { get; set; }
        public int CycleInputPin { get; set; }
        public int MachineStateTimeOut { get; set; }
        public int PublishingIntervall { get; set; }
    }
}
