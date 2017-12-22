using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiFace_II
{
  public  class InputMonitoringConfiguration
    {
        public int InputPin { get; set; }
        public int OutputPin { get; set; }
        public List<string> EmailAddressListForNotification { get; set; }
    }
}
