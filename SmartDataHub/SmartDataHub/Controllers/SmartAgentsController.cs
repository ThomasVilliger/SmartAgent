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
    public class SmartAgentsController : Controller
    {
        private readonly SmartDataHubStorageContext _context;

        public SmartAgentsController(SmartDataHubStorageContext context)
        {
            _context = context;
        }

        // GET: SmartAgents
        public async Task<IActionResult> Index()
        {
            return View(await _context.SmartAgent.ToListAsync());
        }

        // GET: SmartAgents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smartAgent = await _context.SmartAgent
                .SingleOrDefaultAsync(m => m.SmartAgentId == id);
            if (smartAgent == null)
            {
                return NotFound();
            }

            return View(smartAgent);
        }

        // GET: SmartAgents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SmartAgents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SmartAgentId,Name,IpAddress")] SmartAgent smartAgent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(smartAgent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(smartAgent);
        }

        // GET: SmartAgents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smartAgent = await _context.SmartAgent.SingleOrDefaultAsync(m => m.SmartAgentId == id);
            if (smartAgent == null)
            {
                return NotFound();
            }
            return View(smartAgent);
        }

        // POST: SmartAgents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SmartAgentId,Name,IpAddress")] SmartAgent smartAgent)
        {
            if (id != smartAgent.SmartAgentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(smartAgent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SmartAgentExists(smartAgent.SmartAgentId))
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
            return View(smartAgent);
        }

        // GET: SmartAgents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smartAgent = await _context.SmartAgent
                .SingleOrDefaultAsync(m => m.SmartAgentId == id);
            if (smartAgent == null)
            {
                return NotFound();
            }

            return View(smartAgent);
        }

        // POST: SmartAgents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var smartAgent = await _context.SmartAgent.SingleOrDefaultAsync(m => m.SmartAgentId == id);
            _context.SmartAgent.Remove(smartAgent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SmartAgentExists(int id)
        {
            return _context.SmartAgent.Any(e => e.SmartAgentId == id);
        }
    }
}
