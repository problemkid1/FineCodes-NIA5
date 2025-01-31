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
    public class OpportunityController : Controller
    {
        private readonly CRMContext _context;

        public OpportunityController(CRMContext context)
        {
            _context = context;
        }

        // GET: Opportunity
        public async Task<IActionResult> Index(string? OpportunityName, OpportunityStatus? OpportunityStatus, string? OpportunityPriority)
        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            var opportunities = _context.Opportunities
                .AsNoTracking();

            // Filter by Opportunity Name
            if (!string.IsNullOrEmpty(OpportunityName))
            {
                opportunities = opportunities.Where(o => o.OpportunityName.ToLower().Contains(OpportunityName.ToLower()));
                numberFilters++;
            }

            // Filter by Opportunity Status
            if (OpportunityStatus.HasValue)
            {
                opportunities = opportunities.Where(o => o.OpportunityStatus == OpportunityStatus);
                numberFilters++;
            }

            // Filter by Opportunity Priority
            if (!string.IsNullOrEmpty(OpportunityPriority))
            {
                opportunities = opportunities.Where(o => o.OpportunityPriority.ToLower().Contains(OpportunityPriority.ToLower()));
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

            return View(await opportunities.ToListAsync());
        }


        // GET: Opportunity/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(m => m.ID == id);
            if (opportunity == null)
            {
                return NotFound();
            }

            return View(opportunity);
        }

        // GET: Opportunity/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Opportunity/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,OpportunityName,OpportunityStatus,OpportunityPriority,OpportunityAction,OpportunityContact,OpportunityAccount,OpportunityLastContactDate,OpportunityInteractions")] Opportunity opportunity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Add the new member to the context and save changes
                    _context.Add(opportunity);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Opportunity created successfully!";
                    return RedirectToAction(nameof(Details), new { id = opportunity.ID });
                }
                catch (Exception)
                {
                    // Set error message in case of failure
                    TempData["ErrorMessage"] = "An error occurred while creating this Opportunity.";
                }
            }
            else
            {
                // If model validation fails, set an error message
                TempData["ErrorMessage"] = "Please check the input data and try again.";
            }

            // Return to the Create view in case of failure or validation errors
            return View(opportunity);
        }

        // GET: Opportunity/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity == null)
            {
                return NotFound();
            }
            return View(opportunity);
        }

        // POST: Opportunity/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,OpportunityName,OpportunityStatus,OpportunityPriority,OpportunityAction,OpportunityContact,OpportunityAccount,OpportunityLastContactDate,OpportunityInteractions")] Opportunity opportunity)
        {
            if (id != opportunity.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(opportunity);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Opportunity details updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = opportunity.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpportunityExists(opportunity.ID))
                    {
                        // If the Opportunity does not exist anymore, return NotFound
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
                    TempData["ErrorMessage"] = "An error occurred while updating the Opportunity details.";
                }

            }

            // Set error message in case the model is invalid
            TempData["ErrorMessage"] = "Please check the input data and try again.";

            return View(opportunity); // Return to the edit view if there are validation errors
        }

        // GET: Opportunity/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(m => m.ID == id);
            if (opportunity == null)
            {
                return NotFound();
            }

            return View(opportunity);
        }

        // POST: Opportunity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity != null)
            {
                _context.Opportunities.Remove(opportunity);
                await _context.SaveChangesAsync();

                // Set success message in TempData
                TempData["SuccessMessage"] = "Opportunity deleted successfully!";
            }
            else
            {
                // If Insudtry not found, set an error message
                TempData["ErrorMessage"] = "Opportunity not found!";
            }

            // Redirect to the Index or other appropriate page
            return RedirectToAction(nameof(Index));
        }

        private bool OpportunityExists(int id)
        {
            return _context.Opportunities.Any(e => e.ID == id);
        }
    }
}
