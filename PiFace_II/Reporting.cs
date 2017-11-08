using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIFace_II
{
   public class Reporting : ReportConfiguration
    {
        private List<MachineStateHistoryEntity> machineStateHistory;
        public Reporting (List<CycleMachine> cycleMachines, ReportConfiguration reportConfiguration)
        {
            MachineId = reportConfiguration.MachineId;
            EmailAddresses = reportConfiguration.EmailAddresses;

            machineStateHistory = cycleMachines.FirstOrDefault(m => m.CycleMachineConfiguration.MachineId == MachineId).MachineStateHistory;

        }
    }
}
