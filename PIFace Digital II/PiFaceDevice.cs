using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIFace_Digital_II
{
    public class PiFaceDevice : IDevice
    {
        public int NumberOfOutputs { get; } = 8;
        public int NumberOfInputs { get; }  = 16;
        public double DeviceVersion { get; } = 2.0;
        public string DeviceName { get; } = "PiFace";

        public List<IInput> Inputs { get; } = new List<IInput>();

        public List<IOutput> Outputs { get; } = new List<IOutput>();

        public PiFaceDevice()
        {
            for (int i = 0; i < NumberOfInputs; i++)
            {
                this.Inputs.Add(new PiFaceInput(i));
            }

            for (int i = 0; i < NumberOfInputs; i++)
            {
                this.Outputs.Add(new PiFaceOutput(i));
            }
        }

        

        
    }

    public class PiFaceInput : IInput
    {
        private bool inputValue;
        private int index;
        public string Name { get; }
        public event Action<IInput> InputChanged;

        public PiFaceInput(int index)
        {
            this.index = index;
            Name = String.Format("Input{0}", index);
        }

        public bool InputValue
        {
            get
            {
                return inputValue;
            }

            set
            {
                inputValue = value;
                InputChanged?.Invoke(this);
            }
        }
    }

   public class PiFaceOutput : IOutput
    {
        private bool outputValue;
        public int OutputPin { get; }
        public string Name { get; }
        public event Action<IOutput> OutputChanged;

        public PiFaceOutput(int index)
        {
         OutputPin = index;
        Name = String.Format("Output{0}", index);
        }

        public bool OutputValue
        {
            get
            {
                return outputValue;
            }

            set
            {
                outputValue = value;
                OutputChanged?.Invoke(this);
            }
        }

        
    }
}
