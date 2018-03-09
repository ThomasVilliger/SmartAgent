
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SmartDataHub.Models;
using System.Timers;

namespace SmartDataHub
{
    // the client to communicate with the signalR gateway Server
    public static class SignalRgatewayClient
    {
        private static HubConnection _gatewayHub;
        private static Timer _heartbeatTimer = new Timer(3000);
        private static List<SmartAgent> _smartAgents;
        private static int _smartAgentPriority = 0;

        public static async Task Initialize()
        {
            _heartbeatTimer.Elapsed -= Heartbeat;
            _heartbeatTimer.Elapsed += Heartbeat;
            _heartbeatTimer.AutoReset = true;
            _heartbeatTimer.Start();

            await EstablishGatewayHubConnection(null);
        }

        private static void Heartbeat(object sender, ElapsedEventArgs e)
        {
            _gatewayHub?.InvokeAsync("Heartbeat");
        }

        private static async Task EstablishGatewayHubConnection(Exception ex)
        {
            _smartAgents = DataAccess.GetSmartAgents();
            _gatewayHub?.DisposeAsync();

            string url;
            if (_smartAgents.Any() && !(_smartAgentPriority >= _smartAgents.Count))

            {
                url = String.Format(@"http://{0}:59162/GatewayHub", _smartAgents[_smartAgentPriority].IpAddress);
            }

            else
            {
                url = String.Format(@"http://localhost:59162/GatewayHub");
            }

            _gatewayHub = new HubConnectionBuilder().WithUrl(url).Build();

            _gatewayHub.On<ActualMachineData>("PublishActualMachineData", actualMachineData => PublishActualMachineData(actualMachineData));

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

            if (_smartAgentPriority >= _smartAgents.Count + 1)
            {
                _smartAgentPriority = 0;
            }
            await EstablishGatewayHubConnection(null);
        }

        private static async Task NewHistoryDataNotification(int smartAgentId)
        {
            await DataAccess.GetNewHistoryDataFromSmartAgent(smartAgentId);
        }

        public static void PublishActualMachineData(ActualMachineData actualMachineData)
        {
            SmartDataSignalRhub.SmartDataHubClients?.All.InvokeAsync("PublishActualMachineData", actualMachineData);
        }
    }
}