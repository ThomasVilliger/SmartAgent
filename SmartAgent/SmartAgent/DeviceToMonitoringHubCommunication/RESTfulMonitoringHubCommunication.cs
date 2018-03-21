using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmartAgent.DeviceGatewayCommunication
{
    // the class to communicate to the SmartAgant IO monitoring over RESTful
    class RESTfulMonitoringHubCommunication : IMonitoringHubCommunication
    {
        private static readonly HttpClient _client = new HttpClient();

        public void UpdateSingleInputState(IInput input)
        {
            string url = String.Format(@"http://192.168.0.13:59162/DeviceMonitoring/UpdateSingleInputState/");
            _client.PutAsync(url, GetHttpStringContent(new PinState { PinNumber = input.PinNumber, State = input.State }));
        }

        public void UpdateSingleOutputState(IOutput output)
        {
            //var values = new PinState { PinNumber = output.PinNumber, State = output.State };
            //var jsonContent = JsonConvert.SerializeObject(values);
            //var stringContent = new StringContent(jsonContent.ToString());
            //stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            string url = String.Format(@"http://192.168.0.13:59162/DeviceMonitoring/UpdateSingleOutputState/");
            _client.PutAsync(url, GetHttpStringContent(new PinState { PinNumber = output.PinNumber, State = output.State }));
        }


        private StringContent GetHttpStringContent(object values)
        {
            var jsonContent = JsonConvert.SerializeObject(values);
            var stringContent = new StringContent(jsonContent.ToString());
            stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            return stringContent;
        }
    }
}
