using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIFace_II
{
    public interface IDevice
    { 
        int NumberOfOutputs { get; }
        int NumberOfInputs { get; }
        string DeviceName { get; }
        double DeviceVersion { get; }
        List<IInput>Inputs { get;}
        List<IOutput> Outputs { get; }

    }


    public interface IInput
    {
        string Name { get; }
        bool InputValue { get; set; }
        event Action<IInput> InputChanged;
    }


    public interface IOutput

    { 
        string Name { get; }
        bool OutputValue { get; set; }
         event Action<IOutput> OutputChanged;
        int OutputPin { get; }
    }
}
