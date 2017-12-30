
using Microsoft.AspNetCore.SignalR;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

using System.Timers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace DeviceMonitoring
{
    public  class GatewayHub : Hub
    {
        private static IHubClients _clients;
        private static Timer _heartbeatTimer = new Timer(3000);
        private static int _connectionCounter;



        public GatewayHub()
        {
            _heartbeatTimer.Elapsed -= Hearthbeat;
            _heartbeatTimer.Elapsed += Hearthbeat;
            _heartbeatTimer.AutoReset = true;
            _heartbeatTimer.Start();
        }

        private static void Hearthbeat(Object source, ElapsedEventArgs e)
        {
            if (_clients != null)
            {
                _clients.All.InvokeAsync("Heartbeat", true);
            }
        }

        public async Task PublishActualMachineData(object actualMachineData)

        {
            await Clients.All.InvokeAsync("PublishActualMachineData", actualMachineData);
        }

        public async Task NewHistoryDataNotification(int smartAgentId)

        {
            await Clients.All.InvokeAsync("NewHistoryDataNotification", smartAgentId);
        }



        public async Task InitializeNewMachineConfigurations(List<Dictionary<string, string>> machinesConfigurations, string ipAddressOfSmartAgent)

        {
        string connectionId = ConnectionHandler.SmartAgentIpAdressesAndTheirConnections.GetValueOrDefault(ipAddressOfSmartAgent);
        await Clients.Client(connectionId).InvokeAsync("InitializeNewMachineConfigurations", machinesConfigurations);
        }



        public  async Task TestSmartAgentConnection(string ipAddressOfSmartAgent)
        {
            string connectionId = ConnectionHandler.SmartAgentIpAdressesAndTheirConnections.GetValueOrDefault("::1");
            await Clients.Client(connectionId).InvokeAsync("TestSmartAgentConnection", "bla");

            //await Clients.All.InvokeAsync("TestSmartAgentConnection", "bb");
        }


        public async Task TestSmartAgentConnectionResponse(bool success)

        {
            await _clients.All.InvokeAsync("TestSmartAgentConnectionResponse", success);
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

            string clientIpAddress = Context.Connection.GetHttpContext().Connection.RemoteIpAddress.ToString();
            string port = Context.Connection.GetHttpContext().Connection.RemotePort.ToString();
            string clientConnectionId = Context.ConnectionId;

            //Context.Connection.GetHttpContext().Request.HttpContext.Session.

            //       var ip = 

            

            ConnectionHandler.SmartAgentIpAdressesAndTheirConnections.Remove(clientIpAddress);
            ConnectionHandler.SmartAgentIpAdressesAndTheirConnections.Add(clientIpAddress, clientConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            --_connectionCounter;

            Console.WriteLine("current Connections on GatewayHub: " + _connectionCounter);

            _clients = this.Clients;
            ConnectionHandler.SmartAgentIpAdressesAndTheirConnections.Remove(Context.Connection.GetHttpContext().Connection.RemoteIpAddress.ToString());
            return base.OnDisconnectedAsync(ex);
        }






        public static class ConnectionHandler
        {
            public static Dictionary<string, string> SmartAgentIpAdressesAndTheirConnections = new Dictionary<string, string>();
        }
    }
}
