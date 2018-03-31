using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartAgentMonitoring.Controllers
{
    public class GatewayController : Controller
    {
        // GET: Gateway
        public ActionResult Index()
        {
            var hostName = HttpContext.Request.Host.ToString().Split(":");
            ViewData["DevicePortalWebsite"] = "http://" + hostName[0] + ":8080";
            return View();
        }
    }
}