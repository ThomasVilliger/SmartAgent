
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
        private static Timer _heartbeatWatchdog = new Timer(10000);
        private static int _connectionCounter;
        private static bool isCurrentGateway = true;




        public GatewayHub()
        {
            _heartbeatWatchdog.Elapsed -= NoHeartbeat;
            _heartbeatWatchdog.Elapsed += NoHeartbeat;
            //_heartbeatWatchdog.AutoReset = true;
            _heartbeatWatchdog.Start();
        }

        private static void NoHeartbeat(Object source, ElapsedEventArgs e)
        {
            isCurrentGateway = true;
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
            return isCurrentGateway;
        }

        public void Heartbeat ()
        {
            _heartbeatWatchdog.Stop();
            _heartbeatWatchdog.Start();
            isCurrentGateway = true;
        }



        //public async Task InitializeNewMachineConfigurations(List<Dictionary<string, string>> machinesConfigurations, string ipAddressOfSmartAgent)

        //{
        //string connectionId = ConnectionHandler.SmartAgentIpAdressesAndTheirConnections.GetValueOrDefault(ipAddressOfSmartAgent);
        //await Clients.Client(connectionId).InvokeAsync("InitializeNewMachineConfigurations", machinesConfigurations);
        //}



        //public  async Task TestSmartAgentConnection(string ipAddressOfSmartAgent)
        //{
        //    string connectionId = ConnectionHandler.SmartAgentIpAdressesAndTheirConnections.GetValueOrDefault("::1");
        //    await Clients.Client(connectionId).InvokeAsync("TestSmartAgentConnection", "bla");

        //    //await Clients.All.InvokeAsync("TestSmartAgentConnection", "bb");
        //}


        //public async Task TestSmartAgentConnectionResponse(bool success)

        //{
        //    await _clients.All.InvokeAsync("TestSmartAgentConnectionResponse", success);
        //}

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
