using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceMonitoring
{
    // interface to communicate with the device in the SmartAgent UWP Solution
   public interface IDeviceCommunication
    {
        Task SetDeviceInput(PinState pinState);
        Task SetDeviceOutput(PinState pinState);
        Task <List<PinState>> GetAllInputStates();
        Task <List<PinState>> GetAllOutputStates();
    }
}
