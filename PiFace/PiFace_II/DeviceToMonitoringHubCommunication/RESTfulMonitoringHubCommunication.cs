using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PiFace_II.DeviceGatewayCommunication
{
    class RESTfulMonitoringHubCommunication : IMonitoringHubCommunication
    {
        private static readonly HttpClient client = new HttpClient();

        public void UpdateSingleInputState(IInput input)
        {
            var values = new PinState { PinNumber = input.PinNumber, State = input.State };
            var jsonContent = JsonConvert.SerializeObject(values);
            var stringContent = new StringContent(jsonContent.ToString());
            stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            string url = String.Format(@"http://192.168.0.13:59162/DeviceMonitoring/UpdateSingleInputState/");
            client.PutAsync(url, stringContent);
        }

        public void UpdateSingleOutputState(IOutput output)
        {
            var values = new PinState { PinNumber = output.PinNumber, State = output.State };
            var jsonContent = JsonConvert.SerializeObject(values);
            var stringContent = new StringContent(jsonContent.ToString());
            stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            string url = String.Format(@"http://192.168.0.13:59162/DeviceMonitoring/UpdateSingleOutputState/");
            client.PutAsync(url, stringContent);
        }
    }
}
