using Restup.SmartAgent.Model;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System.Collections.Generic;
using System.Linq;
using SmartAgent;
using Windows.Data.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using System;
using DataAccess;
using Newtonsoft.Json;

using System.Collections.ObjectModel;
using System.Net.Http;
using System.Collections.Immutable;


namespace Restup.SmartAgent
{
    // this class contains all the RESTful methods of this  Server (SmartAgent IO Monitoring, SmartAgent Configuration)
    [RestController(InstanceCreationType.Singleton)]
    public sealed class SmartAgentRestController
    {
        private SmartAgentMain _piFaceMain { get; set; }
        private List<MachineStateHistory> _machineStateHistory;
        public SmartAgentRestController(SmartAgentMain piFaceMain)
        {
            _piFaceMain = piFaceMain;
        }

        [UriFormat("/setDeviceInput/")]
        public IPutResponse SetDeviceInput([FromContent] Dictionary<string, string> parameters)
        {
            int pinNumber = Convert.ToInt32(parameters.FirstOrDefault(m => m.Key == "PinNumber").Value);
            bool state = Convert.ToBoolean(parameters.FirstOrDefault(m => m.Key == "State").Value);

            _piFaceMain.Device.SetDeviceInput(pinNumber, state);
            return new PutResponse(PutResponse.ResponseStatus.OK);
        }

        [UriFormat("/setDeviceOutput/")]
        public IPutResponse SetDeviceOutput([FromContent] Dictionary<string, string> parameters)
        {
            int pinNumber = Convert.ToInt32(parameters.FirstOrDefault(m => m.Key == "PinNumber").Value);
            bool state = Convert.ToBoolean(parameters.FirstOrDefault(m => m.Key == "State").Value);

            _piFaceMain.Device.SetDeviceOutput(pinNumber, state);
            return new PutResponse(PutResponse.ResponseStatus.OK);
        }

        [UriFormat("/initializeNewMachines/")]
        public IPutResponse InitializeNewMachines([FromContent] List<MachineConfiguration> configs)
        //public IPutResponse PutInitializeNewMachineConfigurations()
        {
            try
            {
                DataAccess.DataAccess.StoreMachinesConfigurations(configs);
                return new PutResponse(PutResponse.ResponseStatus.OK, configs);
            }

            catch (Exception ex)
            {
                return new PutResponse(PutResponse.ResponseStatus.NotFound, ex.Message);
            }
        }

        [UriFormat("/initializeNewInputMonitorings/")]
        public IPutResponse InitializeNewInputMonitorings([FromContent] List<InputMonitoringConfiguration> configs)
        {
            try
            {
                DataAccess.DataAccess.StoreInputMonitoringConfigurations(configs);
                return new PutResponse(PutResponse.ResponseStatus.OK, configs);
            }

            catch (Exception ex)
            {
                return new PutResponse(PutResponse.ResponseStatus.NotFound, ex.Message);
            }
        }

        [UriFormat("/registerSmartAgents/")]
        public IPutResponse SetSmartAgents([FromContent] List<DataAccess.SmartAgent> configs)
        {
            try
            {
                DataAccess.DataAccess.StoreSmartAgents(configs);
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
                DataAccess.DataAccess.SignSmartAgent(smartAgentId);
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
                var historyData = DataAccess.DataAccess.GetMachineStateHistoryData(lastHistoryNumber);
                return new GetResponse(GetResponse.ResponseStatus.OK, historyData);
            }
            catch (Exception ex)
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
            int numberOfInputs = _piFaceMain.Device.NumberOfInputs;
            deviceInputStates.Add("NumberOfInputs", numberOfInputs.ToString());

            foreach (IInput input in _piFaceMain.Device.Inputs)
            {
                deviceInputStates.Add(input.PinNumber.ToString(), input.State.ToString());
            }
            return new GetResponse(GetResponse.ResponseStatus.OK, deviceInputStates);
        }

        [UriFormat("/getAllDeviceOutputStates/")]
        public IGetResponse GetAllDeviceOutputStates()
        {
            var deviceOutputStates = new Dictionary<string, string>();
            int numberOfOutputs = _piFaceMain.Device.NumberOfOutputs;
            deviceOutputStates.Add("NumberOfOutputs", numberOfOutputs.ToString());

            foreach (IOutput output in _piFaceMain.Device.Outputs)
            {
                deviceOutputStates.Add(output.PinNumber.ToString(), output.State.ToString());

            }
            return new GetResponse(GetResponse.ResponseStatus.OK, deviceOutputStates);
        }
    }
}
