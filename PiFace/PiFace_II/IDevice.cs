using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiFace_II.DeviceGatewayCommunication;

namespace PiFace_II
{
    public interface IDevice
    {
        int NumberOfOutputs { get; }
        int NumberOfInputs { get; }
        string DeviceName { get; }
        double DeviceVersion { get; }
        List<IInput> Inputs { get; }
        List<IOutput> Outputs { get; }
        void SetDeviceInput(int pinNumber, bool state);
        void SetDeviceOutput(int pinNumber, bool state);

    }


    public interface IInput
    {
        string Name { get; }
        bool State { get;  set; }
        event Action<IInput> InputChanged;
        int PinNumber { get; }
    }


    public interface IOutput

    { 
        string Name { get; }
        bool State { get; set; }
        event Action<IOutput> OutputChanged;
        int PinNumber { get; }
    }
}
