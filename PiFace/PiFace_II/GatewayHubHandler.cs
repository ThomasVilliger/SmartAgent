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
  public  class GatewayHubHandler
    {


        private HubConnection _hub;
        public List<Machine> Machines;
        public List<InputMonitoring> InputMonitorings;
        public IDevice Device;
        private  ThreadPoolTimer _heartbeatWatchdog;
        private int _smartAgentId;


        public GatewayHubHandler(IDevice device)
        {


            _heartbeatWatchdog = ThreadPoolTimer.CreatePeriodicTimer(NoConnectionToGatewayHub,
                                   TimeSpan.FromMilliseconds(10000));

            Machines = new List<Machine>();

            

            InputMonitorings = new List<InputMonitoring>();
            Device = device;
            EstablishHubConnection();


            LoadSmartAgentConfiguration();
        }



        public void LoadSmartAgentConfiguration()
        {

           //var  historyData = DataAccess.GetMachineStateHistoryData(0);

            foreach(Machine c in Machines)
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

        private   void  GetAllSmartAgentConnections(bool p)
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
            _hub.InvokeAsync("NewHistoryDataNotification", _smartAgentId);
        }

        public void PublishActualMachineData(Machine machine)
        {   
            ActualMachineData actualMachineData = new ActualMachineData
            {
                MachineState = machine.CurrentMachineState.ToString(),
                DailyCycleCounter = machine.DailyCycleCounter,
                LastCycleTime  = machine.LastCycleTime,
                CyclesInThisPeriod = machine.CyclesInThisPeriod,
                MachineId = machine.MachineConfiguration.MachineId,
                StateDuration = machine.StateDuration.ToString(@"dd\.hh\:mm\:ss")
            };

            _hub.InvokeAsync("PublishActualMachineData", actualMachineData);
        }
    }
}
