using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Net;

namespace DeviceMonitoring.Controllers
{
    // This is the conroller for the SmartAgent IO Monitoring
    public class DeviceMonitoringController : Controller
    {
        private readonly IHubContext<MonitoringHub> _hubContext;

        public DeviceMonitoringController(IHubContext<MonitoringHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public ActionResult Index()
        {
            var hostName = HttpContext.Request.Host.ToString().Split(":");
            ViewData["DevicePortalWebsite"] = "http://" + hostName[0] + ":8080";
            return View();
        }

        [HttpPut]
        public HttpResponseMessage UpdateSingleInputState([FromBody] PinState pinState)
        {
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

        [HttpPut]
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
    }
}