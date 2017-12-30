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
        public  static IHubClients  SmartDataHubClients;


        public async Task TestSmartAgentConnection(string ip)
        {
            try
            {
                string url = String.Format(@"http://{0}:8800/api/testSmartAgentConnection", ip);
                var response = await client.GetAsync(url);
                var responseMessage = await response.Content.ReadAsStringAsync();
                var success = response.IsSuccessStatusCode;

                if (success)
                {
                    var message = String.Format("Connection to {0} successfull", ip);
                    SmartAgentConfigurationResponse(true, message, null, true);
                }
                else
                {         
                      var  message =  responseMessage;
                      var defaultMessage = "connection test failed";
                      SmartAgentConfigurationResponse(false, message, defaultMessage, false);
                }
            }

            catch (Exception ex)
            {
                SmartAgentConfigurationResponse(false, ex.Message, null, false);
            }
        }


        public async void InitializeSmartAgentConfigurations(string ip, int smartAgentId)
        {
            try
            {
                await InitializeMachinesOnSmartAgent(ip, smartAgentId);
                await InitializeInputMonitoringsOnSmartAgent(ip, smartAgentId);
                await SignSmartAgent(ip, smartAgentId);
            }

            catch (Exception ex)
            {
                SmartAgentConfigurationResponse(false, "Initialization failed", null, false);
                SmartAgentConfigurationResponse(false, ex.Message, null, false);
            }
        }



        private async  Task InitializeMachinesOnSmartAgent(string ip, int smartAgentId)
        {
            var values = DataAccess.GetMachines(smartAgentId);
            //var content = new FormUrlEncodedContent(values);
            var jsonContent = JsonConvert.SerializeObject(values);
            var stringContent = new StringContent(jsonContent.ToString());

            //stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            string url = String.Format(@"http://{0}:8800/api/initializeNewMachines", ip);
            var response = await client.PutAsync(url, stringContent);
            var responseMessage = await response.Content.ReadAsStringAsync(); //right!
            var success = response.IsSuccessStatusCode;



            if (success)
            {
                var message = String.Format("Machine configs  successfully written to {0}", ip);
                var configuredMachines = Environment.NewLine + JsonConvert.SerializeObject(responseMessage);
                var defaultMessage = "no machines configured for this SmartAgent";

                SmartAgentConfigurationResponse(true, message, null, true);
                SmartAgentConfigurationResponse(true, configuredMachines, defaultMessage, false);
            }

           else
           {
                var message = responseMessage;
                var defaultMessage = "Failed to write down machine configurations on SmartAgent";
                SmartAgentConfigurationResponse(false, message, defaultMessage, false);
            }
        }

        private async Task InitializeInputMonitoringsOnSmartAgent(string ip, int smartAgentId)
        {
            var values = DataAccess.GetInputMonitorings(smartAgentId);
            var jsonContent = JsonConvert.SerializeObject(values);
            var stringContent = new StringContent(jsonContent.ToString());

            string url = String.Format(@"http://{0}:8800/api/initializeNewInputMonitorings", ip);
            var response = await client.PutAsync(url, stringContent);
            var responseMessage = await response.Content.ReadAsStringAsync(); //right!
            var success = response.IsSuccessStatusCode;

            if (success)
            {
                var message = String.Format("Input monitoring configs  successfully written to {0}", ip);
                var configuredInputMonitorings = Environment.NewLine + JsonConvert.SerializeObject(responseMessage);
                var defaultMessage = "no input Monitorings configured for this SmartAgent";

                SmartAgentConfigurationResponse(true, message, null, true);
                SmartAgentConfigurationResponse(true, configuredInputMonitorings, defaultMessage, false);
            }

            else
           {
                var message = responseMessage;
                var defaultMessage = "Failed to write down inputMonitoring configurations on SmartAgent";
                SmartAgentConfigurationResponse(false, message, defaultMessage, false);
            }
        }

        private async Task SignSmartAgent(string ip, int smartAgentId)
        {
            var values = smartAgentId;
            var jsonContent = JsonConvert.SerializeObject(values);
            var stringContent = new StringContent(jsonContent.ToString());

            string url = String.Format(@"http://{0}:8800/api/signSmartAgentAndLoadConfiguration", ip);
            var response = await client.PutAsync(url, stringContent);
            var responseMessage = await response.Content.ReadAsStringAsync(); //right!
            var success = response.IsSuccessStatusCode;

            if (success)
            {
                var message = String.Format("SmartAgent successfully signed with Id {0} and running with new configuration", smartAgentId);
                SmartAgentConfigurationResponse(true, message, null, true);
            }
            else
            {
                var message = responseMessage;
                var defaultMessage = "Failed to sign SmartAgent";

                SmartAgentConfigurationResponse(false, message, defaultMessage, false);
            }
        }


        public static void SmartAgentConfigurationResponse(bool isSuccess, string message, string defaultMessage, bool isGreenColored)
        {
            string responseMessage;

            if (message != String.Empty)
            {
                responseMessage = message;
            }
            else
            {
                responseMessage = defaultMessage;
            }

            SmartDataHubClients.All.InvokeAsync("SmartAgentConfigurationResponse", isSuccess, responseMessage, isGreenColored);
        }

        public async Task GetAllSmartAgentConnections()
        {
            SmartDataSignalRclient.GetAllSmartAgentConnections();
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

