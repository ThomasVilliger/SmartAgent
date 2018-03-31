
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Windows.System.Threading;

namespace SmartAgent
{
    public class MonitoringHubHandler
    {
        // communicates to the SmartAgent Monitoring Solution over SignalR
        private static HubConnection _hub;
        private IDevice _device;

        public MonitoringHubHandler(IDevice device)
        {
            _device = device;
            EstablishHubConnection();
        }

        private async Task NoConnectionToMonitoringHub(Exception ex)
        {
            _hub.DisposeAsync();
            EstablishHubConnection();
        }

        // connects to the signalR server and rigisters to the events
        private async Task EstablishHubConnection()
        {
            string url = @"http://localhost:59162/MonitoringHub";
            _hub = new HubConnectionBuilder().WithUrl(url).Build();

            _hub.On<PinState>("SetDeviceInput", pinState => SetDeviceInput(pinState));
            _hub.On<PinState>("SetDeviceOutput", pinState => SetDeviceOutput(pinState));

            _hub.On("GetAllInputStates", GetAllInputStates);
            _hub.On("GetAllOutputStates", GetAllOutputStates);

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

        private void GetAllInputStates()
        {
            List<PinState> inputStates = new List<PinState>();
            foreach (IInput input in _device.Inputs)
            {
                inputStates.Add(new PinState { PinNumber = input.PinNumber, State = input.State });
            }
            _hub.InvokeAsync("UpdateAllInputStates", inputStates);
        }

        private void GetAllOutputStates()
        {
            List<PinState> outputStates = new List<PinState>();
            foreach (IOutput output in _device.Outputs)
            {
                outputStates.Add(new PinState { PinNumber = output.PinNumber, State = output.State });
            }
            _hub.InvokeAsync("UpdateAllOutputStates", outputStates);
        }

        private void SetDeviceInput(PinState pinState)
        {
            _device.SetDeviceInput(pinState.PinNumber, pinState.State);
        }

        private void SetDeviceOutput(PinState pinState)
        {
            _device.SetDeviceOutput(pinState.PinNumber, pinState.State);
        }
    }

    public class PinState
    {
        public int PinNumber;
        public bool State;
    }
}
