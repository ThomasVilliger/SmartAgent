using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceMonitoring
{
   public interface IDeviceCommunication
    {
        Task SetDeviceInput(int pinNumber, bool state);
        Task SetDeviceOutput(int pinNumber, bool state);
        Task GetAllInputStates();
        Task GetAllOutputStates();
    }
}
