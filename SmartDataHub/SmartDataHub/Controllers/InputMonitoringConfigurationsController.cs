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
    public class InputMonitoringConfigurationsController : Controller
    {
        private readonly SmartDataHubStorageContext _context;

        public InputMonitoringConfigurationsController(SmartDataHubStorageContext context)
        {
            _context = context;
        }

        // GET: InputMonitoringConfigurations
        public async Task<IActionResult> Index()
        {
            var smartDataHubStorageContext = _context.InputMonitoringConfiguration.Include(i => i.SmartAgent);
            return View(await smartDataHubStorageContext.ToListAsync());
        }

        // GET: InputMonitoringConfigurations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inputMonitoringConfiguration = await _context.InputMonitoringConfiguration
                .Include(i => i.SmartAgent)
                .SingleOrDefaultAsync(m => m.InputMonitoringConfigurationId == id);
            if (inputMonitoringConfiguration == null)
            {
                return NotFound();
            }

            return View(inputMonitoringConfiguration);
        }

        // GET: InputMonitoringConfigurations/Create
        public IActionResult Create()
        {
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "IpAddress");
            return View();
        }

        // POST: InputMonitoringConfigurations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InputMonitoringConfigurationId,SmartAgentId,MonitoringName,InputPin,OutputPin,Active")] InputMonitoringConfiguration inputMonitoringConfiguration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inputMonitoringConfiguration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "IpAddress", inputMonitoringConfiguration.SmartAgentId);
            return View(inputMonitoringConfiguration);
        }

        // GET: InputMonitoringConfigurations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inputMonitoringConfiguration = await _context.InputMonitoringConfiguration.SingleOrDefaultAsync(m => m.InputMonitoringConfigurationId == id);
            if (inputMonitoringConfiguration == null)
            {
                return NotFound();
            }
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "IpAddress", inputMonitoringConfiguration.SmartAgentId);
            return View(inputMonitoringConfiguration);
        }

        // POST: InputMonitoringConfigurations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InputMonitoringConfigurationId,SmartAgentId,MonitoringName,InputPin,OutputPin,Active")] InputMonitoringConfiguration inputMonitoringConfiguration)
        {
            if (id != inputMonitoringConfiguration.InputMonitoringConfigurationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inputMonitoringConfiguration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InputMonitoringConfigurationExists(inputMonitoringConfiguration.InputMonitoringConfigurationId))
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
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "IpAddress", inputMonitoringConfiguration.SmartAgentId);
            return View(inputMonitoringConfiguration);
        }

        // GET: InputMonitoringConfigurations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inputMonitoringConfiguration = await _context.InputMonitoringConfiguration
                .Include(i => i.SmartAgent)
                .SingleOrDefaultAsync(m => m.InputMonitoringConfigurationId == id);
            if (inputMonitoringConfiguration == null)
            {
                return NotFound();
            }

            return View(inputMonitoringConfiguration);
        }

        // POST: InputMonitoringConfigurations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inputMonitoringConfiguration = await _context.InputMonitoringConfiguration.SingleOrDefaultAsync(m => m.InputMonitoringConfigurationId == id);
            _context.InputMonitoringConfiguration.Remove(inputMonitoringConfiguration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InputMonitoringConfigurationExists(int id)
        {
            return _context.InputMonitoringConfiguration.Any(e => e.InputMonitoringConfigurationId == id);
        }
    }
}
