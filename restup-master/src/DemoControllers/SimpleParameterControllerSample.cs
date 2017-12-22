using Restup.DemoControllers.Model;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System.Collections.Generic;
using System.Linq;
using PiFace_II;
using Windows.Data.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using System;

using System.Collections.ObjectModel;
using System.Net.Http;
using System.Collections.Immutable;


namespace Restup.DemoControllers
{
    

    [RestController(InstanceCreationType.Singleton)]
    public sealed class SimpleParameterControllerSample
    {


        private  PiFaceMain _piFaceMain { get; set; } 
      private List<MachineStateHistoryEntity> machineStateHistory;
        /// <summary>
        /// Make sure the number of parameters in your UriFormat match the parameters in your method and
        /// the names (case sensitive) and order are respected.
        /// </summary>
        /// 

        public SimpleParameterControllerSample(PiFaceMain piFaceMain)
        {
            _piFaceMain = piFaceMain;
        }


        [UriFormat("/getCurrentMachineData/{machineId}")]
        public IGetResponse GetCurrentMachineData(int machineId)
        {
            CycleMachine cycleMachine = _piFaceMain.GatewayHubHandler.CycleMachines.FirstOrDefault(m => m.CycleMachineConfiguration.MachineId == machineId);
            MachineStateHistoryEntity lastMachineStateHistoryEntity = cycleMachine.MachineStateHistory.LastOrDefault();
            TimeSpan durationCurrentMachineState;
            if (lastMachineStateHistoryEntity != null)
            {
                durationCurrentMachineState = DateTime.Now - lastMachineStateHistoryEntity.StartDateTime;
            }

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                new CurrentMachineData()
                {
                    MachineState = cycleMachine.CurrentMachineState.ToString(),
                    CycleCounterPerMachineState = cycleMachine.CyclesInThisPeriod,
                    DailyCycleCunter = cycleMachine.DailyCycleCounter,
                    LastCycleTime = cycleMachine.LastCycleTime,
                    DurationCurrentMachineState = durationCurrentMachineState.ToString()


                });
        }


        [UriFormat("/getMachineHistory/{machineId}")]
        public IGetResponse GetMachineHistory(int machineId)
        {
            machineStateHistory = _piFaceMain.GatewayHubHandler.CycleMachines.FirstOrDefault(m=>m.CycleMachineConfiguration.MachineId == machineId).MachineStateHistory;

            //MemoryStream stream1 = new MemoryStream();
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<MachineStateHistoryEntity>));
            //ser.WriteObject(stream1, machineStateHistory);
            //StreamReader streamReader = new StreamReader(stream1);
            //stream1.Position = 0;
            //string machineHistory = streamReader.ReadToEnd();

              XmlSerializer _xs = new XmlSerializer(typeof(List<MachineStateHistoryEntity>));

            MemoryStream fileStream = new MemoryStream();

            _xs.Serialize(fileStream, machineStateHistory);
            fileStream.Flush();
     
            fileStream.Position = 0;
            StreamReader streamReader = new StreamReader(fileStream);
            string machineHistory = streamReader.ReadToEnd();


            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                new MachineHistoryData()
                {
                    MachineStateHistory = machineHistory
                });
        }

        [UriFormat("/setDeviceInput/")]
        public IPutResponse SetDeviceInput([FromContent] Dictionary<string, string> parameters)
        {
            int pinNumber = Convert.ToInt32(parameters.FirstOrDefault(m => m.Key == "PinNumber").Value);
            bool state = Convert.ToBoolean(parameters.FirstOrDefault(m => m.Key == "State").Value);

            _piFaceMain.device.SetDeviceInput(pinNumber, state);
            return new PutResponse(PutResponse.ResponseStatus.OK);
        }


        [UriFormat("/setDeviceOutput/")]
        public IPutResponse SetDeviceOutput([FromContent] Dictionary<string, string> parameters)
        {

            int pinNumber = Convert.ToInt32(parameters.FirstOrDefault(m => m.Key == "PinNumber").Value);
            bool state = Convert.ToBoolean(parameters.FirstOrDefault(m => m.Key == "State").Value);

            _piFaceMain.device.SetDeviceOutput(pinNumber, state);
            return new PutResponse(PutResponse.ResponseStatus.OK);
        }




        [UriFormat("/initializeNewMachineConfigurations/")]
        public IPutResponse InitializeNewMachineConfigurations([FromContent] List<DataStorageLibrary.CycleMachineConfiguration> configuration)
        //public IPutResponse PutInitializeNewMachineConfigurations()
        {

            try
            {

                _piFaceMain.GatewayHubHandler.InitializeNewMachineConfigurations(configuration);
                return new PutResponse(PutResponse.ResponseStatus.OK, configuration);
            }


            catch(Exception ex)
            {
                return new PutResponse(PutResponse.ResponseStatus.NotFound, ex.Message);
            }
        }



        [UriFormat("/initializeNewInputMonitoringConfigurations/")]
        public IPutResponse InitializeNewInputMonitoringConfigurations([FromContent] List<Dictionary<string, string>> monitoringConfigurations)
        {
            _piFaceMain.GatewayHubHandler.InitializeNewInputMonitoringConfigurations(monitoringConfigurations);
            return new PutResponse(PutResponse.ResponseStatus.OK);
        }




        [UriFormat("/testSmartAgentConnection/")]
        public IGetResponse TestSmartAgentConnection()
        {
            return new GetResponse(GetResponse.ResponseStatus.OK);
        }









        [UriFormat("/getAllDeviceInputStates/")]
        public IGetResponse GetAllDeviceInputStates()
        {

            var deviceInputStates = new Dictionary<string, string>();

            int numberOfInputs = _piFaceMain.device.NumberOfInputs;

            deviceInputStates.Add("NumberOfInputs", numberOfInputs.ToString());


            foreach (IInput input in _piFaceMain.device.Inputs)
            {
                deviceInputStates.Add(input.PinNumber.ToString(), input.State.ToString());

            }

            return new GetResponse(GetResponse.ResponseStatus.OK, deviceInputStates);
        }



        [UriFormat("/getAllDeviceOutputStates/")]
        public IGetResponse GetAllDeviceOutputStates()
        {

            var deviceOutputStates = new Dictionary<string, string>();
            int numberOfOutputs = _piFaceMain.device.NumberOfOutputs;
            deviceOutputStates.Add("NumberOfOutputs", numberOfOutputs.ToString());


            foreach (IOutput output in _piFaceMain.device.Outputs)
            {
                deviceOutputStates.Add(output.PinNumber.ToString(), output.State.ToString());

            }
            return new GetResponse(GetResponse.ResponseStatus.OK, deviceOutputStates);
        }



      //  [UriFormat("/writeDeviceInput/")]
      //  public IPutResponse PutWriteDeviceInput([FromContent] Dictionary<string, string> parameters)
      //  {

      //      int pinNumber = Convert.ToInt32(parameters.FirstOrDefault(m => m.Key == "PinNumber").Value);
      //      bool state = Convert.ToBoolean(parameters.FirstOrDefault(m => m.Key == "State").Value);

      //      _piFaceMain.device.SetDeviceInput(pinNumber, state);

      //      return new PutResponse(PutResponse.ResponseStatus.OK);
      //  }


      //  [UriFormat("/writeDeviceOutput/")]
      ////  public IPostResponse writeDeviceOutput([FromContent] Dictionary<string, string> parameters)
      //      public IPutResponse PutWriteDeviceOutput([FromContent] Dictionary<string, string> parameters)
      //  {

      //      int pinNumber = Convert.ToInt32(parameters.FirstOrDefault(m => m.Key == "PinNumber").Value);
      //      bool state = Convert.ToBoolean(parameters.FirstOrDefault(m => m.Key == "State").Value);

      //      _piFaceMain.device.SetDeviceOutput(pinNumber, state);

      //      return new PutResponse(PutResponse.ResponseStatus.OK);
      //  }













        /// <summary>
        /// Make sure the number of parameters in your UriFormat match the parameters in your method and
        /// the names (case sensitive) and order are respected.
        /// </summary>

        [UriFormat("/simpleparameter/{id}/property/{propName}")]
        public IDeleteResponse DeleteWithSimpleParameters(int id, string propName)
        {
            return new DeleteResponse(DeleteResponse.ResponseStatus.OK);
        }


  

        /// <summary>
        /// Make sure the number of parameters in your UriFormat match the parameters in your method and
        /// the names (case sensitive) and order are respected.
        /// </summary>
        [UriFormat("/intparameterarray/{ids}")]
        public IGetResponse GetWithIntArrayParameters(IEnumerable<int> ids)
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                new DataReceived()
                {
                    ID = ids.Sum()
                });
        }

        /// <summary>
        /// Make sure the number of parameters in your UriFormat match the parameters in your method and
        /// the names (case sensitive) and order are respected.
        /// </summary>
        [UriFormat("/doubleparameterarray/{ids}")]
        public IGetResponse GetWithDoubleArrayParameters(IEnumerable<double> ids)
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                new DataReceived()
                {
                    ID = (int)ids.Sum()
                });
        }



        [UriFormat("/test/")]
        public IGetResponse GetTest()
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                new DataReceived()
                {
                    
                });
        }
    }
}
