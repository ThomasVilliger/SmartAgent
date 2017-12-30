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

        // GET: CycleMachineConfigurations
        public async Task<IActionResult> Index()
        {
            var smartDataHubStorageContext = _context.Machine.Include(c => c.SmartAgent);
            return View(await smartDataHubStorageContext.ToListAsync());
        }

        // GET: CycleMachineConfigurations/Details/5
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

        // GET: CycleMachineConfigurations/Create
        public IActionResult Create()
        {
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "Name"); // Change IpAddress TO Name!!!!
            return View();
        }

        // POST: CycleMachineConfigurations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: CycleMachineConfigurations/Edit/5
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

        // POST: CycleMachineConfigurations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: CycleMachineConfigurations/Delete/5
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

        // POST: CycleMachineConfigurations/Delete/5
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
