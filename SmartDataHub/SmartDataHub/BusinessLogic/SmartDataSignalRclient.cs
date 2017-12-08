
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SmartDataHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Timers;

namespace SmartDataHub
{
    public static class SmartDataSignalRclient
    {

        private static List<Dictionary<string, string>> _cycleMachinesConfigurations = new List<Dictionary<string, string>>();
        
      
        private static HubConnection _gatewayHub;
        private static Timer _heartbeatWatchdog = new Timer(9000);
        private static Timer _testSmartAgentConnectionTimer = new Timer(3000);
        private static Timer _broadcastAllSmargAgentsTimer = new Timer(3000);

        public static DbSet<CycleMachineConfiguration> _cycleMachineConfigurationContext {


                set

            {
                _cycleMachinesConfigurations.Clear();
                foreach (CycleMachineConfiguration c in value)
                {
                    _cycleMachinesConfigurations.Add(new Dictionary<string, string>
                {
                    {"MachineName", c.MachineName },
                    {"MachineId", c.MachineId.ToString() },
                    {"CycleInputPin", c.CycleInputPin.ToString() },
                    {"MachineStateTimeOut", c.MachineStateTimeOut.ToString() },
                    {"PublishingIntervall", c.PublishingIntervall.ToString() }
                }
                        );
                }
            }

           
         }




        public static void Initialize()
        {

            _broadcastAllSmargAgentsTimer.Elapsed += NoSmartAgentsFoundInNetwork;
            _testSmartAgentConnectionTimer.Elapsed += NoConnectionTestResponse;
            _heartbeatWatchdog.Elapsed += NoConnectionToGatewayHub;
            _heartbeatWatchdog.Start();

            EstablishGatewayHubConnection();

        }

        private static void NoSmartAgentsFoundInNetwork(object sender, ElapsedEventArgs e)
        {
            ReturnSmartAgentConnection(null);
        }

        private static void NoConnectionTestResponse(object sender, ElapsedEventArgs e)
        {
            TestSmartAgentConnectionResponse(false);
        }




        private static void NoConnectionToGatewayHub(object sender, ElapsedEventArgs e)
        {
          
            
                SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("NoConnectionToGatewayHub");
            
            EstablishGatewayHubConnection();
            ResetHeartbeatWatchdogTimer();
        }

        private static async Task EstablishGatewayHubConnection()
        {
            string url = @"http://192.168.0.13:59162/GatewayHub";
            _gatewayHub = new HubConnectionBuilder().WithUrl(url).Build();

            _gatewayHub.On<Dictionary<string, string>>("PublishActualCycleMachineData", actualCycleMachineData => PublishActualCycleMachineData(actualCycleMachineData));

            _gatewayHub.On<bool>("Heartbeat", p => GatewayHubHeartbeat(p));
            _gatewayHub.On<bool>("TestSmartAgentConnectionResponse", connectionSuccess => TestSmartAgentConnectionResponse(connectionSuccess));
            _gatewayHub.On<List<string>>("ReturnSmartAgentConnection", connectionAttributes => ReturnSmartAgentConnection(connectionAttributes));
            await _gatewayHub.StartAsync().ContinueWith(OnConnectionError, TaskContinuationOptions.OnlyOnFaulted);

            //_gatewayHub.InvokeAsync("TestSmartAgentConnection", "I'm_a_IP_You_know?");

        }

        private static void ReturnSmartAgentConnection(List<string> connectionAttributes)
        {

            _broadcastAllSmargAgentsTimer.Stop();

            string hostName = string.Empty;
            string ipAddress = string.Empty;
            bool success;

            if (connectionAttributes != null)
            {
                 hostName = connectionAttributes?[0];
                 ipAddress = connectionAttributes?[1];
                success = true;
            }
            else
            {
                success = false;
            }
           

          
            SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("ReturnSmartAgentConnection", hostName, ipAddress, success);

        }

        private static void GatewayHubHeartbeat(bool p)
        {
            SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("Heartbeat");
            ResetHeartbeatWatchdogTimer();
        }


        private static void ResetHeartbeatWatchdogTimer()
        {
            try
            {
                _heartbeatWatchdog.Stop();
                _heartbeatWatchdog.Start();
            }

            catch(Exception)
            {

            }
        }

        private static void OnConnectionError(Task arg1, object arg2)
        {
            //throw new NotImplementedException();
        }

        private static void TestSmartAgentConnectionResponse(bool connectionSuccess)
        {

            _testSmartAgentConnectionTimer.Stop();
            SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("TestSmartAgentConnectionResponse", connectionSuccess);
        }

        public static void PublishActualCycleMachineData(Dictionary<string, string> actualCycleMachineData)
        {
            ActualCycleMachineData machineData = new ActualCycleMachineData
            {
                MachineState = actualCycleMachineData.FirstOrDefault(m => m.Key == "machineState").Value,
                DailyCycleCounter = actualCycleMachineData.FirstOrDefault(m => m.Key == "dailyCycleCounter").Value,
                CycleTime = actualCycleMachineData.FirstOrDefault(m => m.Key == "cycleTime").Value,
                CycleCounterPerMachineState = actualCycleMachineData.FirstOrDefault(m => m.Key == "cycleCounterPerMachineState").Value,
                StateDuration = actualCycleMachineData.FirstOrDefault(m => m.Key == "stateDuration").Value
            };

            int machineId = Convert.ToInt32(actualCycleMachineData.FirstOrDefault(m => m.Key == "machineId").Value);

            //int machineId = 999;
            //var machineData = new Dictionary<string, string>
            //{
            //    { "MachineState", "Running" },
            //    { "DailyCycleCounter", "666" },
            //    { "StateDuration", "17h 55min" },
            //     { "CycleTime", "7.83s" },


        //};

            try
            {
                SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("PublishActualCycleMachineData", machineId, machineData);




            }

            catch (Exception)
            {
              
             
            }
        }





        public static void  TestSmartAgentConnection(string ipAddress)
        {

            _testSmartAgentConnectionTimer.Start();
            _gatewayHub.InvokeAsync("TestSmartAgentConnection", ipAddress);
        }


        //public static void InizializeNewConfiguration(string ipAddress)
        //{

        //    _gatewayHub.InvokeAsync("TestSmartAgentConnection", ipAddress);
        //}



        public static async Task GetAllSmartAgentConnections()
        {
            _broadcastAllSmargAgentsTimer.Start();
            await  _gatewayHub.InvokeAsync("GetAllSmartAgentConnections");
        }



        public static void InitializeNewMachineConfigurations()
        {


            if (_gatewayHub != null)
            {
                _gatewayHub.InvokeAsync("InitializeNewMachineConfigurations", _cycleMachinesConfigurations);
            }



        }


    }
}