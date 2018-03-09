using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartDataHub.Models;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace SmartDataHub.Controllers
{
    public class ReportController : Controller
    {
        private readonly SmartDataHubStorageContext _context;

        public ReportController(SmartDataHubStorageContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            ViewData["MachineId"] = new SelectList(_context.Machine, "MachineId", "MachineName");

            return View(new Machine());
          }
    }
}