using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
   public class MachineConfiguration
    {
        public string MachineName { get; set; }
        public int MachineId { get; set; }
        public int CycleInputPin { get; set; }
        public int MachineStateTimeout { get; set; }
        public int PublishingIntervall { get; set; }
    }
}
