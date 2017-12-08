using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiFace_II.DeviceGatewayCommunication
{
  public interface  IMonitoringHubCommunication
    {
       void UpdateSingleInputState(IInput input);
       void UpdateSingleOutputState(IOutput output);

    }
}
