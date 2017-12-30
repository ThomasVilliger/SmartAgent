using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStorageLibrary;

namespace PiFace_II
{
   public class Reporting : ReportConfiguration
    {
        private List<MachineStateHistory> machineStateHistory;
        public Reporting (List<Machine> machines, ReportConfiguration reportConfiguration)
        {
            MachineId = reportConfiguration.MachineId;
            EmailAddresses = reportConfiguration.EmailAddresses;

            machineStateHistory = machines.FirstOrDefault(m => m.MachineConfiguration.MachineId == MachineId).MachineStateHistory;

        }
    }
}
