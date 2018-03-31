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
        private static bool _isCurrentGateway = false;
        private static int _connectedSmartAgentCounter;

        public GatewayHub()
        {
            _heartbeatWatchdog.Elapsed -= NoHeartbeat;
            _heartbeatWatchdog.Elapsed += NoHeartbeat;
            _heartbeatWatchdog.Start();
        }

        private static void NoHeartbeat(Object source, ElapsedEventArgs e)
        {
            _isCurrentGateway = false;
            _clients.All.InvokeAsync("ServerOrderedDisconnect");
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
            IncreaseDeviceCounter();
            return _isCurrentGateway;
        }

        public void Heartbeat()
        {
            _heartbeatWatchdog.Stop();
            _heartbeatWatchdog.Start();
            _isCurrentGateway = true;
            _clients.All.InvokeAsync("IsCurrentGateway", _isCurrentGateway);
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
            _clients = this.Clients;

            try
            {
                _clients.All.InvokeAsync("UpdateDeviceCounter", GatewayDeviceCounter.ConnectedSmartAgents.Count);
                _clients.All.InvokeAsync("IsCurrentGateway", _isCurrentGateway);
            }
            catch (Exception ex)
            {
                this.Context.Connection.Abort();
        }
 
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            _clients = this.Clients;
            DecreaseDeviceCounter();
            return base.OnDisconnectedAsync(ex);
        }

        private void DecreaseDeviceCounter()
        {
            GatewayDeviceCounter.ConnectedSmartAgents.Remove(Context.ConnectionId);
            _clients.All.InvokeAsync("UpdateDeviceCounter", GatewayDeviceCounter.ConnectedSmartAgents.Count);
        }

        private void IncreaseDeviceCounter()
        {
            GatewayDeviceCounter.ConnectedSmartAgents.Add(Context.ConnectionId);
            _clients.All.InvokeAsync("UpdateDeviceCounter", GatewayDeviceCounter.ConnectedSmartAgents.Count);
        }

        public static class GatewayDeviceCounter
        {
            public static HashSet<string> ConnectedSmartAgents = new HashSet<string>();
        }
    }
}
