
using SPI_GPIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using PiFace_II.DeviceGatewayCommunication;
using DataStorageLibrary;

namespace PiFace_II
{
   public class PiFaceMain
    {

        public IDevice device;

        public GatewayHubHandler GatewayHubHandler;
        public MonitoringHubHandler MonitoringHubHandler;

        public  PiFaceMain()
        {
            DataAccess.InitializeDatabase();
            chooseDevice();
           
            GatewayHubHandler = new GatewayHubHandler(device);
            MonitoringHubHandler = new MonitoringHubHandler(device);                                                                                                                                       
        }


        // dependency injection.. select the device you want to use by comment / uncomment
        private void chooseDevice()
        {
            //device = new PiFaceDevice(new SignalRMonitoringHubCommunication());
            //device = new PiFaceDevice(new RESTfulMonitoringHubCommunication());

            //   device = new DeviceSimulator(new RESTfulMonitoringHubCommunication(), true);
            device = new DeviceSimulator(new SignalRMonitoringHubCommunication(), true);
            //device = new DeviceSimulator(new RESTfulMonitoringHubCommunication(), false);
            //device = new DeviceSimulator(new SignalRMonitoringHubCommunication(), false);
        }
    }
}
