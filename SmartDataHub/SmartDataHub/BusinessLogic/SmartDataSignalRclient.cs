
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

        private static List<Dictionary<string, string>> _machines = new List<Dictionary<string, string>>();
        
      
        private static HubConnection _gatewayHub;
        private static  Timer _heartbeatWatchdog = new Timer(9000);
        //private static Timer _testSmartAgentConnectionTimer = new Timer(3000);
        private static Timer _broadcastAllSmargAgentsTimer = new Timer(3000);

        //public static DbSet<CycleMachineConfiguration> _cycleMachineConfigurationContext {


        //        set

        //    {
        //        _cycleMachinesConfigurations.Clear();
        //        foreach (CycleMachineConfiguration c in value)
        //        {
        //            _cycleMachinesConfigurations.Add(new Dictionary<string, string>
        //        {
        //            {"MachineName", c.MachineName },
        //            {"MachineId", c.CycleMachineConfigurationId.ToString() },
        //            {"CycleInputPin", c.CycleInputPin.ToString() },
        //            {"MachineStateTimeOut", c.MachineStateTimeOut.ToString() },
        //            {"PublishingIntervall", c.PublishingIntervall.ToString() }
        //        }
        //                );
        //        }
        //    }

           
        // }




        public static void Initialize()
        {

            _broadcastAllSmargAgentsTimer.Elapsed += NoSmartAgentsFoundInNetwork;
            //_testSmartAgentConnectionTimer.Elapsed += NoConnectionTestResponse;
            _heartbeatWatchdog.Elapsed += NoConnectionToGatewayHub;
            _heartbeatWatchdog.Start();

            EstablishGatewayHubConnection();

        }

        private static void NoSmartAgentsFoundInNetwork(object sender, ElapsedEventArgs e)
        {
            ReturnSmartAgentConnection(null);
        }

        //private static void NoConnectionTestResponse(object sender, ElapsedEventArgs e)
        //{
        //    //TestSmartAgentConnectionResponse(false);
        //}




        private static void NoConnectionToGatewayHub(object sender, ElapsedEventArgs e)
        {
          
            
                SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("NoConnectionToGatewayHub");

            _gatewayHub.DisposeAsync();
            
            EstablishGatewayHubConnection();
            ResetHeartbeatWatchdogTimer();
        }

        private static async Task EstablishGatewayHubConnection()
        {
            string url = @"http://192.168.0.13:59162/GatewayHub";
            _gatewayHub = new HubConnectionBuilder().WithUrl(url).Build();

            _gatewayHub.On<ActualMachineData>("PublishActualMachineData", actualMachineData => PublishActualMachineData(actualMachineData));

            _gatewayHub.On<bool>("Heartbeat", p => GatewayHubHeartbeat(p));
            //_gatewayHub.On<bool>("TestSmartAgentConnectionResponse", connectionSuccess => TestSmartAgentConnectionResponse(connectionSuccess));
            _gatewayHub.On<List<string>>("ReturnSmartAgentConnection", connectionAttributes => ReturnSmartAgentConnection(connectionAttributes));
            
                _gatewayHub.On<int>("NewHistoryDataNotification", smartAgentId => RequestNewHistoryData(smartAgentId));
            await _gatewayHub.StartAsync().ContinueWith(OnConnectionError, TaskContinuationOptions.OnlyOnFaulted);

            //_gatewayHub.InvokeAsync("TestSmartAgentConnection", "I'm_a_IP_You_know?");

        }

        private static void RequestNewHistoryData(int smartAgentId)
        {
            DataAccess.GetNewHistoryDataFromSmartAgent(smartAgentId);
        }

        private static void ReturnSmartAgentConnection(List<string> connectionAttributes)
        {

            _broadcastAllSmargAgentsTimer.Stop();

            string hostName = string.Empty;
            string ipAddress = string.Empty;
            bool success;
            string message;

            if (connectionAttributes != null)
            {
                 hostName = connectionAttributes?[0];
                 ipAddress = connectionAttributes?[1];
                 success = true;

                 message = String.Format("{0}: {1}", hostName, ipAddress);
            }
            else
            {
                success = false;
                message = "no SmartAgents found in network";
            }
           


            SmartDataSignalRhub.SmartAgentConfigurationResponse(success, message, null, false);

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

        //private static void TestSmartAgentConnectionResponse(bool success)
        //{

        //    _testSmartAgentConnectionTimer.Stop();
        //    var message = "TODO";
        //    SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("Response", success, message);
        //}

        public static void PublishActualMachineData(ActualMachineData actualMachineData)
        {
            //ActualMachineData machineData = new ActualMachineData
            //{
            //    MachineState = actualMachineData.FirstOrDefault(m => m.Key == "machineState").Value,
            //    DailyCycleCounter = actualMachineData.FirstOrDefault(m => m.Key == "dailyCycleCounter").Value,
            //    CycleTime = actualMachineData.FirstOrDefault(m => m.Key == "cycleTime").Value,
            //    CyclesInThisPeriod = actualMachineData.FirstOrDefault(m => m.Key == "cyclesInThisPeriod").Value,
            //    StateDuration = actualMachineData.FirstOrDefault(m => m.Key == "stateDuration").Value
            //};

            //int machineId = Convert.ToInt32(actualMachineData.FirstOrDefault(m => m.Key == "machineId").Value);


         
                SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("PublishActualMachineData", actualMachineData);
        

    
        }





        //public static void  TestSmartAgentConnection(string ipAddress)
        //{

        //    _testSmartAgentConnectionTimer.Start();
        //    _gatewayHub.InvokeAsync("TestSmartAgentConnection", ipAddress);
        //}


        //public static void InizializeNewConfiguration(string ipAddress)
        //{

        //    _gatewayHub.InvokeAsync("TestSmartAgentConnection", ipAddress);
        //}



        public static async Task GetAllSmartAgentConnections()
        {
            _broadcastAllSmargAgentsTimer.Start();
            await  _gatewayHub.InvokeAsync("GetAllSmartAgentConnections");
        }



    }
}