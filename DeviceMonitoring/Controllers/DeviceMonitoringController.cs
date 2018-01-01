using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Internal;
using System.Net.Http;
using System.Net;

namespace DeviceMonitoring.Controllers
{
    public class DeviceMonitoringController : Controller
    {
        private readonly IHubContext<MonitoringHub> _hubContext;
       

   
        public DeviceMonitoringController(IHubContext<MonitoringHub> hubContext)
        {      
           _hubContext = hubContext;
        }


        // GET: DeviceIOcontroller
        public ActionResult Index()
        {
            return View();
        }

        // GET: DeviceIOcontroller/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DeviceIOcontroller/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeviceIOcontroller/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }




        // POST: DeviceIOcontroller/Create
        [HttpPut]
      //  [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateSingleInputState([FromBody] PinState pinState)
        {
            //try
            //{
            //    int pinNumber = Convert.ToInt32(collection.FirstOrDefault(m => m.Key=="PinNumber").Value);
            //    bool state = Convert.ToBoolean(collection.FirstOrDefault(m => m.Key == "State").Value);




            //         object pinState = new PinState { PinNumber = pinNumber, State = state };

            //    //Clients.All.UpdateInput(pinNumber, state); old


            try
            {

                _hubContext.Clients.All.InvokeAsync("UpdateSingleInputState", pinState);
                return new HttpResponseMessage(HttpStatusCode.OK);

            }

            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }

              
        }






        //// POST: DeviceIOcontroller/Create
        //[HttpPost]
        ////  [ValidateAntiForgeryToken]
        //public ActionResult PublishActualCycleMachineData(IFormCollection collection)
        //{
        //    try
        //    {
        //    Dictionary<string, string> actualCycleMachineData = new Dictionary<string, string>();

        //    var form = new Dictionary<string, string>();
        //        foreach (var key in collection.Keys)
        //        {
        //            var value = collection[key];
        //            actualCycleMachineData.Add(key.ToString(), value.ToString());
        //        }




        //        _hubContext.Clients.All.InvokeAsync("PublishActualCycleMachineData", actualCycleMachineData);


        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();

        //    }
        //}













        // POST: DeviceIOcontroller/Create
        [HttpPut]
    //    [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateSingleOutputState([FromBody] PinState pinState)
        {
            try
            {
                _hubContext.Clients.All.InvokeAsync("UpdateSingleOutputState", pinState);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }
        }


        // GET: DeviceIOcontroller/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DeviceIOcontroller/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DeviceIOcontroller/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DeviceIOcontroller/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}