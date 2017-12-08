using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace PiFace_II
{
  public  class GatewayHubHandler
    {


        private HubConnection _hub;
        public List<CycleMachine> CycleMachines;
        public List<InputSignalMonitoring> InputMonitorings;
        private IDevice _device;
        private static ThreadPoolTimer _heartbeatWatchdog;


        public GatewayHubHandler(IDevice device)
        {


            _heartbeatWatchdog = ThreadPoolTimer.CreatePeriodicTimer(NoConnectionToGatewayHub,
                                   TimeSpan.FromMilliseconds(10000));

            CycleMachines = new List<CycleMachine>();

            

            InputMonitorings = new List<InputSignalMonitoring>();
            _device = device;
            EstablishHubConnection();


            CreateTestCycleMachines();
        }








        private void CreateTestCycleMachines()
        {
            CycleMachines.Add(new CycleMachine(_device, this, new CycleMachineConfiguration { MachineName = "MachineXY", CycleInputPin = 0, MachineId = 999, MachineStateTimeOut = 10000, PublishingIntervall = 500 })); // TODO do this foreach CycleMachine in configuration ressource
            CycleMachines.Add(new CycleMachine(_device, this, new CycleMachineConfiguration { MachineName = "MachineXY", CycleInputPin = 4, MachineId = 666, MachineStateTimeOut = 10000, PublishingIntervall = 5000 }));
            CycleMachines.Add(new CycleMachine(_device, this, new CycleMachineConfiguration { MachineName = "MachineXY", CycleInputPin = 7, MachineId = 555, MachineStateTimeOut = 10000, PublishingIntervall = 5000 }));
        }









        private void NoConnectionToGatewayHub(ThreadPoolTimer timer)
        {
            EstablishHubConnection();

            _heartbeatWatchdog.Cancel();
            _heartbeatWatchdog = ThreadPoolTimer.CreatePeriodicTimer(NoConnectionToGatewayHub,
                                  TimeSpan.FromMilliseconds(10000));

        }





        private async Task EstablishHubConnection()
        {
            string url = @"http://localhost:59162/GatewayHub";
            _hub = new HubConnectionBuilder().WithUrl(url).Build();

          
            _hub.On<List<Dictionary<string, string>>>("InitializeNewMachineConfigurations", cycleMachinesConfigurations => IntializeNewMachineConfigurations(cycleMachinesConfigurations));
            _hub.On<string>("TestSmartAgentConnection", ipAddress => TestSmartAgentConnection(ipAddress));
            _hub.On<bool>("GetAllSmartAgentConnections", p => GetAllSmartAgentConnections(p));
            _hub.On<bool>("Heartbeat", p => Heartbeat(p));

            await _hub.StartAsync(); 
        }

        private void Heartbeat(bool p)
        {
            _heartbeatWatchdog.Cancel();
            _heartbeatWatchdog = ThreadPoolTimer.CreatePeriodicTimer(NoConnectionToGatewayHub,
                                  TimeSpan.FromMilliseconds(10000));
        }

        private void GetAllSmartAgentConnections(bool p)
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostInfo = Dns.GetHostEntry(hostName);


            foreach (IPAddress ipOfTheSmartAgent in hostInfo.AddressList)
            {

                    _hub.InvokeAsync("ReturnSmartAgentConnection", hostName, ipOfTheSmartAgent.ToString());
       
            }
        }

        private void TestSmartAgentConnection(string ipAddress)
        {
            string HostName =  Dns.GetHostName();
            IPHostEntry hostInfo = Dns.GetHostEntry(HostName);


            foreach( IPAddress localIpAddress in hostInfo.AddressList)
            {
                if(localIpAddress.ToString() == ipAddress)
                {
                    _hub.InvokeAsync("TestSmartAgentConnectionResponse", true);
                }
            }          
        }




        private void IntializeNewMachineConfigurations(List<Dictionary<string, string>> cycleMachinesConfigurations)
        {
            CycleMachines.Clear();

            foreach (Dictionary<string, string> machineConfig in cycleMachinesConfigurations)
            {

                var config = new CycleMachineConfiguration
                {
                MachineName = machineConfig.FirstOrDefault(c => c.Key == "MachineName").Value,
                MachineId = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "MachineId").Value),
                CycleInputPin = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "CycleInputPin").Value),
                MachineStateTimeOut = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "MachineStateTimeOut").Value),
                PublishingIntervall = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "PublishingIntervall").Value)
                };

                CycleMachines.Add(new CycleMachine(_device, this, config));
            }
        }

        public void PublishActualCycleMachineData(CycleMachine cycleMachine)
        {
            var actualCycleMachineData = new Dictionary<string, string>
        {
           { "MachineState", cycleMachine.CurrentMachineState.ToString() },
           { "DailyCycleCounter", cycleMachine.DailyCycleCounter.ToString() },
           { "CycleTime", cycleMachine.LastCycleTime.ToString() },
           { "CycleCounterPerMachineState", cycleMachine.CycleCounterPerMachineState.ToString()},
           { "MachineId", cycleMachine.CycleMachineConfiguration.MachineId.ToString() },
           { "StateDuration", cycleMachine.StateDuration.ToString(@"dd\.hh\:mm\:ss") }
        };
            _hub.InvokeAsync("PublishActualCycleMachineData", actualCycleMachineData);
        }
    }
}
