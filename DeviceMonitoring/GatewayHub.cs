
using Microsoft.AspNetCore.SignalR;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

using System.Timers;

namespace DeviceMonitoring
{
    public  class GatewayHub : Hub
    {
        private static IHubClients _clients;
        private static Timer _heartbeatTimer = new Timer(3000);
        //private static Timer _testConnectionTimer = new Timer(5000);


        public GatewayHub()
        {
           
            //_testConnectionTimer.Elapsed -= noConnectionTestResponse;
            //_testConnectionTimer.Elapsed += noConnectionTestResponse;
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

        public async Task PublishActualCycleMachineData(Dictionary<string, string> actualCycleMachineData)

        {
            await Clients.All.InvokeAsync("PublishActualCycleMachineData", actualCycleMachineData);
        }





        public async Task InitializeNewMachineConfigurations(List<Dictionary<string, string>> cycleMachinesConfigurations)

        {
        await Clients.All.InvokeAsync("InitializeNewMachineConfigurations", cycleMachinesConfigurations);
        }



        public  async Task TestSmartAgentConnection(string ipAddress)
        {

            //_testConnectionTimer.Start();
            await Clients.All.InvokeAsync("TestSmartAgentConnection", ipAddress);
        }

        //private void noConnectionTestResponse(object sender, ElapsedEventArgs e)
        //{
        //    TestSmartAgentConnectionResponse(false);
        //}

        public async Task TestSmartAgentConnectionResponse(bool success)

        {
            //_testConnectionTimer.Stop();
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
            _clients = this.Clients;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            _clients = this.Clients;
            return base.OnDisconnectedAsync(ex);
        }
    }
}
