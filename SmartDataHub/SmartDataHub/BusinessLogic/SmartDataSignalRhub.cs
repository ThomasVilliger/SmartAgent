using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Timers;

namespace SmartDataHub
{
    public class SmartDataSignalRhub : Hub
    {
        private static readonly HttpClient client = new HttpClient();
      
        private  Timer _responseTimer = new Timer(4000);

        public  static IHubClients  SmartDataHubClients;

        public SmartDataSignalRhub()
        {

            _responseTimer.Elapsed -= responseTimeOut;
            _responseTimer.Elapsed += responseTimeOut;
            _responseTimer.AutoReset = false;

        }

        private void responseTimeOut(object sender, ElapsedEventArgs e)
        {
            Clients.All.InvokeAsync("Response",  false, "response TimeOut", false);
        }

        public async Task TestSmartAgentConnection(string ip)
        {
            string url = String.Format(@"http://{0}:8800/api/testSmartAgentConnection", ip);

            _responseTimer.Start();

            var response = await client.GetAsync(url);
            _responseTimer.Stop();
            var responseMessage = await response.Content.ReadAsStringAsync(); 
            var success = response.IsSuccessStatusCode;

            if (success)
            {
                var successMessage = String.Format("Connection to {0} successfull", ip);

                Clients.All.InvokeAsync("Response", success, successMessage, true);
               
            }
            else
            {
                var errorMessage = Environment.NewLine +  JsonConvert.SerializeObject(responseMessage);
                Clients.All.InvokeAsync("Response", success, errorMessage, false);
            }
        }



        public async Task GetAllSmartAgentConnections()
        {
            SmartDataSignalRclient.GetAllSmartAgentConnections();
        }



        public async Task InitializeNewMachineConfigurations(string ip, int smartAgentId)
        {
            var values = DataAccess.GetCycleMachineConfigurations(smartAgentId);
            //var content = new FormUrlEncodedContent(values);

            var jsonContent= JsonConvert.SerializeObject(values);
            var stringContent = new StringContent(jsonContent.ToString());

            //stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
      
            string url = String.Format(@"http://{0}:8800/api/initializeNewMachineConfigurations", ip);

            _responseTimer.Start();
            var response = await client.PutAsync(url, stringContent);
            _responseTimer.Stop();
            var responseMessage = await response.Content.ReadAsStringAsync(); //right!
            var success = response.IsSuccessStatusCode;



            if (success)
            {
              var successMessage = String.Format("SmartAgent {0} successfully configured", ip) ;
              var configuredMachines = Environment.NewLine + JsonConvert.SerializeObject(responseMessage);

              await  Clients.All.InvokeAsync("Response", success, successMessage, true);
                Clients.All.InvokeAsync("Response",  success, configuredMachines, false);
            }
            else
            {
                var errorMessage = Environment.NewLine + responseMessage;
                Clients.All.InvokeAsync("Response", success, errorMessage, false);
            }      
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
