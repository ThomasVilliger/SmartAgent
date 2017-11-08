using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restup.DemoControllers.Model
{
    public sealed class CurrentMachineData
    {
        public string MachineState { get; set; }
        public int CycleCounterPerMachineState { get; set; }
        public int DailyCycleCunter { get; set; }
        public long LastCycleTime  { get; set; }
        
    }
}
