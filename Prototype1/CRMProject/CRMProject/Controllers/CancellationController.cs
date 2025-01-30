using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRMProject.Data;
using CRMProject.Models;

namespace CRMProject.Controllers
{
    public class CancellationController : Controller
    {
        private readonly CRMContext _context;

        public CancellationController(CRMContext context)
        {
            _context = context;
        }

        // GET: Cancellation
        public async Task<IActionResult> Index()
        {
            var cRMContext = _context.Cancellations.Include(c => c.Member);
            return View(await cRMContext.ToListAsync());
        }

        // GET: Cancellation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cancellation = await _context.Cancellations
                .Include(c => c.Member)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cancellation == null)
            {
                return NotFound();
            }

            return View(cancellation);
        }

        // GET: Cancellation/Create
        public IActionResult Create()
        {
            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberAccountsPayableEmail");
            return View();
        }

        // POST: Cancellation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CancellationDate,CancellationReason,CancellationNotes,MemberID")] Cancellation cancellation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cancellation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberAccountsPayableEmail", cancellation.MemberID);
            return View(cancellation);
        }

        // GET: Cancellation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cancellation = await _context.Cancellations.FindAsync(id);
            if (cancellation == null)
            {
                return NotFound();
            }
            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberAccountsPayableEmail", cancellation.MemberID);
            return View(cancellation);
        }

        // POST: Cancellation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CancellationDate,CancellationReason,CancellationNotes,MemberID")] Cancellation cancellation)
        {
            if (id != cancellation.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cancellation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CancellationExists(cancellation.ID))
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
            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberAccountsPayableEmail", cancellation.MemberID);
            return View(cancellation);
        }

        // GET: Cancellation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cancellation = await _context.Cancellations
                .Include(c => c.Member)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cancellation == null)
            {
                return NotFound();
            }

            return View(cancellation);
        }

        // POST: Cancellation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cancellation = await _context.Cancellations.FindAsync(id);
            if (cancellation != null)
            {
                _context.Cancellations.Remove(cancellation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CancellationExists(int id)
        {
            return _context.Cancellations.Any(e => e.ID == id);
        }
    }
}
