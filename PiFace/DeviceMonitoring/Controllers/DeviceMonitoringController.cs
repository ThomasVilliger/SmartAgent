using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Internal;

namespace DeviceMonitoring.Controllers
{
    public class DeviceMonitoringController : Controller
    {
        private readonly IHubContext<MonitoringHub> hubConnection;

   
        public DeviceMonitoringController(IHubContext<MonitoringHub> hubContext)
        {
            hubConnection = hubContext;
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
        [HttpPost]
      //  [ValidateAntiForgeryToken]
        public ActionResult UpdateViewInputs(IFormCollection collection)
        {
            try
            {
                int pinNumber = Convert.ToInt32(collection.FirstOrDefault(m => m.Key=="PinNumber").Value);
                bool state = Convert.ToBoolean(collection.FirstOrDefault(m => m.Key == "State").Value);

         


                     object parameters = new Parameters { PinNumber = pinNumber, State = state };

                //Clients.All.UpdateInput(pinNumber, state); old

        
        

                hubConnection.Clients.All.InvokeAsync("UpdateInputState", parameters);


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();

            }
        }


        // POST: DeviceIOcontroller/Create
        [HttpPost]
    //    [ValidateAntiForgeryToken]
        public ActionResult UpdateViewOutputs(IFormCollection collection)
        {
            try
            {
                int pinNumber = Convert.ToInt32(collection.FirstOrDefault(m => m.Key == "PinNumber").Value);
                bool state = Convert.ToBoolean(collection.FirstOrDefault(m => m.Key == "State").Value);




                object parameters = new Parameters { PinNumber = pinNumber, State = state };

                //Clients.All.UpdateInput(pinNumber, state); old




                hubConnection.Clients.All.InvokeAsync("UpdateOutputState", parameters);


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();

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