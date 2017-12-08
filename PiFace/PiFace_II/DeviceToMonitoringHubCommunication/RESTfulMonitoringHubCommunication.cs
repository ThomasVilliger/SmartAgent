using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PiFace_II.DeviceGatewayCommunication
{
    class RESTfulMonitoringHubCommunication : IMonitoringHubCommunication
    {
        private static readonly HttpClient client = new HttpClient();

//        public void PublishActualCycleMachineData(CycleMachine cycleMachine)
//        {
//            var actualCycleMachineData = new Dictionary<string, string>
//{
//   { "MachineState", cycleMachine.CurrentMachineState.ToString() },
//   { "DailyCycleCounter", cycleMachine.DailyCycleCounter.ToString() },
//   { "CycleTime", cycleMachine.LastCycleTime.ToString() },
//   { "CycleCounterPerMachineState", cycleMachine.CycleCounterPerMachineState.ToString()},
//   { "MachineId", cycleMachine.CycleMachineConfiguration.MachineId.ToString() }



//};

//            var content = new FormUrlEncodedContent(actualCycleMachineData);

//            string url = String.Format(@"http://192.168.0.13:59162/DeviceMonitoring/PublishActualCycleMachineData/");

//            var response = client.PostAsync(url, content);


//        }

        public void UpdateSingleInputState(IInput input)
        {
            var values = new Dictionary<string, string>
{
   { "PinNumber", input.PinNumber.ToString() },
   { "State", input.State.ToString() }
};

            var content = new FormUrlEncodedContent(values);


            string url = String.Format(@"http://192.168.0.13:59162/DeviceMonitoring/UpdateSingleInputState/");

            var response = client.PostAsync(url, content);




        }

        public void UpdateSingleOutputState(IOutput output)
        {

            var values = new Dictionary<string, string>
{
   { "PinNumber", output.PinNumber.ToString() },
   { "State", output.State.ToString() }
};



            var content = new FormUrlEncodedContent(values);


            string url = String.Format(@"http://192.168.0.13:59162/DeviceMonitoring/UpdateSingleOutputState/");

            var response = client.PostAsync(url, content);
        }
    }
}
