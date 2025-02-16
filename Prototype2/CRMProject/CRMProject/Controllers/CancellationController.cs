using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRMProject.Data;
using CRMProject.Models;
using System.Net;
using Microsoft.IdentityModel.Tokens;

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
        public async Task<IActionResult> Index(DateTime? CancellationDate, string? CancellationReason, string? CancellationNotes)
        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            var cRMContext = _context.Cancellations.Include(c => c.Member)
                .AsNoTracking(); // Eager loading the related Member entity

            // Filter by Cancellation Date
            if (CancellationDate.HasValue)
            {
                cRMContext = cRMContext.Where(c => c.CancellationDate == CancellationDate.Value);
                numberFilters++;
            }

            // Filter by Cancellation Reason
            if (!string.IsNullOrEmpty(CancellationReason))
            {
                cRMContext = cRMContext.Where(c => c.CancellationReason.ToLower().Contains(CancellationReason.ToLower()));
                numberFilters++;
            }

            // Filter by Cancellation Notes
            if (!string.IsNullOrEmpty(CancellationNotes))
            {
                cRMContext = cRMContext.Where(c => c.CancellationNotes.ToLower().Contains(CancellationNotes.ToLower()));
                numberFilters++;
            }

            // Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                // Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = "btn-danger";
                // Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString() + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
                // Keep the Bootstrap collapse open
                ViewData["ShowFilter"] = "show";
            }

            // Return the filtered list of cancellations with associated Member data
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
            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberName");
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
                try
                {
                    // Add the new cancellation to the context and save changes
                    _context.Add(cancellation);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Cancellation created successfully!";
                    return RedirectToAction(nameof(Details), new { id = cancellation.ID });
                }
                catch (Exception)
                {
                    // Set error message in case of failure
                    TempData["ErrorMessage"] = "An error occurred while creating cancellation.";
                }
            }
            else
            {
                // If model validation fails, set an error message
                TempData["ErrorMessage"] = "Please check the input data and try again.";
            }

            // Return to the Create view in case of failure or validation errors
            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberName", cancellation.MemberID);
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
            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberName", cancellation.MemberID);
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

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Cancellation updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = cancellation.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CancellationExists(cancellation.ID))
                    {
                        // If the cancellation does not exist anymore, return NotFound
                        return NotFound();
                    }
                    else
                    {
                        // Rethrow exception if there is a concurrency issue
                        throw;
                    }
                }
                catch (Exception)
                {
                    // Set error message for generic errors
                    TempData["ErrorMessage"] = "An error occurred while updating cancellation.";
                }

            }

            // Set error message in case the model is invalid
            TempData["ErrorMessage"] = "Please check the input data and try again.";

            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberName", cancellation.MemberID);
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
                await _context.SaveChangesAsync();

                // Set success message in TempData
                TempData["SuccessMessage"] = "Cancellation deleted successfully!";
            }
            else
            {
                // If Address not found, set an error message
                TempData["ErrorMessage"] = "Cancellation not found!";
            }

            // Redirect to the Index or other appropriate page
            return RedirectToAction(nameof(Index));
        }

        private bool CancellationExists(int id)
        {
            return _context.Cancellations.Any(e => e.ID == id);
        }
    }
}
