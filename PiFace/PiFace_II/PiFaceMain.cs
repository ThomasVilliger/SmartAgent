
using SPI_GPIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace PiFace_II
{
   public class PiFaceMain
    {

        public static List<CycleMachine> cycleMachines = new List<CycleMachine>();
        private static List<InputSignalMonitoring> inputSignalMonitorings = new List<InputSignalMonitoring>();
        private static readonly HttpClient client = new HttpClient();

        public IDevice device;

        public  PiFaceMain()
        {
            chooseDevice();
            cycleMachines.Add(new CycleMachine(device, new CycleMachineConfiguration { MachineName = "MachineXY", CycleInputPin = 0, MachineId = 999, MachineStateTimeOut = 10000 })); // TODO do this foreach CycleMachine in configuration ressource

            inputSignalMonitorings.Add(new InputSignalMonitoring(device, new InputSignalMonitoringConfiguration { InputPinToMonitor = 0, OutputPinForNotification = 0 }));
            inputSignalMonitorings.Add(new InputSignalMonitoring(device, new InputSignalMonitoringConfiguration { InputPinToMonitor = 2, OutputPinForNotification = 6 })); // pin8 = pin0 ???
                                                                                                                                                                           //inputSignalMonitorings.Add(new InputSignalMonitoring(device, new InputSignalMonitoringConfiguration { InputPinToMonitor = 1, OutputPinForNotification = 8 }));

            foreach (IOutput iOutput in device.Outputs)
            {
                iOutput.OutputChanged += updateLocalWebAppViewAsync;
            }



            foreach (IInput iInput in device.Inputs)
            {
                iInput.InputChanged += updateLocalWebAppViewAsync;
            }
        }




        // dependency injection.. select the device you want to use by comment / uncomment
        private void chooseDevice()
        {
            //device = new PiFaceDevice();
            device = new DeviceSimulator(true);

        }



        public  void WriteDeviceInput(int pinNumber, bool state)
        {
            try
            {
                if (pinNumber <= device.NumberOfInputs)
                {
                    device.Inputs[pinNumber].State = state;
                }
            }
            catch (Exception)
            {

            }

        }


        public void WriteDeviceOutput(int pinNumber, bool state)
        {
            try
            {
                if (pinNumber <= device.NumberOfInputs)
                {
                    device.Outputs[pinNumber].State = state;
                }
            }
            catch (Exception)
            {

            }

        }






        private  void  updateLocalWebAppViewAsync(IOutput ioutput)
        {


            var values = new Dictionary<string, string>
{
   { "PinNumber", ioutput.PinNumber.ToString() },
   { "State", ioutput.State.ToString() }
};



            var content = new FormUrlEncodedContent(values);


            string url = String.Format(@"http://192.168.0.13:59162/DeviceMonitoring/UpdateSingleOutputState/");

            var response = client.PostAsync(url, content);


        }


        private void updateLocalWebAppViewAsync(IInput iInput)
        {
            var values = new Dictionary<string, string>
{
   { "PinNumber", iInput.PinNumber.ToString() },
   { "State", iInput.State.ToString() }
};

            var content = new FormUrlEncodedContent(values);


            string url = String.Format(@"http://192.168.0.13:59162/DeviceMonitoring/UpdateSingleInputState/");

            var response = client.PostAsync(url, content);





        }

    }
}
