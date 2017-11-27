
using Microsoft.AspNetCore.SignalR;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DeviceMonitoring
{
    public class MonitoringHub : Hub
    {
        private static readonly HttpClient client = new HttpClient();


        public  Task    SetDeviceInput(int pinNumber, bool state)
        {
            string url = String.Format(@"http://smartagent:8800/api/writedeviceinput/{0}/state/{1}", pinNumber, state);
            return client.GetAsync(url);
        }


        public  Task SetDeviceOutput(int pinNumber, bool state)
        {
            string url = String.Format(@"http://smartagent:8800/api/writedeviceoutput/{0}/state/{1}", pinNumber, state);
            return client.GetAsync(url);
        }


        public async Task UpdateAllInputStates()
        {
            string url = String.Format(@"http://smartagent:8800/api/getAllDeviceInputStates");

            HttpResponseMessage getResponse = await client.GetAsync(url);

            int numberOfInputs = Convert.ToInt32(getResponse.Headers.FirstOrDefault(m => m.Key == "NumberOfInputs").Value.FirstOrDefault());
            List<PinState> inputStates = new List<PinState>();

            for (int i = 0; i < numberOfInputs; i++)
            {
                bool state = Convert.ToBoolean(getResponse.Headers.FirstOrDefault(m => m.Key == String.Format("{0}", i)).Value.FirstOrDefault());
                inputStates.Add(new PinState { PinNumber = i, State = state });
            }

            Clients.Client(Context.ConnectionId).InvokeAsync("UpdateAllInputStates", inputStates);
        }


        public async Task UpdateAllOutputStates()
        {
            string url = String.Format(@"http://smartagent:8800/api/getAllDeviceOutputStates");

            HttpResponseMessage getResponse = await client.GetAsync(url);

            int numberOfOutputs = Convert.ToInt32(getResponse.Headers.FirstOrDefault(m => m.Key == "NumberOfOutputs").Value.FirstOrDefault());
            List<PinState> outputStates = new List<PinState>();

            for (int i = 0; i < numberOfOutputs; i++)
            {
                bool state = Convert.ToBoolean(getResponse.Headers.FirstOrDefault(m => m.Key == String.Format("{0}", i)).Value.FirstOrDefault());
                outputStates.Add(new PinState { PinNumber = i, State = state });
            }
            Clients.Client(Context.ConnectionId).InvokeAsync("UpdateAllOutputStates", outputStates);
        }

        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            Clients.All.InvokeAsync("UpdateClientCounter", UserHandler.ConnectedIds.Count);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            Clients.All.InvokeAsync("UpdateClientCounter", UserHandler.ConnectedIds.Count);
            return base.OnDisconnectedAsync(ex);
        }
    }

    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }

    public class PinState
    {
        public int PinNumber;
        public bool State;
    }
}
