
using SPI_GPIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using PiFace_II.DeviceGatewayCommunication;

namespace PiFace_II
{
   public class PiFaceMain
    {

        //public static List<CycleMachine> cycleMachines = new List<CycleMachine>();
        //private static List<InputSignalMonitoring> inputSignalMonitorings = new List<InputSignalMonitoring>();
        private static readonly HttpClient client = new HttpClient();

        public IDevice device;

        public GatewayHubHandler GatewayHubHandler;
        public MonitoringHubHandler MonitoringHubHandler;

        public  PiFaceMain()
        {        
            chooseDevice();
           
            GatewayHubHandler = new GatewayHubHandler(device);
            MonitoringHubHandler = new MonitoringHubHandler(device);

            //cycleMachines.Add(new CycleMachine(device, gatewayHubHandler, new CycleMachineConfiguration { MachineName = "MachineXY", CycleInputPin = 0, MachineId = 999, MachineStateTimeOut = 10000, PublishingIntervall=5000 })); // TODO do this foreach CycleMachine in configuration ressource
            //cycleMachines.Add(new CycleMachine(device, gatewayHubHandler, new CycleMachineConfiguration { MachineName = "MachineXY", CycleInputPin = 4, MachineId = 666, MachineStateTimeOut = 10000, PublishingIntervall = 5000 }));
            //cycleMachines.Add(new CycleMachine(device, gatewayHubHandler, new CycleMachineConfiguration { MachineName = "MachineXY", CycleInputPin = 7, MachineId = 555, MachineStateTimeOut = 10000, PublishingIntervall = 5000 }));









            //inputSignalMonitorings.Add(new InputSignalMonitoring(device, new InputSignalMonitoringConfiguration { InputPinToMonitor = 0, OutputPinForNotification = 0 }));
            //inputSignalMonitorings.Add(new InputSignalMonitoring(device, new InputSignalMonitoringConfiguration { InputPinToMonitor = 2, OutputPinForNotification = 6 })); // pin8 = pin0 ???                                                                                                                                                                        //inputSignalMonitorings.Add(new InputSignalMonitoring(device, new InputSignalMonitoringConfiguration { InputPinToMonitor = 1, OutputPinForNotification = 8 }));         
        }


        // dependency injection.. select the device you want to use by comment / uncomment
        private void chooseDevice()
        {
            // device = new PiFaceDevice(new SignalRMonitoringHubCommunication());
            //device = new PiFaceDevice(new RESTfulMonitoringHubCommunication());

            //   device = new DeviceSimulator(new RESTfulMonitoringHubCommunication(), true);
            device = new DeviceSimulator(new SignalRMonitoringHubCommunication(), true);
            //device = new DeviceSimulator(new RESTfulMonitoringHubCommunication(), false);
            //device = new DeviceSimulator(new SignalRMonitoringHubCommunication(), false);
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
    }
}
