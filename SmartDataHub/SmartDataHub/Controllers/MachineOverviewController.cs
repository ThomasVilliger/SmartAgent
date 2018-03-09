using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartDataHub.Models;

namespace SmartDataHub.Controllers
{
    public class MachineOverviewController : Controller
    {
        private readonly SmartDataHubStorageContext _context;

        public MachineOverviewController(SmartDataHubStorageContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var smartDataHubStorageContext = _context.Machine.Include(c => c.SmartAgent);
            return View(await smartDataHubStorageContext.ToListAsync());
        }

        private bool MachineExists(int id)
        {
            return _context.Machine.Any(e => e.MachineId == id);
        }
    }
}
