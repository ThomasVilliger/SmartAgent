﻿using SPI_GPIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiFace_II
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

            initPiFace();
        }





        private void initPiFace()
        {
            initSPIchip();
            PiFaceIOcommands.InitPiFaceIO(this);

            foreach (IOutput iOutput in Outputs)
            {
                iOutput.OutputChanged += PiFaceIOcommands.WriteOutput;
            }
        }


        private async void initSPIchip()
        {
            try
            {
                await MCP23S17.InitSPI();

                MCP23S17.InitMCP23S17();
                MCP23S17.setPinMode(0x00FF);
                MCP23S17.pullupMode(0x00FF);
                MCP23S17.WriteWord(0x0000);
            }
            catch (Exception ex)
            {

            }

        }




    }

    public class PiFaceInput : IInput
    {
        private bool state;
        public int PinNumber { get; }
        public string Name { get; }
        public event Action<IInput> InputChanged;

        public PiFaceInput(int pinNumber)
        {
            PinNumber = pinNumber;
            Name = String.Format("Input{0}", pinNumber);
        }

        public bool State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
                InputChanged?.Invoke(this);
            }
        }
    }

   public class PiFaceOutput : IOutput
    {
        private bool state;
        public int PinNumber { get; }
        public string Name { get; }
        public event Action<IOutput> OutputChanged;

        public PiFaceOutput(int pinNumber)
        {
         PinNumber = pinNumber;
        Name = String.Format("Output{0}", pinNumber);
        }

        public bool State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
                OutputChanged?.Invoke(this);
            }
        }

        
    }
}