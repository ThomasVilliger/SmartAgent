using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SmartDataHub
{
    public class SmartDataSignalRhub : Hub
    {

       

        public  static IHubClients  SmartDataHubClients;




        public void TestSmartAgentConnection(string ipAddress)
        {
            SmartDataSignalRclient.TestSmartAgentConnection(ipAddress);
        }



        public async Task GetAllSmartAgentConnections()
        {
            SmartDataSignalRclient.GetAllSmartAgentConnections();
        }



        public async Task InitializeNewMachineConfigurations()
        {
            SmartDataSignalRclient.InitializeNewMachineConfigurations();
        }


        public override Task OnConnectedAsync()
        {
            SmartDataHubClients = this.Clients;


            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            SmartDataHubClients = this.Clients;
            return base.OnDisconnectedAsync(ex);
        }
    }
}
