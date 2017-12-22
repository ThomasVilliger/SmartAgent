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
    public class CycleMachineConfigurationsController : Controller
    {
        private readonly SmartDataHubStorageContext _context;

        public CycleMachineConfigurationsController(SmartDataHubStorageContext context)
        {

            _context = context;
        }

        // GET: CycleMachineConfigurations
        public async Task<IActionResult> Index()
        {
            var smartDataHubStorageContext = _context.CycleMachineConfiguration.Include(c => c.SmartAgent);
            return View(await smartDataHubStorageContext.ToListAsync());
        }

        // GET: CycleMachineConfigurations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cycleMachineConfiguration = await _context.CycleMachineConfiguration
                .Include(c => c.SmartAgent)
                .SingleOrDefaultAsync(m => m.CycleMachineConfigurationId == id);
            if (cycleMachineConfiguration == null)
            {
                return NotFound();
            }

            return View(cycleMachineConfiguration);
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
        public async Task<IActionResult> Create([Bind("CycleMachineConfigurationId,SmartAgentId,MachineName,CycleInputPin,MachineStateTimeOut,PublishingIntervall,Active")] CycleMachineConfiguration cycleMachineConfiguration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cycleMachineConfiguration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "Name", cycleMachineConfiguration.SmartAgentId); // Change IpAddress TO Name!!!!
            return View(cycleMachineConfiguration);
        }

        // GET: CycleMachineConfigurations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cycleMachineConfiguration = await _context.CycleMachineConfiguration.SingleOrDefaultAsync(m => m.CycleMachineConfigurationId == id);
            if (cycleMachineConfiguration == null)
            {
                return NotFound();
            }
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "Name", cycleMachineConfiguration.SmartAgentId); // Change IpAddress TO Name!!!!
            return View(cycleMachineConfiguration);
        }

        // POST: CycleMachineConfigurations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CycleMachineConfigurationId,SmartAgentId,MachineName,CycleInputPin,MachineStateTimeOut,PublishingIntervall,Active")] CycleMachineConfiguration cycleMachineConfiguration)
        {
            if (id != cycleMachineConfiguration.CycleMachineConfigurationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cycleMachineConfiguration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CycleMachineConfigurationExists(cycleMachineConfiguration.CycleMachineConfigurationId))
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
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "Name", cycleMachineConfiguration.SmartAgentId); // Change IpAddress TO Name!!!!
            return View(cycleMachineConfiguration);
        }

        // GET: CycleMachineConfigurations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cycleMachineConfiguration = await _context.CycleMachineConfiguration
                .Include(c => c.SmartAgent)
                .SingleOrDefaultAsync(m => m.CycleMachineConfigurationId == id);
            if (cycleMachineConfiguration == null)
            {
                return NotFound();
            }

            return View(cycleMachineConfiguration);
        }

        // POST: CycleMachineConfigurations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cycleMachineConfiguration = await _context.CycleMachineConfiguration.SingleOrDefaultAsync(m => m.CycleMachineConfigurationId == id);
            _context.CycleMachineConfiguration.Remove(cycleMachineConfiguration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CycleMachineConfigurationExists(int id)
        {
            return _context.CycleMachineConfiguration.Any(e => e.CycleMachineConfigurationId == id);
        }
    }
}
