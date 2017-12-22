
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
        private static ThreadPoolTimer _heartbeatWatchdog;

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
            EstablishHubConnection();

            ResetHeartbeatWatchDog();

        }


        private void Heartbeat(bool p)
        {
            ResetHeartbeatWatchDog();
        }



        private async Task EstablishHubConnection()
        {


           
            string url = @"http://localhost:59162/MonitoringHub";
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
            foreach (IInput output in _device.Inputs)
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
            try
            {
                if (pinState.PinNumber < _device.NumberOfInputs)
                {
                    _device.Inputs[pinState.PinNumber].State = pinState.State;
                }
            }
            catch (Exception)
            {

            }
        }



        private void SetDeviceOutput(PinState pinState)
        {
            try
            {
                if (pinState.PinNumber < _device.NumberOfOutputs)
                {
                    _device.Outputs[pinState.PinNumber].State = pinState.State;
                }
            }
            catch (Exception)
            {

            }
        }
    }



    public class PinState
    {
        public int PinNumber;
        public bool State;
    }

}
