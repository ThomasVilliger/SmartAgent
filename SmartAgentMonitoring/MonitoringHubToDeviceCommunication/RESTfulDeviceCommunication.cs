using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DeviceMonitoring
{
    // communication with the device over RESTful
    public class RESTfulDeviceCommunication : IDeviceCommunication
    {
        private static readonly HttpClient _client = new HttpClient();
        private MonitoringHub _hub;

        public RESTfulDeviceCommunication(MonitoringHub hub)
        {
            _hub = hub;
        }

        public Task SetDeviceInput(PinState pinState)
        {       
            string url = String.Format(@"http://192.168.0.207:8800/api/setDeviceInput/");
            return  _client.PutAsync(url, GetHttpStringContent(pinState));        
        }

        public Task SetDeviceOutput(PinState pinState)
        {         
            string url = String.Format(@"http://192.168.0.207:8800/api/setDeviceOutput/");
            return _client.PutAsync(url, GetHttpStringContent(pinState));
        }

        public async Task<List<PinState>>  GetAllInputStates()
        {
            string url = String.Format(@"http://192.168.0.207:8800/api/getAllDeviceInputStates");

            HttpResponseMessage getResponse = await _client.GetAsync(url);

            int numberOfInputs = Convert.ToInt32(getResponse.Headers.FirstOrDefault(m => m.Key == "NumberOfInputs").Value.FirstOrDefault());
            List<PinState> inputStates = new List<PinState>();

            for (int i = 0; i < numberOfInputs; i++)
            {
                bool state = Convert.ToBoolean(getResponse.Headers.FirstOrDefault(m => m.Key == i.ToString()).Value.FirstOrDefault());
                inputStates.Add(new PinState { PinNumber = i, State = state });
            }

            return inputStates;
        }

        public async Task<List<PinState>> GetAllOutputStates()
        {
            string url = String.Format(@"http://192.168.0.207:8800/api/getAllDeviceOutputStates");

            HttpResponseMessage getResponse = await _client.GetAsync(url);

            int numberOfOutputs = Convert.ToInt32(getResponse.Headers.FirstOrDefault(m => m.Key == "NumberOfOutputs").Value.FirstOrDefault());
            List<PinState> outputStates = new List<PinState>();

            for (int i = 0; i < numberOfOutputs; i++)
            {
                bool state = Convert.ToBoolean(getResponse.Headers.FirstOrDefault(m => m.Key == i.ToString()).Value.FirstOrDefault());
                outputStates.Add(new PinState { PinNumber = i, State = state });
            }
            return outputStates;
        }


        private StringContent GetHttpStringContent(object values)
        {
            var jsonContent = JsonConvert.SerializeObject(values);
            return new StringContent(jsonContent.ToString());
        }
    }
}
