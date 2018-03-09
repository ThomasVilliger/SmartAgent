using System.Threading.Tasks;

namespace DeviceMonitoring
{
    // interface to communicate with the device in the SmartAgent UWP Solution
   public interface IDeviceCommunication
    {
        Task SetDeviceInput(int pinNumber, bool state);
        Task SetDeviceOutput(int pinNumber, bool state);
        Task GetAllInputStates();
        Task GetAllOutputStates();
    }
}
