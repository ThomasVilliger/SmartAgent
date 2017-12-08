﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;

namespace DeviceMonitoring
{
    public class SignalRdeviceCommunication : IDeviceCommunication
    {
        private Hub _hub;

       public SignalRdeviceCommunication(Hub hubClients)
        {
            _hub = hubClients;
        }


        public Task SetDeviceInput(int pinNumber, bool state)
        {
            PinState pinState = new PinState { PinNumber = pinNumber, State = state };
            return _hub.Clients.All.InvokeAsync("SetDeviceInput", pinState);
        }

        public Task SetDeviceOutput(int pinNumber, bool state)
        {
            PinState pinState = new PinState { PinNumber = pinNumber, State = state };
            return _hub.Clients.All.InvokeAsync("SetDeviceOutput", pinState);
        }


        public Task GetAllInputStates()
        {
          return _hub.Clients.All.InvokeAsync("GetAllInputStates");
        }

        public Task GetAllOutputStates()
        {
            return _hub.Clients.All.InvokeAsync("GetAllOutputStates");
        }
    }
}
