
using Microsoft.AspNetCore.SignalR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DeviceMonitoring
{
    public class MonitoringHub : Hub
    {

        private static readonly HttpClient client = new HttpClient();



        public Task UpdateAllClientsInputState( int pinNumber, bool state)
        {
                object parameters = new Parameters { PinNumber = pinNumber, State = state };

            //Clients.All.UpdateInput(pinNumber, state); old

           return  Clients.All.InvokeAsync("UpdateInputState", parameters);

        }



        public Task UpdateAllClientsOutputState(int pinNumber, bool state)
        {
                object parameters = new Parameters { PinNumber = pinNumber, State = state };

            return Clients.All.InvokeAsync("UpdateOutputState", parameters);
        }





        public Task SetDeviceInput(int pinNumber, bool state)
        {

            var values = new Dictionary<string, string>
{
   { "PinNumber", pinNumber.ToString() },
   { "State", state.ToString()}
};



            var content = new FormUrlEncodedContent(values);


            string url = String.Format(@"http://localhost:8800/writeDeviceInput/");

           return client.PostAsync(url, content);

        }




        public Task SetDeviceOutput(int pinNumber, bool state)
        {

            var values = new Dictionary<string, string>
{
   { "PinNumber", pinNumber.ToString() },
   { "State", state.ToString()}
};



            var content = new FormUrlEncodedContent(values);

            object o = (object)content;


            string url = String.Format(@"http://localhost:8800/writeDeviceOutput/");

            return client.PostAsync(url, content);

        }



 
  








    }






    public class Parameters
    {
        public int PinNumber;
        public bool State;

    }
}
