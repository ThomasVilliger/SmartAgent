using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIFace_II
{
  public  class InputSignalMonitoringConfiguration
    {
        public int InputPinToMonitor;
        public int OutputPinForNotification;
        public List<string> EmailAddressListForNotification; 
    }
}
