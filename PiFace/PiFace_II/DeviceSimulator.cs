using PiFace_II.DeviceGatewayCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace PiFace_II
{
    public class DeviceSimulator : IDevice
    {
        public static bool randomize;
        public int NumberOfOutputs { get; } = 8;
        public int NumberOfInputs { get; } = 8;
        public double DeviceVersion { get; } = 1.0;
        public string DeviceName { get; } = "Simulator";

        public List<IInput> Inputs { get; } = new List<IInput>();

        public List<IOutput> Outputs { get; } = new List<IOutput>();
       


        public DeviceSimulator(IMonitoringHubCommunication gatewayCommunication, bool produceRandomInputValues)
        {
            randomize = produceRandomInputValues;

            for (int i = 0; i < NumberOfInputs; i++)
            {
                this.Inputs.Add(new DeviceSimulatorInput(i));
                Inputs[i].InputChanged += gatewayCommunication.UpdateSingleInputState;
            }

            for (int i = 0; i < NumberOfInputs; i++)
            {
                this.Outputs.Add(new DeviceSimulatorOutput(i));
                Outputs[i].OutputChanged += gatewayCommunication.UpdateSingleOutputState;
            }
        }





        public void SetDeviceInput(int pinNumber, bool state)
        {
                if (pinNumber < NumberOfInputs)
                {
                    Inputs[pinNumber].State = state;
                }        
        }


        public void SetDeviceOutput(int pinNumber, bool state)
        {
                if (pinNumber < NumberOfInputs)
                {
                    Outputs[pinNumber].State = state;
                }
        }
    }





















    public class DeviceSimulatorInput : IInput
    {
        private int updateCounter;
        private bool inputValue;
        public int PinNumber { get; }
        public string Name { get; }
        public event Action<IInput> InputChanged;

        private ThreadPoolTimer timerRandomInputValue;

  

        public DeviceSimulatorInput(int index)
        {
            this.PinNumber = index;
            Name = String.Format("Input{0}", index);

            if (DeviceSimulator.randomize)
            {
                timerRandomInputValue = ThreadPoolTimer.CreatePeriodicTimer(UpdateSimValue,
                                   TimeSpan.FromMilliseconds(1000));
            }
        }




        public bool State
        {
            get
            {
                return inputValue;
            }

            set
            {
                if (inputValue != value)
                {
                    inputValue = value;
                    InputChanged?.Invoke(this);
                }
            }
        }



        private void UpdateSimValue(ThreadPoolTimer timer)
        {

            if (updateCounter == PinNumber)
            {


                Random r1 = new Random();
                if (r1.Next() % 2 == 0)
                {

                    State = true;
                }

                else
                {
                    State = false;
                }

            }
            updateCounter++;

            if (updateCounter >= 8 )
            {
                updateCounter = 0;
            }

        }
    }

    public class DeviceSimulatorOutput : IOutput
    {
        private bool outputValue;
        public int PinNumber { get; }
        public string Name { get; }
        public event Action<IOutput> OutputChanged;

        public DeviceSimulatorOutput(int index)
        {
            PinNumber = index;
            Name = String.Format("Output{0}", index);
        }

        public bool State
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
