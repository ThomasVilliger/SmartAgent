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


namespace PiFace_II
{
   public static class PiFaceIOcommands
    {

        private static DispatcherTimer timerInputReader;         // create a timer
        private const byte Off = MCP23S17.Off;
        private const byte On = MCP23S17.On;
        private static PiFaceDevice piFaceDevice;
        private static UInt16[] oldInputValues = new UInt16[16];

        public static ushort OldInputValues { get; private set; } = 255;



        public  static void InitPiFaceIO(PiFaceDevice device)
        {

            piFaceDevice = device;
            initTimer();

        }

        private static void MainPage_Unloaded(object sender, object args)
        {
            /* Cleanup */
        }

        private static void initTimer()
        {
            // read timer
            timerInputReader = new DispatcherTimer();
            timerInputReader.Interval = TimeSpan.FromMilliseconds(200); //sample every 200mS
            timerInputReader.Tick += Timer_Tick_ReadInputs;
            timerInputReader.Start();
        }
        // read GPIO and display it
        private static void Timer_Tick_ReadInputs(object sender, object e)
        {
            /*LightLEDs(MCP23S17.ReadRegister16());  */  // do something with the values
            ReadInputs();
        }



        public static void ReadInputs()
        {

            UInt16 value = MCP23S17.ReadRegister16();  // read all the input values
            if (value != OldInputValues)               // if something changed read each input
            {
                for (byte i = 0; i < piFaceDevice.NumberOfInputs; i++)
                {
                    var val = (MCP23S17.ReadPin(i)); // read single Input

                    if (oldInputValues[i] != val)        // if input changed set input on PiFaceDevice
                    {
                        piFaceDevice.Inputs[i].State = Convert.ToBoolean(val);
                        oldInputValues[i] = val;
                    }
                }
                OldInputValues = value;
            }
        }



        public static void WriteOutput(IOutput output)
        {
            MCP23S17.WritePin(Convert.ToByte(output.PinNumber + 8), Convert.ToByte(output.State));
        }





       


        //private void LightLEDs(UInt16 Inputs)
        //{
        //    image0.Source = ((Inputs & 1 << PFDII.IN0) != 0 && !Switch0.IsPressed) ? imageOn.Source : imageOff.Source;
        //    image1.Source = ((Inputs & 1 << PFDII.IN1) != 0 && !Switch1.IsPressed) ? imageOn.Source : imageOff.Source;
        //    image2.Source = ((Inputs & 1 << PFDII.IN2) != 0 && !Switch2.IsPressed) ? imageOn.Source : imageOff.Source;
        //    image3.Source = ((Inputs & 1 << PFDII.IN3) != 0 && !Switch3.IsPressed) ? imageOn.Source : imageOff.Source;
        //    image4.Source = ((Inputs & 1 << PFDII.IN4) != 0) ? imageOn.Source : imageOff.Source;
        //    image5.Source = ((Inputs & 1 << PFDII.IN5) != 0) ? imageOn.Source : imageOff.Source;
        //    image6.Source = ((Inputs & 1 << PFDII.IN6) != 0) ? imageOn.Source : imageOff.Source;
        //    image7.Source = ((Inputs & 1 << PFDII.IN7) != 0) ? imageOn.Source : imageOff.Source;

        //    image0_Out.Source = ((Inputs & 1 << PFDII.LED0) != 0) ? imageROn.Source : imageROff.Source;
        //    image1_Out.Source = ((Inputs & 1 << PFDII.LED1) != 0) ? imageROn.Source : imageROff.Source;
        //    image2_Out.Source = ((Inputs & 1 << PFDII.LED2) != 0) ? imageROn.Source : imageROff.Source;
        //    image3_Out.Source = ((Inputs & 1 << PFDII.LED3) != 0) ? imageROn.Source : imageROff.Source;
        //    image4_Out.Source = ((Inputs & 1 << PFDII.LED4) != 0) ? imageROn.Source : imageROff.Source;
        //    image5_Out.Source = ((Inputs & 1 << PFDII.LED5) != 0) ? imageROn.Source : imageROff.Source;
        //    image6_Out.Source = ((Inputs & 1 << PFDII.LED6) != 0) ? imageROn.Source : imageROff.Source;
        //    image7_Out.Source = ((Inputs & 1 << PFDII.LED7) != 0) ? imageROn.Source : imageROff.Source;

        //}

        ////Note RelayA = R1, RelayB = R0
        //private void RelayA_Checked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.RelayA, On);
        //    //RelayAImage.Source = RelayOn.Source;
        //    //LED1.IsChecked = true; ;
        //}

        //private void RelayB_Checked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.RelayB, On);
        //    //RelayBImage.Source = RelayOn.Source;
        //    //LED0.IsChecked = true; ;
        //}

        //private void RelayA_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.RelayA, Off);
        //    //RelayAImage.Source = RelayOff.Source;
        //    //LED1.IsChecked = false; ;
        //}

        //private void RelayB_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.RelayB, Off);
        //    //RelayBImage.Source = RelayOff.Source;
        //    //LED0.IsChecked = false; ;
        //}

        //private void LED0_Checked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED0, On);
        //    //RelayB.IsChecked = true; // LED0 and RelayA are the same output pin
        //}

        //private void LED1_Checked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED1, On);
        //    //RelayA.IsChecked = true; // LED1 and RelayB are the same output pin
        //}

        //private void LED2_Checked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED2, On);
        //}

        //private void LED3_Checked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED3, On);
        //}

        //private void LED4_Checked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED4, On);
        //}

        //private void LED5_Checked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED5, On);
        //}

        //private void LED6_Checked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED6, On);
        //}

        //private void LED7_Checked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED7, On);
        //}

        //private void LED0_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED0, Off);
        //    //RelayB.IsChecked = false; // LED0 and RelayA are the same output pin

        //}

        //private void LED1_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED1, Off);
        //    //RelayA.IsChecked = false; // LED1 and RelayB are the same output pin
        //}

        //private void LED2_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED2, Off);
        //}

        //private void LED3_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED3, Off);
        //}

        //private void LED4_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED4, Off);
        //}

        //private void LED5_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED5, Off);
        //}

        //private void LED6_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED6, Off);
        //}

        //private void LED7_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    MCP23S17.WritePin(PFDII.LED7, Off);
        //}

        //private void Switch0_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    Switch0.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        //}

        //private void Switch0_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    Switch0.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
        //}
        //private void Switch1_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    Switch1.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        //}

        //private void Switch1_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    Switch1.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
        //}
        //private void Switch2_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    Switch2.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        //}

        //private void Switch2_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    Switch2.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
        //}
        //private void Switch3_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    Switch3.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        //}

        //private void Switch3_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    Switch3.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
        //}

        //private void btnExit_Click(object sender, RoutedEventArgs e)
        //{
        //    App2.Current.Exit(); // exit the app
        //}
    }
}
