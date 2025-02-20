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
    public class IndustryController : Controller
    {
        private readonly CRMContext _context;

        public IndustryController(CRMContext context)
        {
            _context = context;
        }

        // GET: Industry
        public async Task<IActionResult> Index(string? IndustrySector, string? IndustrySubsector, string? IndustryNAICSCode )
        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            // Include related MemberIndustries and Member data for each Industry
            var industries = _context.Industries
                .Include(i => i.MemberIndustries)
                .ThenInclude(i => i.Member)
                .AsNoTracking(); // Eager loading with .Include and .ThenInclude

            // Filter by Industry Sector
            if (!string.IsNullOrEmpty(IndustrySector))
            {
                industries = industries.Where(i => i.IndustrySector.ToLower().Contains(IndustrySector.ToLower()));
                numberFilters++;
            }

            // Filter by Industry NAICS Code
            if (!string.IsNullOrEmpty(IndustryNAICSCode))
            {
                industries = industries.Where(i => i.IndustryNAICSCode.Contains(IndustryNAICSCode));
                numberFilters++;
            }

            // Filter by Industry Subsector
            if (!string.IsNullOrEmpty(IndustrySubsector))
            {
                industries = industries.Where(i => i.IndustrySubsector.ToLower().Contains(IndustrySubsector.ToLower()));
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

            // Return the filtered list of industries with the related MemberIndustries and Member data
            return View(await industries.ToListAsync());
        }



        // GET: Industry/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var industry = await _context.Industries
                     .Include(i => i.MemberIndustries).ThenInclude(i => i.Member)
                    .FirstOrDefaultAsync(m => m.ID == id);

                if (industry == null)
                {
                    return NotFound();
                }

                return View(industry);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while retrieving the industry details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Industry/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Industry/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,IndustrySector,IndustrySubsector,IndustryNAICSCode")] Industry industry)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check for unique NAICS Code before adding
                    var existingIndustry = await _context.Industries
                        .FirstOrDefaultAsync(i => i.IndustryNAICSCode == industry.IndustryNAICSCode);

                    if (existingIndustry != null)
                    {
                        // Return a validation error if the NAICS code already exists
                        ModelState.AddModelError("IndustryNAICSCode", "Industry with this NAICS Code already exists.");
                        return View(industry);
                    }

                    // Add the new member to the context and save changes
                    _context.Add(industry);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Industry created successfully!";
                    return RedirectToAction(nameof(Details), new { id = industry.ID });
                }
                catch (DbUpdateException dex)
                {
                    // Directly handling the exception without ExceptionMessageVM
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("IndustryNAICSCode", "Unable to save changes. Remember, Industry NAICS Code must be unique.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while saving the Industry.";
                    }
                }
                catch (Exception)
                {
                    // General error handling
                    TempData["ErrorMessage"] = "An error occurred while creating the Industry.";
                }
            }
            else
            {
                // If model validation fails, set an error message
                TempData["ErrorMessage"] = "Please check the input data and try again.";
            }

            // Return to the Create view in case of failure or validation errors
            return View(industry);
        }

        // GET: Industry/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var industry = await _context.Industries.FindAsync(id);
                if (industry == null)
                {
                    return NotFound();
                }

                return View(industry);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while fetching the industry for editing: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Industry/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,IndustrySector,IndustrySubsector,IndustryNAICSCode")] Industry industry)
        {
            if (id != industry.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check for unique NAICS Code before updating
                    var existingIndustry = await _context.Industries
                        .FirstOrDefaultAsync(i => i.IndustryNAICSCode == industry.IndustryNAICSCode && i.ID != industry.ID);

                    if (existingIndustry != null)
                    {
                        // Return a validation error if the NAICS code already exists
                        ModelState.AddModelError("IndustryNAICSCode", "Industry with this NAICS Code already exists.");
                        return View(industry);
                    }

                    _context.Update(industry);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Industry details updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = industry.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IndustryExists(industry.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {
                    // Directly handling the exception without ExceptionMessageVM
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("IndustryNAICSCode", "Unable to save changes. Remember, Industry NAICS Code must be unique.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while saving the Industry.";
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the industry details.";
                }
            }

            TempData["ErrorMessage"] = "Please check the input data and try again.";
            return View(industry); // Return to the edit view if there are validation errors
        }

        // GET: Industry/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var industry = await _context.Industries
                    .Include(i => i.MemberIndustries).ThenInclude(i => i.Member)
                    .FirstOrDefaultAsync(m => m.ID == id);

                if (industry == null)
                {
                    return NotFound();
                }

                return View(industry);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while fetching the industry for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Industry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var industry = await _context.Industries
               .Include(i => i.MemberIndustries).ThenInclude(i => i.Member)
               .FirstOrDefaultAsync(i => i.ID == id);
            if (industry != null)
            {
                _context.Industries.Remove(industry);
                await _context.SaveChangesAsync();

                // Set success message in TempData
                TempData["SuccessMessage"] = "Industry deleted successfully!";
            }
            else
            {
                // If Insudtry not found, set an error message
                TempData["ErrorMessage"] = "Industry not found!";
            }

            // Redirect to the Index or other appropriate page
            return RedirectToAction(nameof(Index));
        }

        private bool IndustryExists(int id)
        {
            return _context.Industries.Any(e => e.ID == id);
        }
    }
}
