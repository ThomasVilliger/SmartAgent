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
using DataStorageLibrary;
using Newtonsoft.Json;

using System.Collections.ObjectModel;
using System.Net.Http;
using System.Collections.Immutable;


namespace Restup.DemoControllers
{
    

    [RestController(InstanceCreationType.Singleton)]
    public sealed class SimpleParameterControllerSample
    {


        private  PiFaceMain _piFaceMain { get; set; } 
      private List<MachineStateHistory> machineStateHistory;
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
            Machine machine = _piFaceMain.GatewayHubHandler.Machines.FirstOrDefault(m => m.MachineConfiguration.MachineId == machineId);
            MachineStateHistory lastMachineStateHistoryEntity = machine.MachineStateHistory.LastOrDefault();
            TimeSpan durationCurrentMachineState;
            if (lastMachineStateHistoryEntity != null)
            {
                durationCurrentMachineState = DateTime.Now - lastMachineStateHistoryEntity.StartDateTime;
            }

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                new CurrentMachineData()
                {
                    MachineState = machine.CurrentMachineState.ToString(),
                    CycleCounterPerMachineState = machine.CyclesInThisPeriod,
                    DailyCycleCunter = machine.DailyCycleCounter,
                    LastCycleTime = machine.LastCycleTime,
                    DurationCurrentMachineState = durationCurrentMachineState.ToString()


                });
        }


        [UriFormat("/getMachineHistory/{machineId}")]
        public IGetResponse GetMachineHistory(int machineId)
        {
            machineStateHistory = _piFaceMain.GatewayHubHandler.Machines.FirstOrDefault(m=>m.MachineConfiguration.MachineId == machineId).MachineStateHistory;

            //MemoryStream stream1 = new MemoryStream();
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<MachineStateHistoryEntity>));
            //ser.WriteObject(stream1, machineStateHistory);
            //StreamReader streamReader = new StreamReader(stream1);
            //stream1.Position = 0;
            //string machineHistory = streamReader.ReadToEnd();

              XmlSerializer _xs = new XmlSerializer(typeof(List<MachineStateHistory>));

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




        [UriFormat("/initializeNewMachines/")]
        public IPutResponse InitializeNewMachines([FromContent] List<MachineConfiguration> configs)
        //public IPutResponse PutInitializeNewMachineConfigurations()
        {
            try
            {
                DataAccess.MachinesConfigurations(configs);
                return new PutResponse(PutResponse.ResponseStatus.OK, configs);
            }

            catch(Exception ex)
            {
                return new PutResponse(PutResponse.ResponseStatus.NotFound, ex.Message);
            }
        }



        [UriFormat("/initializeNewInputMonitorings/")]
        public IPutResponse InitializeNewInputMonitorings([FromContent] List<InputMonitoringConfiguration> configs)
        {
            try
            {
                DataAccess.StoreInputMonitoringConfigurations(configs);
                return new PutResponse(PutResponse.ResponseStatus.OK, configs);
            }

            catch (Exception ex)
            {
                return new PutResponse(PutResponse.ResponseStatus.NotFound, ex.Message);
            }
        }


                [UriFormat("/signSmartAgentAndLoadConfiguration/")]
        public IPutResponse SignSmartAgent([FromContent] int smartAgentId)
        {
            try
            {
                DataAccess.SignSmartAgent(smartAgentId);
                _piFaceMain.GatewayHubHandler.LoadSmartAgentConfiguration();
                return new PutResponse(PutResponse.ResponseStatus.OK, smartAgentId);
            }

            catch (Exception ex)
            {
                return new PutResponse(PutResponse.ResponseStatus.NotFound, ex.Message);
            }
        }


        [UriFormat("/getMachineStateHistoryData/{lastHistoryNumber}")]
        public IGetResponse GetMachineStateHistoryData(int lastHistoryNumber)
        {
            try
            {
                var historyData = DataAccess.GetMachineStateHistoryData(lastHistoryNumber);

               

                return new GetResponse(GetResponse.ResponseStatus.OK, historyData);
            }
            catch(Exception ex)
            {
                return new GetResponse(GetResponse.ResponseStatus.NotFound, ex.Message);
            }
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
