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
    public class MachinesController : Controller
    {
        private readonly SmartDataHubStorageContext _context;

        public MachinesController(SmartDataHubStorageContext context)
        {

            _context = context;
        }
       
        public async Task<IActionResult> Index()
        {
            var smartDataHubStorageContext = _context.Machine.Include(c => c.SmartAgent);
            return View(await smartDataHubStorageContext.ToListAsync());
        }

     
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machine = await _context.Machine
                .Include(c => c.SmartAgent)
                .SingleOrDefaultAsync(m => m.MachineId == id);
            if (machine == null)
            {
                return NotFound();
            }

            return View(machine);
        }

        public IActionResult Create()
        {
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "Name"); // Change IpAddress TO Name!!!!
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MachineId,SmartAgentId,MachineName,CycleInputPin,MachineStateTimeOut,PublishingIntervall,Active")] Machine machine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(machine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "Name", machine.SmartAgentId); // Change IpAddress TO Name!!!!
            return View(machine);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machine = await _context.Machine.SingleOrDefaultAsync(m => m.MachineId == id);
            if (machine == null)
            {
                return NotFound();
            }
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "Name", machine.SmartAgentId); // Change IpAddress TO Name!!!!
            return View(machine);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MachineId,SmartAgentId,MachineName,CycleInputPin,MachineStateTimeOut,PublishingIntervall,Active")] Machine machine)
        {
            if (id != machine.MachineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(machine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineExists(machine.MachineId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "Name", machine.SmartAgentId); // Change IpAddress TO Name!!!!
            return View(machine);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machine = await _context.Machine
                .Include(c => c.SmartAgent)
                .SingleOrDefaultAsync(m => m.MachineId == id);
            if (machine == null)
            {
                return NotFound();
            }

            return View(machine);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machine = await _context.Machine.SingleOrDefaultAsync(m => m.MachineId == id);
            _context.Machine.Remove(machine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MachineExists(int id)
        {
            return _context.Machine.Any(e => e.MachineId == id);
        }
    }
}
