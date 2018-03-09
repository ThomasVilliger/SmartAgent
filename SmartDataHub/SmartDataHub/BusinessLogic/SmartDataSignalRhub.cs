using Microsoft.AspNetCore.SignalR;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

namespace SmartDataHub
{
    // the signalR Hub of this website
    public class SmartDataSignalRhub : Hub
    {
        private static readonly HttpClient _client = new HttpClient();
        public static IHubClients SmartDataHubClients;

        public async Task TestSmartAgentConnection(string ip)
        {
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(100);
                CancellationToken ct = cts.Token;

                string url = String.Format(@"http://{0}:8800/api/testSmartAgentConnection", ip);
                var response = await _client.GetAsync(url, ct);
                var responseMessage = await response.Content.ReadAsStringAsync();
                var success = response.IsSuccessStatusCode;

                if (success)
                {
                    var message = String.Format("Connection to SmartAgent {0} successfull", ip);
                    SmartAgentConfigurationResponse(true, message, null, true, false);
                }
                else
                {
                    var message = responseMessage;
                    var defaultMessage = "connection test failed";
                    SmartAgentConfigurationResponse(false, message, defaultMessage, false, false);
                }
            }

            catch (TaskCanceledException ex)
            {
                var message = "connection test failed";
                SmartAgentConfigurationResponse(false, message, null, false, false);
            }
        }

        public async void InitializeSmartAgentConfigurations(string ip, int smartAgentId)
        {
            try
            {
                await InitializeMachinesOnSmartAgent(ip, smartAgentId);
                await InitializeInputMonitoringsOnSmartAgent(ip, smartAgentId);
                await RegisterSmartAgentsOnSmartAgent(ip);
                await SignSmartAgent(ip, smartAgentId);
            }

            catch (Exception ex)
            {
                SmartAgentConfigurationResponse(false, "Initialization failed", null, false, false);
                SmartAgentConfigurationResponse(false, ex.Message, null, false, false);
            }
        }

        private async Task InitializeMachinesOnSmartAgent(string ip, int smartAgentId)
        {
            var values = DataAccess.GetMachines(smartAgentId);

            string url = String.Format(@"http://{0}:8800/api/initializeNewMachines", ip);
            var response = await _client.PutAsync(url, GetHttpStringContent(values));
            var responseMessage = await response.Content.ReadAsStringAsync(); //right!
            var success = response.IsSuccessStatusCode;

            if (success)
            {
                var message = String.Format("Machine configs  successfully written to {0}", ip);
                SmartAgentConfigurationResponse(true, message, null, true, false);

                var defaultMessage = "no machines configured for this SmartAgent";
                ConfigResponse(responseMessage, defaultMessage);
            }

            else
            {
                var message = responseMessage;
                var defaultMessage = "Failed to write down machine configurations on SmartAgent";
                SmartAgentConfigurationResponse(false, message, defaultMessage, false, false);
            }
        }

        private StringContent GetHttpStringContent(object values)
        {
            var jsonContent = JsonConvert.SerializeObject(values);
            return new StringContent(jsonContent.ToString());
        }

        private async Task RegisterSmartAgentsOnSmartAgent(string ip)
        {
            var values = DataAccess.GetSmartAgents();

            string url = String.Format(@"http://{0}:8800/api/registerSmartAgents", ip);
            var response = await _client.PutAsync(url, GetHttpStringContent(values));
            var responseMessage = await response.Content.ReadAsStringAsync(); //right!
            var success = response.IsSuccessStatusCode;

            if (success)
            {
                var message = String.Format("The SmartAgent has the following SmartAgents registered:");
                SmartAgentConfigurationResponse(true, message, null, true, false);

                var defaultMessage = "no SmartAgents configured";
                ConfigResponse(responseMessage, defaultMessage);
            }

            else
            {
                var message = responseMessage;
                var defaultMessage = "Failed to register SmartAgents on SmartAgent";
                SmartAgentConfigurationResponse(false, message, defaultMessage, false, false);
            }
        }

        private async Task InitializeInputMonitoringsOnSmartAgent(string ip, int smartAgentId)
        {
            var values = DataAccess.GetInputMonitorings(smartAgentId);

            string url = String.Format(@"http://{0}:8800/api/initializeNewInputMonitorings", ip);
            var response = await _client.PutAsync(url, GetHttpStringContent(values));
            var responseMessage = await response.Content.ReadAsStringAsync(); //right!
            var success = response.IsSuccessStatusCode;

            if (success)
            {
                var message = String.Format("Input monitoring configs  successfully written to {0}", ip);
                SmartAgentConfigurationResponse(true, message, null, true, false);

                var defaultMessage = "no input Monitorings configured for this SmartAgent";
                ConfigResponse(responseMessage, defaultMessage);

            }

            else
            {
                var message = responseMessage;
                var defaultMessage = "Failed to write down inputMonitoring configurations on SmartAgent";
                SmartAgentConfigurationResponse(false, message, defaultMessage, false, false);
            }
        }


        private void ConfigResponse(string responseMessage, string defaultMessage)
        {
            if (responseMessage.Contains("[]"))
            {
                responseMessage = String.Empty;
                SmartAgentConfigurationResponse(true, responseMessage, defaultMessage, false, false);
            }
            else
            {
                var configs = Regex.Split(responseMessage, @"(?<=},)");
                foreach (string config in configs)
                {
                    SmartAgentConfigurationResponse(true, config, null, false, false);
                }
            }
        }

        private async Task SignSmartAgent(string ip, int smartAgentId)
        {
            var values = smartAgentId;

            string url = String.Format(@"http://{0}:8800/api/signSmartAgentAndLoadConfiguration", ip);
            var response = await _client.PutAsync(url, GetHttpStringContent(values));
            var responseMessage = await response.Content.ReadAsStringAsync(); //right!
            var success = response.IsSuccessStatusCode;

            if (success)
            {
                var message = String.Format("SmartAgent successfully signed with Id {0} and running with new configuration", smartAgentId);
                SmartAgentConfigurationResponse(true, message, null, true, false);
            }
            else
            {
                var message = responseMessage;
                var defaultMessage = "Failed to sign SmartAgent";

                SmartAgentConfigurationResponse(false, message, defaultMessage, false, false);
            }
        }


        public static async void SmartAgentConfigurationResponse(bool isSuccess, string message, string defaultMessage, bool isGreenColored, bool isHeader)
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
            await SmartDataHubClients.All.InvokeAsync("SmartAgentConfigurationResponse", isSuccess, responseMessage, isGreenColored, isHeader);
        }


        public async Task BroadcastSearchSmartAgents()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostInfo = Dns.GetHostEntry(hostName);
            bool anySmartAgentFound = false;

            foreach (IPAddress ip in hostInfo.AddressList)
            {
                byte[] adressBytes = { 0, 0, 0, 0 };

                if (!(ip.IsIPv6LinkLocal | ip.IsIPv6Multicast | ip.IsIPv6SiteLocal))
                    for (int i = 0; i < 255; i++)
                    {
                        adressBytes = ip.GetAddressBytes();
                        adressBytes[3] = (byte)i;

                        if (adressBytes[0] == 192 && adressBytes[3] >= 200 && adressBytes[2] == 0)
                        {
                            var ipAddress = new IPAddress(adressBytes);

                            var message = String.Format("Searching SmartAgents {0}...", ipAddress.ToString());
                            SmartAgentConfigurationResponse(true, message, null, false, true);

                            var cts = new CancellationTokenSource();
                            cts.CancelAfter(100);
                            CancellationToken ct = cts.Token;

                            try
                            {
                                string url = String.Format(@"http://{0}:8800/api/testSmartAgentConnection", ipAddress.ToString());
                                var response = await _client.GetAsync(url, ct);

                                anySmartAgentFound = true;
                                var smessage = String.Format("Connection to SmartAgent {0} successfull", ipAddress.ToString());
                                SmartAgentConfigurationResponse(true, smessage, null, true, false);
                            }
                            catch (TaskCanceledException ex)
                            {
                                continue;
                            }
                        }
                    }
            }

            if (!anySmartAgentFound)
            {
                var message = String.Format("no SmartAgents found in network");
                SmartAgentConfigurationResponse(false, message, null, false, false);
            }

            else
            {
                var message = String.Format("broadcast search finished");
                SmartAgentConfigurationResponse(true, message, null, false, false);
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

