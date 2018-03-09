using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Timers;

namespace DeviceMonitoring
{
    // the signalR Hub for the gateway communication
    public class GatewayHub : Hub
    {
        private static IHubClients _clients;
        private static Timer _heartbeatWatchdog = new Timer(10000);
        private static int _connectionCounter;
        private static bool _isCurrentGateway = true;

        public GatewayHub()
        {
            _heartbeatWatchdog.Elapsed -= NoHeartbeat;
            _heartbeatWatchdog.Elapsed += NoHeartbeat;
            _heartbeatWatchdog.Start();
        }

        private static void NoHeartbeat(Object source, ElapsedEventArgs e)
        {
            _isCurrentGateway = true;
        }

        public async Task PublishActualMachineData(object actualMachineData)

        {
            await Clients.All.InvokeAsync("PublishActualMachineData", actualMachineData);
        }

        public async Task NewHistoryDataNotification(int smartAgentId)

        {
            await Clients.All.InvokeAsync("NewHistoryDataNotification", smartAgentId);
        }

        public async Task<bool> CheckIfIsCurrentGateway()
        {
            return _isCurrentGateway;
        }

        public void Heartbeat()
        {
            _heartbeatWatchdog.Stop();
            _heartbeatWatchdog.Start();
            _isCurrentGateway = true;
        }
        public async Task GetAllSmartAgentConnections()

        {
            await Clients.All.InvokeAsync("GetAllSmartAgentConnections", true);
        }


        public async Task ReturnSmartAgentConnection(string hostName, string ipAddress)

        {
            await Clients.All.InvokeAsync("ReturnSmartAgentConnection", new List<string>() { hostName, ipAddress });
        }

        public override Task OnConnectedAsync()
        {
            ++_connectionCounter;
            Console.WriteLine("current Connections on GatewayHub: " + _connectionCounter);

            _clients = this.Clients;

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            --_connectionCounter;

            Console.WriteLine("current Connections on GatewayHub: " + _connectionCounter);

            _clients = this.Clients;
            return base.OnDisconnectedAsync(ex);
        }
    }
}
