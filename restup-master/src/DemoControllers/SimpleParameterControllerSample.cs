using Restup.DemoControllers.Model;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System.Collections.Generic;
using System.Linq;
using PIFace_II;
using Windows.Data.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

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


        [UriFormat("/getMachineHistory/{machineId}")]
        public IGetResponse GetMachineHistory(int machineId)
        {
            machineStateHistory = PiFaceMain.cycleMachines[machineId].MachineStateHistory;

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

        [UriFormat("/writePiFaceInput/{pinNumber}/value/{value}")]
        public IGetResponse GetWritePiFaceInput(int pinNumber, bool value)
        {
            PiFaceMain.WritePiFaceInput(pinNumber, value);
            return new GetResponse(GetResponse.ResponseStatus.OK);
        }




        [UriFormat("/getCurrentMachineData/{machineId}")]
        public IGetResponse GetCurrentMachineData(int machineId)
        {
            CycleMachine cycleMachine = PiFaceMain.cycleMachines[machineId];

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                new CurrentMachineData()
                {
                    MachineState = cycleMachine.CurrentMachineState.ToString(),
                    CycleCounterPerMachineState = cycleMachine.CycleCounterPerMachineState,
                    DailyCycleCunter = cycleMachine.DailyCycleCounter,
                    LastCycleTime = cycleMachine.LastCycleTime
                    
                });
        }






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
