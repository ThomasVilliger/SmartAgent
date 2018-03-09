using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Windows.System.Threading;

namespace SmartAgent.DeviceGatewayCommunication
{
    // the class to communicates to the SmartAgant IO monitoring over SignalR
    class SignalRMonitoringHubCommunication : IMonitoringHubCommunication
    {
        private static HubConnection _hub;
        public SignalRMonitoringHubCommunication()
        {
            EstablishHubConnection();
        }

        private async Task NoConnectionToMonitoringHub(Exception ex)
        {
            _hub.DisposeAsync();
            EstablishHubConnection();
        }


        private async Task EstablishHubConnection()
        {          
            string url = @"http://localhost:59162/MonitoringHub";
            _hub = new HubConnectionBuilder().WithUrl(url).Build();

            _hub.Closed -= NoConnectionToMonitoringHub;
            _hub.Closed += NoConnectionToMonitoringHub;

            await _hub.StartAsync().ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    NoConnectionToMonitoringHub(null);
                }
            });
        }

        public void UpdateSingleInputState(IInput input)
        {
            PinState pinState = new PinState { PinNumber = input.PinNumber, State = input.State };
            _hub?.InvokeAsync("UpdateSingleInputState", pinState);
        }

        public void UpdateSingleOutputState(IOutput output)
        {
            PinState pinState = new PinState { PinNumber = output.PinNumber, State = output.State };
            _hub?.InvokeAsync("UpdateSingleOutputState", pinState);
        }
    }
}
