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
            return View();
        }
    }
}