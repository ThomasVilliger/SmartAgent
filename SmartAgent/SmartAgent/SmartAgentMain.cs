
using SPI_GPIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using SmartAgent.DeviceGatewayCommunication;
using DataAccess;

namespace SmartAgent
{
    public class SmartAgentMain
    {
        // starting class of the SmartAgent
        public IDevice Device;

        public GatewayHubHandler GatewayHubHandler;
        public MonitoringHubHandler MonitoringHubHandler;

        public SmartAgentMain()
        {
            DataAccess.DataAccess.InitializeDatabase();
            ChooseDevice();

            GatewayHubHandler = new GatewayHubHandler(Device);
            //MonitoringHubHandler = new MonitoringHubHandler(Device);
        }


        // dependency injection.. select the device you want to use by comment / uncomment
        private void ChooseDevice()
        {
            //Device = new PiFaceDevice(new SignalRMonitoringHubCommunication());
            //Device = new PiFaceDevice(new RESTfulMonitoringHubCommunication());

            //Device = new DeviceSimulator(new RESTfulMonitoringHubCommunication(), false);
            //Device = new DeviceSimulator(new SignalRMonitoringHubCommunication(), true);
            Device = new DeviceSimulator(new RESTfulMonitoringHubCommunication(), true);
            //Device = new DeviceSimulator(new SignalRMonitoringHubCommunication(), false);
        }
    }
}
