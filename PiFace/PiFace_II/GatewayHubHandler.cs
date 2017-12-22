using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace PiFace_II
{
  public  class GatewayHubHandler
    {


        private HubConnection _hub;
        public List<CycleMachine> CycleMachines;
        public List<InputMonitoring> InputMonitorings;
        public IDevice Device;
        private  ThreadPoolTimer _heartbeatWatchdog;


        public GatewayHubHandler(IDevice device)
        {


            _heartbeatWatchdog = ThreadPoolTimer.CreatePeriodicTimer(NoConnectionToGatewayHub,
                                   TimeSpan.FromMilliseconds(10000));

            CycleMachines = new List<CycleMachine>();

            

            InputMonitorings = new List<InputMonitoring>();
            Device = device;
            EstablishHubConnection();


            loadMachineConfiguration();
        }



        private void loadMachineConfiguration()
        {

            foreach(CycleMachine c in CycleMachines)
            {
                c.StopMachineDataGeneration();
            }

            CycleMachines.Clear();
          
            var cycleMachineConfigurations = DataStorageLibrary.DataStorageLibrary.GetCycleMachineConfigurations();
            foreach (Dictionary<string, string> machineConfig in cycleMachineConfigurations)
            {
                var config = new CycleMachineConfiguration
                {
                    MachineName = machineConfig.FirstOrDefault(c => c.Key == "MachineName").Value,
                    MachineId = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "MachineId").Value),
                    CycleInputPin = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "CycleInputPin").Value),
                    MachineStateTimeOut = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "MachineStateTimeout").Value),
                    PublishingIntervall = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "PublishingIntervall").Value)
                };
                CycleMachines.Add(new CycleMachine(Device, this, config));
            }



            var inputMonitoringConfigurations = DataStorageLibrary.DataStorageLibrary.GetInputMonitoringConfigurations();
            foreach (Dictionary<string, string> monitoringConfig in inputMonitoringConfigurations)

            {
                var config = new InputMonitoringConfiguration
                {
                    InputPin = Convert.ToInt32(monitoringConfig.FirstOrDefault(c => c.Key == "InputPin").Value),
                    OutputPin = Convert.ToInt32(monitoringConfig.FirstOrDefault(c => c.Key == "OutputPin").Value),
                };

                InputMonitorings.Add(new InputMonitoring(Device, config));
            }
        }


        private void NoConnectionToGatewayHub(ThreadPoolTimer timer)
        {

            _hub.DisposeAsync();
            EstablishHubConnection();

            _heartbeatWatchdog.Cancel();
            _heartbeatWatchdog = ThreadPoolTimer.CreatePeriodicTimer(NoConnectionToGatewayHub,
                                  TimeSpan.FromMilliseconds(10000));

        }


        private async Task EstablishHubConnection()
        {
            string url = @"http://192.168.0.13:59162/GatewayHub";
            _hub = new HubConnectionBuilder().WithUrl(url).Build();
              
            _hub.On<string>("TestSmartAgentConnection", p => TestSmartAgentConnection(p));
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

        private void TestSmartAgentConnection(string p)
        {
           _hub.InvokeAsync("TestSmartAgentConnectionResponse", true);        
        }


        public void InitializeNewMachineConfigurations(List<DataStorageLibrary.CycleMachineConfiguration> cycleMachinesConfigurations)
        {

            // CycleMachines.Clear();

            //foreach (DataStorageLibrary.CycleMachineConfiguration machineConfig in cycleMachinesConfigurations)
            //{
            //    CycleMachines.Add(new CycleMachine(Device, this, machineConfig));
            //}



            DataStorageLibrary.DataStorageLibrary.StoreCycleMachineConfigurations(cycleMachinesConfigurations);

            loadMachineConfiguration();
        }



        public void InitializeNewMachineConfigurations(StringContent configuration)
        {
            CycleMachines.Clear();

            //foreach (Dictionary<string, string> machineConfig in cycleMachinesConfigurations)
            //{

            //    var config = new CycleMachineConfiguration
            //    {
            //        MachineName = machineConfig.FirstOrDefault(c => c.Key == "MachineName").Value,
            //        MachineId = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "MachineId").Value),
            //        CycleInputPin = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "CycleInputPin").Value),
            //        MachineStateTimeOut = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "MachineStateTimeOut").Value),
            //        PublishingIntervall = Convert.ToInt32(machineConfig.FirstOrDefault(c => c.Key == "PublishingIntervall").Value)
            //    };

            //    CycleMachines.Add(new CycleMachine(Device, this, config));
            //}

            //DataStorageLibrary.DataStorageLibrary.StoreCycleMachineConfigurations(cycleMachinesConfigurations);
        }







        public void InitializeNewInputMonitoringConfigurations(List<Dictionary<string, string>> inputMonitoringConfigurations)
        {
            InputMonitorings.Clear();
            foreach (Dictionary<string, string> monitoringConfig in inputMonitoringConfigurations)
            {

                var config = new InputMonitoringConfiguration
                {
                    InputPin = Convert.ToInt32(monitoringConfig.FirstOrDefault(c => c.Key == "InputPin").Value),
                    OutputPin = Convert.ToInt32(monitoringConfig.FirstOrDefault(c => c.Key == "OutputPin").Value)
                };

                InputMonitorings.Add(new InputMonitoring(Device, config));
            }  
            //DataStorageLibrary.DataStorageLibrary.StoreCycleMachineConfigurations(inputMonitoringConfigurations);
        }


        public void PublishActualCycleMachineData(CycleMachine cycleMachine)
        {
            var actualCycleMachineData = new Dictionary<string, string>
        {
           { "MachineState", cycleMachine.CurrentMachineState.ToString() },
           { "DailyCycleCounter", cycleMachine.DailyCycleCounter.ToString() },
           { "CycleTime", cycleMachine.LastCycleTime.ToString() },
           { "CyclesInThisPeriod", cycleMachine.CyclesInThisPeriod.ToString()},
           { "MachineId", cycleMachine.CycleMachineConfiguration.MachineId.ToString() },
           { "StateDuration", cycleMachine.StateDuration.ToString(@"dd\.hh\:mm\:ss") }
        };
            _hub.InvokeAsync("PublishActualCycleMachineData", actualCycleMachineData);
        }
    }
}
