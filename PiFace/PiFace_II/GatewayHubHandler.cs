using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using DataStorageLibrary;

namespace PiFace_II
{
    public class GatewayHubHandler
    {


        private HubConnection _gatewayHub;
        public List<Machine> Machines;
        public List<InputMonitoring> InputMonitorings;
        public IDevice Device;
        //private  ThreadPoolTimer _heartbeatWatchdog;
        private int _smartAgentId;
        private int _smartAgentPriority = 0;

        private List<SmartAgent> _smartAgents;


        public GatewayHubHandler(IDevice device)
        {
            Machines = new List<Machine>();

            InputMonitorings = new List<InputMonitoring>();
            Device = device;
            
            LoadSmartAgentConfiguration();
        }



        public void LoadSmartAgentConfiguration()
        {

            _smartAgents = DataAccess.SmartAgents();
            EstablishHubConnection();

            foreach (Machine c in Machines)
            {
                c.StopMachineDataGeneration();
            }

            _smartAgentId = DataAccess.GetSmartAgentId();

            Machines.Clear();

            var machineConfigurations = DataAccess.GetMachineConfigurations();
            foreach (MachineConfiguration config in machineConfigurations)
            {
                Machines.Add(new Machine(Device, this, config));
            }

            foreach (InputMonitoring m in InputMonitorings)
            {
                m.StopInputMonitoring();
            }

            InputMonitorings.Clear();

            var inputMonitoringConfigurations = DataAccess.GetInputMonitoringConfigurations();
            foreach (InputMonitoringConfiguration config in inputMonitoringConfigurations)
            {
                InputMonitorings.Add(new InputMonitoring(Device, config));
            }
        }




     
        private async Task EstablishHubConnection()
        {
                _gatewayHub?.DisposeAsync();

                string url;
                if (_smartAgents.Any())
                {
                    url = String.Format(@"http://{0}:59162/GatewayHub", _smartAgents[_smartAgentPriority].IpAddress);
                }

                else
                {
                    url = String.Format(@"http://localhost:59162/GatewayHub");
                }

                _gatewayHub = new HubConnectionBuilder().WithUrl(url).Build();

                //_gatewayHub.On<string>("TestSmartAgentConnection", p => TestSmartAgentConnection(p));
                //_gatewayHub.On<bool>("GetAllSmartAgentConnections", p => GetAllSmartAgentConnections(p));
                //_hub.On<bool>("Heartbeat", p => Heartbeat(p));



                await _gatewayHub.StartAsync().ContinueWith(async task =>
               {
                   if (task.IsFaulted || task.IsCanceled)
                   {
                       await TryNextGatewayHub();
                   }
                   else
                   {
                       bool isCurrentGateway = await _gatewayHub.InvokeAsync<bool>("CheckIfIsCurrentGateway");

                       if (!isCurrentGateway)
                       {
                           await TryNextGatewayHub();
                       }
                       else
                       {
                           _gatewayHub.Closed -= ReEstablishHubConnection;
                           _gatewayHub.Closed += ReEstablishHubConnection;
                       }                    
                   }
               });
        }



        private async Task ReEstablishHubConnection(Exception ex)
        {
            await EstablishHubConnection();
        }

        private async Task TryNextGatewayHub()
        {
            ++_smartAgentPriority;

            if (_smartAgentPriority >= _smartAgents.Count)
            {
                _smartAgentPriority = 0;
            }
           await EstablishHubConnection();
        }



        private void GetAllSmartAgentConnections(bool p)
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostInfo = Dns.GetHostEntry(hostName);

            foreach (IPAddress ipOfTheSmartAgent in hostInfo.AddressList)
            {
                _gatewayHub?.InvokeAsync("ReturnSmartAgentConnection", hostName, ipOfTheSmartAgent.ToString());
            }
        }

        private void TestSmartAgentConnection(string p)
        {
            _gatewayHub?.InvokeAsync("TestSmartAgentConnectionResponse", true);
        }


        public void InitializeNewMachineConfigurations(List<MachineConfiguration> machinesConfigurations)
        {
            DataAccess.StoreMachinesConfigurations(machinesConfigurations);
        }


        public void InitializeNewInputMonitoringConfigurations(List<InputMonitoringConfiguration> inputMonitoringConfigurations)
        {
            DataAccess.StoreInputMonitoringConfigurations(inputMonitoringConfigurations);
            LoadSmartAgentConfiguration();
        }



        public void NewHistoryDataNotification()
        {
            _gatewayHub?.InvokeAsync("NewHistoryDataNotification", _smartAgentId);
        }

        public void PublishActualMachineData(Machine machine)
        {
            ActualMachineData actualMachineData = new ActualMachineData
            {
                MachineState = machine.CurrentMachineState.ToString(),
                DailyCycleCounter = machine.DailyCycleCounter,
                LastCycleTime = machine.LastCycleTime,
                CyclesInThisPeriod = machine.CyclesInThisPeriod,
                MachineId = machine.MachineConfiguration.MachineId,
                StateDuration = machine.StateDuration.ToString(@"dd\.hh\:mm\:ss")
            };

            _gatewayHub?.InvokeAsync("PublishActualMachineData", actualMachineData);
        }
    }
}
