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
        private Dictionary<int, string> _gatewayHubsIPs;
        private int _gatewayHubPriority = 1;


        public GatewayHubHandler(IDevice device)
        {
            Machines = new List<Machine>();

            InputMonitorings = new List<InputMonitoring>();
            Device = device;
            EstablishHubConnection();

            LoadSmartAgentConfiguration();
        }



        public void LoadSmartAgentConfiguration()
        {
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


        private async Task ReEstablishHubConnection(Exception ex)
        {
            _gatewayHub.DisposeAsync();
            EstablishHubConnection();
            //ResetHeartbeatWatchdogTimer();
        }


        private async Task EstablishHubConnection()
        {


            string url = String.Format(@"http://{0}:59162/GatewayHub", _gatewayHubsIPs.GetValueOrDefault(_gatewayHubPriority)) ;
            _gatewayHub = new HubConnectionBuilder().WithUrl(url).Build();

            //_gatewayHub.On<string>("TestSmartAgentConnection", p => TestSmartAgentConnection(p));
            //_gatewayHub.On<bool>("GetAllSmartAgentConnections", p => GetAllSmartAgentConnections(p));
            //_hub.On<bool>("Heartbeat", p => Heartbeat(p));

            _gatewayHub.Closed -= ReEstablishHubConnection;
            _gatewayHub.Closed += ReEstablishHubConnection;

            await _gatewayHub.StartAsync().ContinueWith(async task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    ++_gatewayHubPriority;                
                    ReEstablishHubConnection(null);
                }
                else
                {
                 bool  isCurrentGateway =  await   _gatewayHub.InvokeAsync<bool>("CheckIfIsCurrentGateway");

                    if(!isCurrentGateway)
                    {
                        ++_gatewayHubPriority;
                        ReEstablishHubConnection(null);
                    }             
                }

                if (_gatewayHubPriority > _gatewayHubsIPs.Count)
                {
                    _gatewayHubPriority = 1;
                }
            });
        }


        private void GetAllSmartAgentConnections(bool p)
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostInfo = Dns.GetHostEntry(hostName);

            foreach (IPAddress ipOfTheSmartAgent in hostInfo.AddressList)
            {
                _gatewayHub.InvokeAsync("ReturnSmartAgentConnection", hostName, ipOfTheSmartAgent.ToString());
            }
        }

        private void TestSmartAgentConnection(string p)
        {
            _gatewayHub.InvokeAsync("TestSmartAgentConnectionResponse", true);
        }


        public void InitializeNewMachineConfigurations(List<MachineConfiguration> machinesConfigurations)
        {
            DataAccess.MachinesConfigurations(machinesConfigurations);
        }


        public void InitializeNewInputMonitoringConfigurations(List<InputMonitoringConfiguration> inputMonitoringConfigurations)
        {
            DataAccess.StoreInputMonitoringConfigurations(inputMonitoringConfigurations);
            LoadSmartAgentConfiguration();
        }



        public void NewHistoryDataNotification()
        {
            _gatewayHub.InvokeAsync("NewHistoryDataNotification", _smartAgentId);
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

            _gatewayHub.InvokeAsync("PublishActualMachineData", actualMachineData);
        }
    }
}
