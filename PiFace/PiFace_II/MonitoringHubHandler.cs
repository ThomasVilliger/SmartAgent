
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Windows.System.Threading;

namespace PiFace_II
{
   public class MonitoringHubHandler
    {
        private static HubConnection _hub;
        private IDevice _device;
        private  ThreadPoolTimer _heartbeatWatchdog;

        public MonitoringHubHandler(IDevice device)
        {

            _heartbeatWatchdog = ThreadPoolTimer.CreatePeriodicTimer(NoConnectionToGatewayHub,
                                   TimeSpan.FromMilliseconds(9000));

            _device = device;
            EstablishHubConnection();

        }





        private void ResetHeartbeatWatchDog()
        {
            _heartbeatWatchdog.Cancel();
            _heartbeatWatchdog = ThreadPoolTimer.CreatePeriodicTimer(NoConnectionToGatewayHub,
                                  TimeSpan.FromMilliseconds(9000));
        }


        private void NoConnectionToGatewayHub(ThreadPoolTimer timer)
        {

            _hub.DisposeAsync();
            EstablishHubConnection();
            ResetHeartbeatWatchDog();

        }


        private void Heartbeat(bool p)
        {
            ResetHeartbeatWatchDog();
        }



        private async Task EstablishHubConnection()
        {


           
            string url = @"http://192.168.0.13:59162/MonitoringHub";
        _hub = new HubConnectionBuilder().WithUrl(url).Build();
            _hub.On<bool>("Heartbeat", p => Heartbeat(p));
            _hub.On<PinState>("SetDeviceInput", pinState => SetDeviceInput(pinState));
            _hub.On<PinState>("SetDeviceOutput", pinState => SetDeviceOutput(pinState));

            _hub.On("GetAllInputStates", GetAllInputStates);
            _hub.On("GetAllOutputStates", GetAllOutputStates);


            await _hub.StartAsync().ContinueWith(OnConnectionError, TaskContinuationOptions.OnlyOnFaulted);

            //await _hub.StartAsync(); 
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

        private void OnConnectionError(Task arg1, object arg2)
    {
        Console.WriteLine("MonitoringHub connection error");
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
