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
    public class InputMonitoringController : Controller
    {
        private readonly SmartDataHubStorageContext _context;

        public InputMonitoringController(SmartDataHubStorageContext context)
        {
            _context = context;
        }

        // GET: InputMonitoringConfigurations
        public async Task<IActionResult> Index()
        {
            var smartDataHubStorageContext = _context.InputMonitoring.Include(i => i.SmartAgent);
            return View(await smartDataHubStorageContext.ToListAsync());
        }

        // GET: InputMonitoringConfigurations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inputMonitoring = await _context.InputMonitoring
                .Include(i => i.SmartAgent)
                .SingleOrDefaultAsync(m => m.InputMonitoringId == id);
            if (inputMonitoring == null)
            {
                return NotFound();
            }

            return View(inputMonitoring);
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
        public async Task<IActionResult> Create([Bind("InputMonitoringId,SmartAgentId,MonitoringName,InputPin,OutputPin,Active")] InputMonitoring inputMonitoring)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inputMonitoring);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "IpAddress", inputMonitoring.SmartAgentId);
            return View(inputMonitoring);
        }

        // GET: InputMonitoringConfigurations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inputMonitoring = await _context.InputMonitoring.SingleOrDefaultAsync(m => m.InputMonitoringId == id);
            if (inputMonitoring == null)
            {
                return NotFound();
            }
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "IpAddress", inputMonitoring.SmartAgentId);
            return View(inputMonitoring);
        }

        // POST: InputMonitoringConfigurations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InputMonitoringId,SmartAgentId,MonitoringName,InputPin,OutputPin,Active")] InputMonitoring inputMonitoring)
        {
            if (id != inputMonitoring.InputMonitoringId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inputMonitoring);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InputMonitoringExists(inputMonitoring.InputMonitoringId))
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
            ViewData["SmartAgentId"] = new SelectList(_context.SmartAgent, "SmartAgentId", "IpAddress", inputMonitoring.SmartAgentId);
            return View(inputMonitoring);
        }

        // GET: InputMonitoringConfigurations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inputMonitoring = await _context.InputMonitoring
                .Include(i => i.SmartAgent)
                .SingleOrDefaultAsync(m => m.InputMonitoringId == id);
            if (inputMonitoring == null)
            {
                return NotFound();
            }

            return View(inputMonitoring);
        }

        // POST: InputMonitoringConfigurations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inputMonitoring = await _context.InputMonitoring.SingleOrDefaultAsync(m => m.InputMonitoringId == id);
            _context.InputMonitoring.Remove(inputMonitoring);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InputMonitoringExists(int id)
        {
            return _context.InputMonitoring.Any(e => e.InputMonitoringId == id);
        }
    }
}
