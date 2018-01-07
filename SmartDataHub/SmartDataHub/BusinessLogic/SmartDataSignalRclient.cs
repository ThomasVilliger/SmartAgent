
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
        private static HubConnection _gatewayHub;
        private static Timer _heartbeatTimer = new Timer(3000);
        //private static Timer _heartbeatWatchdog = new Timer(9000);
        //private static Timer _broadcastAllSmargAgentsTimer = new Timer(3000);
        private static List<SmartAgent> SmartAgents;
        private static int _smartAgentPriority = 0;

        public static async Task Initialize()
        {
            //_broadcastAllSmargAgentsTimer.Elapsed += NoSmartAgentsFoundInNetwork;
            //_heartbeatWatchdog.Elapsed += NoConnectionToGatewayHub;
            //_heartbeatWatchdog.Start();

            _heartbeatTimer.Elapsed -= Heartbeat;
            _heartbeatTimer.Elapsed += Heartbeat;
            _heartbeatTimer.AutoReset = true;
            _heartbeatTimer.Start();

            DataAccess.Initialize();

          await  EstablishGatewayHubConnection(null);
        }

        private static void Heartbeat(object sender, ElapsedEventArgs e)
        {
             _gatewayHub?.InvokeAsync("Heartbeat");
        }

        //private static void NoSmartAgentsFoundInNetwork(object sender, ElapsedEventArgs e)
        //{
        //    ReturnSmartAgentConnection(null);
        //}


        //private static async Task NoConnectionToGatewayHub(Exception ex)
        //{
        //    SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("NoConnectionToGatewayHub");

        //    _gatewayHub.DisposeAsync();

        //    EstablishGatewayHubConnection();
        //    //ResetHeartbeatWatchdogTimer();
        //}

        private static async Task EstablishGatewayHubConnection(Exception ex)
        {
            SmartAgents = DataAccess.GetSmartAgents();

            _gatewayHub?.DisposeAsync();

            string url;
            if (SmartAgents.Any())
            {
                url = String.Format(@"http://{0}:59162/GatewayHub", SmartAgents[_smartAgentPriority].IpAddress);
            }

            else
            {
                url = String.Format(@"http://localhost:59162/GatewayHub");
            }



            _gatewayHub = new HubConnectionBuilder().WithUrl(url).Build();

            _gatewayHub.On<ActualMachineData>("PublishActualMachineData", actualMachineData => PublishActualMachineData(actualMachineData));
            //_gatewayHub.On<bool>("Heartbeat", p => GatewayHubHeartbeat(p));
            //_gatewayHub.On<List<string>>("ReturnSmartAgentConnection", connectionAttributes => ReturnSmartAgentConnection(connectionAttributes));
            _gatewayHub.On<int>("NewHistoryDataNotification", smartAgentId => NewHistoryDataNotification(smartAgentId));

            _gatewayHub.Closed -= EstablishGatewayHubConnection;
            _gatewayHub.Closed += EstablishGatewayHubConnection;

            await _gatewayHub.StartAsync().ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("NoConnectionToGatewayHub");
                    TryNextGatewayHub();
                }

                else
                {
                    SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("ConnectionToGatewayHubEstablished");
                }
            });
        }


        private static async void TryNextGatewayHub()
        {
            ++_smartAgentPriority;

            if (_smartAgentPriority >= SmartAgents.Count)
            {
                _smartAgentPriority = 0;
            }
           await EstablishGatewayHubConnection(null);
        }


        private static async Task NewHistoryDataNotification(int smartAgentId)
        {
          await  DataAccess.GetNewHistoryDataFromSmartAgent(smartAgentId);
        }

        //private static void ReturnSmartAgentConnection(List<string> connectionAttributes)
        //{

        //    _broadcastAllSmargAgentsTimer.Stop();

        //    string hostName = string.Empty;
        //    string ipAddress = string.Empty;
        //    bool success;
        //    string message;

        //    if (connectionAttributes != null)
        //    {
        //        hostName = connectionAttributes?[0];
        //        ipAddress = connectionAttributes?[1];
        //        success = true;

        //        message = String.Format("{0}: {1}", hostName, ipAddress);
        //    }
        //    else
        //    {
        //        success = false;
        //        message = "no SmartAgents found in network";
        //    }



        //    SmartDataSignalRhub.SmartAgentConfigurationResponse(success, message, null, false, false);

        //}

        //private static void GatewayHubHeartbeat(bool p)
        //{
        //    SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("Heartbeat");
        //    //ResetHeartbeatWatchdogTimer();
        //}


        //private static void ResetHeartbeatWatchdogTimer()
        //{
        //    try
        //    {
        //        _heartbeatWatchdog.Stop();
        //        _heartbeatWatchdog.Start();
        //    }

        //    catch (Exception)
        //    {

        //    }
        //}

        //private static void OnConnectionError(Task arg1, object arg2)
        //{

        //    NoConnectionToGatewayHub();
        //}


        public static void PublishActualMachineData(ActualMachineData actualMachineData)
        {
            SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("PublishActualMachineData", actualMachineData);
        }


        //public static async Task GetAllSmartAgentConnections()
        //{
        //    _broadcastAllSmargAgentsTimer.Start();
        //   await _gatewayHub.InvokeAsync("GetAllSmartAgentConnections");
        //}
    }
}