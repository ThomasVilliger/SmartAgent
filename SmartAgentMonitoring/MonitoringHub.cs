using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace DeviceMonitoring
{
    // the signalR hub for the monitoring web client
    public class MonitoringHub : Hub
    {   
        private IDeviceCommunication _deviceCommunication;
        private static IHubClients _clients;

        public MonitoringHub()
        {
            // Dependency Injection... Choose the communication protocol
            _deviceCommunication = new RESTfulDeviceCommunication(this);
            //_deviceCommunication = new SignalRdeviceCommunication(this);
        }

        public async  Task    SetDeviceInput(int pinNumber, bool state)
        {
        await _deviceCommunication.SetDeviceInput(pinNumber, state);
        }

        public async  Task SetDeviceOutput(int pinNumber, bool state)
        {
            await _deviceCommunication.SetDeviceOutput(pinNumber, state);
        }

        public async Task GetAllInputStates()
        {
           await _deviceCommunication.GetAllInputStates();
        }

        public async Task GetAllOutputStates()
        {
           await _deviceCommunication.GetAllOutputStates();
        }

        public async Task UpdateAllInputStates(List<PinState> inputStates)
        {
            await Clients.All.InvokeAsync("UpdateAllInputStates", inputStates);
        }

        public async Task UpdateAllOutputStates(List<PinState> outputStates)
        {
            await Clients.Client(ConnectionCounter.ConnectedIds.Last()).InvokeAsync("UpdateAllOutputStates", outputStates);  // TODO!!!!!
        }

        public async Task UpdateSingleInputState(PinState pinState)
        {
           await Clients.All.InvokeAsync("UpdateSingleInputState", pinState);
        }


        public async Task UpdateSingleOutputState(PinState pinState)
        {
           await Clients.All.InvokeAsync("UpdateSingleOutputState", pinState);
        }

        public override Task OnConnectedAsync()
        {
            _clients = this.Clients;

            try
            {
                ConnectionCounter.ConnectedIds.Add(Context.ConnectionId);
                _clients.All.InvokeAsync("UpdateClientCounter", ConnectionCounter.ConnectedIds.Count);
            }
            catch(Exception ex)
            {
                this.Context.Connection.Abort();
            }
            GetAllInputStates();
            GetAllOutputStates();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            _clients = this.Clients;
           
            ConnectionCounter.ConnectedIds.Remove(Context.ConnectionId);
            _clients.All.InvokeAsync("UpdateClientCounter", ConnectionCounter.ConnectedIds.Count);
            return base.OnDisconnectedAsync(ex);
        }
    }

    public static class ConnectionCounter
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }

    public class PinState
    {
        public int PinNumber;
        public bool State;
    }
}
