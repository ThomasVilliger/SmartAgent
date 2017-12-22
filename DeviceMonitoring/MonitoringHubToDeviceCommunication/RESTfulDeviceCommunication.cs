using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;

namespace DeviceMonitoring
{
    
    
    public class RESTfulDeviceCommunication : IDeviceCommunication
    {
        private static readonly HttpClient client = new HttpClient();

        private MonitoringHub hub;

        public RESTfulDeviceCommunication(MonitoringHub hub)
        {
            this.hub = hub;
        }


        public Task SetDeviceInput(int pinNumber, bool state)
        {
            var values = new Dictionary<string, string>
{
   { "PinNumber", pinNumber.ToString() },
   { "State", state.ToString() }
};

            var content = new FormUrlEncodedContent(values);
          string url = String.Format(@"http://localhost:8800/api/setDeviceInput/");

            return client.PostAsync(url, content);

        }

        public Task SetDeviceOutput(int pinNumber, bool state)
        {
            var values = new Dictionary<string, string>
{
   { "PinNumber", pinNumber.ToString() },
   { "State", state.ToString() }
};

            var content = new FormUrlEncodedContent(values);


            string url = String.Format(@"http://localhost:8800/api/setDeviceOutput/");

            return client.PostAsync(url, content);


        }

        public  async Task GetAllInputStates()
        {
            string url = String.Format(@"http://localhost:8800/api/getAllDeviceInputStates");

            HttpResponseMessage getResponse = await client.GetAsync(url);

            int numberOfInputs = Convert.ToInt32(getResponse.Headers.FirstOrDefault(m => m.Key == "NumberOfInputs").Value.FirstOrDefault());
            List<PinState> inputStates = new List<PinState>();

            for (int i = 0; i < numberOfInputs; i++)
            {
                bool state = Convert.ToBoolean(getResponse.Headers.FirstOrDefault(m => m.Key == i.ToString()).Value.FirstOrDefault());
                inputStates.Add(new PinState { PinNumber = i, State = state });
            }

            hub.UpdateAllInputStates(inputStates);

            
        }

        public async Task GetAllOutputStates()
        {
            string url = String.Format(@"http://localhost:8800/api/getAllDeviceOutputStates");

            HttpResponseMessage getResponse = await client.GetAsync(url);

            int numberOfOutputs = Convert.ToInt32(getResponse.Headers.FirstOrDefault(m => m.Key == "NumberOfOutputs").Value.FirstOrDefault());
            List<PinState> outputStates = new List<PinState>();

            for (int i = 0; i < numberOfOutputs; i++)
            {
                bool state = Convert.ToBoolean(getResponse.Headers.FirstOrDefault(m => m.Key == i.ToString()).Value.FirstOrDefault());
                outputStates.Add(new PinState { PinNumber = i, State = state });
            }


            hub.UpdateAllOutputStates(outputStates);

        }
    }
}
