using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorageLibrary
{
   public class MachineStateHistoryEntity
    {
        public string MachineState { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string Duration { get; set; }
        public int DailyCycleCoutner { get; set; }
        public int CyclesInThisPeriod { get; set; }
        public int MachineId { get; set; }

    }



}
