
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Timers;
using System.Collections.ObjectModel;

namespace DeviceMonitoring
{

    public class MonitoringHub : Hub
    {
      
        private IDeviceCommunication deviceCommunication;
        //private static Timer _heartbeatTimer = new Timer(3000);
        private static IHubClients _clients;

        public MonitoringHub()
        {
            // Dependency Injection... Choose the communication protocol
            //deviceCommunication = new RESTfulDeviceCommunication(this);
            deviceCommunication = new SignalRdeviceCommunication(this);

            //_heartbeatTimer.Elapsed -= Heartbeat;
            //_heartbeatTimer.Elapsed += Heartbeat;
            //_heartbeatTimer.AutoReset = true;
            //_heartbeatTimer.Start();
        }

        //private static void Heartbeat(Object source, ElapsedEventArgs e)
        //{
        //    if (_clients != null)
        //    {
        //        _clients.All.InvokeAsync("Heartbeat", true);
        //    }
        //}

        public async  Task    SetDeviceInput(int pinNumber, bool state)
        {
        await deviceCommunication.SetDeviceInput(pinNumber, state);
        }

        public async  Task SetDeviceOutput(int pinNumber, bool state)
        {
            await deviceCommunication.SetDeviceOutput(pinNumber, state);
        }


        public async Task GetAllInputStates()
        {
           await deviceCommunication.GetAllInputStates();
        }

        public async Task GetAllOutputStates()
        {
           await deviceCommunication.GetAllOutputStates();
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


        //public async Task PublishActualCycleMachineData(Dictionary <string, string> actualCycleMachineData )

        //{
        //    await Clients.All.InvokeAsync("PublishActualCycleMachineData", actualCycleMachineData);
        //}



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
