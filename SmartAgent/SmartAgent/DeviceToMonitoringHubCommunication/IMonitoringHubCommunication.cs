using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAgent.DeviceGatewayCommunication
{
    // interface to communicate to the SmartAgent Monitoring
  public interface  IMonitoringHubCommunication
    {
       void UpdateSingleInputState(IInput input);
       void UpdateSingleOutputState(IOutput output);

    }
}
