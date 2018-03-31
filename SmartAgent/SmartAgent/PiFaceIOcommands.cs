using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using SPI_GPIO;
using Windows.UI.Xaml.Input;


namespace SmartAgent
{
    // this class provides the methods to communicate with the MCP23S17 library
    // it checks the inputs all 200ms for changes on the MCP23S17 chip

    public static class PiFaceIOcommands
    {
        private static DispatcherTimer _timerInputReader;         // create a timer
        private const byte _off = MCP23S17.Off;
        private const byte _on = MCP23S17.On;
        private static PiFaceDevice _piFaceDevice;
        private static UInt16[] _oldInputValues = new UInt16[16];
        public static ushort OldInputValues { get; private set; } = 255;

        public static void InitPiFaceIO(PiFaceDevice device)
        {
            _piFaceDevice = device;
            InitTimer();
        }

        private static void MainPage_Unloaded(object sender, object args)
        {
            /* Cleanup */
        }

        private static void InitTimer()
        {
            // read timer
            _timerInputReader = new DispatcherTimer();
            _timerInputReader.Interval = TimeSpan.FromMilliseconds(200); //sample every 200mS
            _timerInputReader.Tick += Timer_Tick_ReadInputs;
            _timerInputReader.Start();
        }
        // read GPIO and display it
        private static void Timer_Tick_ReadInputs(object sender, object e)
        {
            ReadInputs();
        }

        public static void ReadInputs()
        {
            UInt16 value = MCP23S17.ReadRegister16();  // read all the input values
            if (value != OldInputValues)               // if something changed read each input
            {
                for (byte i = 0; i < _piFaceDevice.NumberOfInputs; i++)
                {
                    var val = (MCP23S17.ReadPin(i)); // read single Input

                    if (_oldInputValues[i] != val)        // if input changed set input on PiFaceDevice
                    {
                        _piFaceDevice.Inputs[i].State = Convert.ToBoolean(val);
                        _oldInputValues[i] = val;
                    }
                }
                OldInputValues = value;
            }
        }

        // write output on the MCP23S17 chip
        public static void WriteOutput(IOutput output)
        {
            MCP23S17.WritePin(Convert.ToByte(output.PinNumber + 8), Convert.ToByte(output.State));
        }
    }
}
