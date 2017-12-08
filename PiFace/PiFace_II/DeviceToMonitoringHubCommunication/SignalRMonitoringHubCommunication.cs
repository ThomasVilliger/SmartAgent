using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Windows.System.Threading;

namespace PiFace_II.DeviceGatewayCommunication
{
    class SignalRMonitoringHubCommunication : IMonitoringHubCommunication
    {
        private  ThreadPoolTimer _hearthbeatWatchdog;

        private static HubConnection _hub;
        public SignalRMonitoringHubCommunication()
        {

            _hearthbeatWatchdog = ThreadPoolTimer.CreatePeriodicTimer(NoConnectionToMonitoringHub,
                                   TimeSpan.FromMilliseconds(9000));

            EstablishHubConnection();
        }

        private void NoConnectionToMonitoringHub(ThreadPoolTimer timer)
        {
            EstablishHubConnection();

        }


        private void ResetHeartBeatWatchDogTimer()
        {
            lock (this)
            {
                _hearthbeatWatchdog.Cancel();
                _hearthbeatWatchdog = null;
                _hearthbeatWatchdog = ThreadPoolTimer.CreatePeriodicTimer(NoConnectionToMonitoringHub,
                                      TimeSpan.FromMilliseconds(9000));
            }
        }

        private async Task EstablishHubConnection()
        {
           
            string url = @"http://localhost:59162/MonitoringHub";
            _hub = new HubConnectionBuilder().WithUrl(url).Build();
            _hub.On<bool>("Heartbeat", p => Heartbeat(p));
          
            await _hub.StartAsync();
        }

        private void Heartbeat(bool p)
        {
            ResetHeartBeatWatchDogTimer();
        }

        private void OnConnectionError(Task arg1, object arg2)
        {
            Console.WriteLine("MonitoringHub connection error");
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
