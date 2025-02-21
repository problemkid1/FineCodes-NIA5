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
    public class MembershipTypeController : Controller
    {
        private readonly CRMContext _context;

        public MembershipTypeController(CRMContext context)
        {
            _context = context;
        }

        // GET: MembershipType
        public async Task<IActionResult> Index(string? MembershipTypeName, string? MembershipTypeDescription, string? MembershipTypeFee)
        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            var membershipTypes = _context.MembershipTypes
                .Include(mt => mt.MemberMembershipTypes)
                .AsNoTracking();

            // Create a SelectList for MembershipTypeName enum to be used in the dropdown
            ViewData["MembershipTypeNameList"] = new SelectList(Enum.GetValues(typeof(MembershipTypeName)), MembershipTypeName);

            // Filter by Membership Type Name (if selected from dropdown)
            if (!string.IsNullOrEmpty(MembershipTypeName))
            {
                membershipTypes = membershipTypes.Where(mt => mt.MembershipTypeName.ToString()
                                                               .ToLower()
                                                               .Contains(MembershipTypeName.ToLower()));
                numberFilters++;
            }

            // Filter by Membership Type Description
            if (!string.IsNullOrEmpty(MembershipTypeDescription))
            {
                membershipTypes = membershipTypes.Where(mt => mt.MembershipTypeDescription != null && mt.MembershipTypeDescription.ToLower().Contains(MembershipTypeDescription.ToLower()));
                numberFilters++;
            }

            // Filter by Membership Type Fee as a string for flexible matching
            if (!string.IsNullOrEmpty(MembershipTypeFee))
            {
                membershipTypes = membershipTypes.Where(mt => mt.MembershipTypeFee.HasValue &&
                    mt.MembershipTypeFee.Value.ToString().Contains(MembershipTypeFee.ToLower()));
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


            return View(await membershipTypes.ToListAsync());
        }

        // GET: MembershipType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipTypes
                 .Include(i => i.MemberMembershipTypes).ThenInclude(i => i.Member)
                .FirstOrDefaultAsync(m => m.ID == id);
                        
            if (membershipType == null)
            {
                return NotFound();
            }

            return View(membershipType);
        }

        // GET: MembershipType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MembershipType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MembershipTypeName,MembershipTypeDescription,MembershipTypeFee,MembershipTypeBenefits")] MembershipType membershipType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(membershipType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(membershipType);
        }

        // GET: MembershipType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipTypes.FindAsync(id);
            if (membershipType == null)
            {
                return NotFound();
            }
            return View(membershipType);
        }

        // POST: MembershipType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MembershipTypeName,MembershipTypeDescription,MembershipTypeFee,MembershipTypeBenefits")] MembershipType membershipType)
        {
            if (id != membershipType.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membershipType);
                    await _context.SaveChangesAsync();// Set success message in TempData
                    TempData["SuccessMessage"] = "Membership Type details updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = membershipType.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipTypeExists(membershipType.ID))
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

            return View(membershipType); // Return to the edit view if there are validation errors

        }

        // GET: MembershipType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipTypes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (membershipType == null)
            {
                return NotFound();
            }

            return View(membershipType);
        }

        // POST: MembershipType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membershipType = await _context.MembershipTypes.FindAsync(id);
            if (membershipType != null)
            {
                _context.MembershipTypes.Remove(membershipType);
                await _context.SaveChangesAsync();

                // Set success message in TempData
                TempData["SuccessMessage"] = "Membership Type deleted successfully!";
            }
            else
            {
                // If contact not found, set an error message
                TempData["ErrorMessage"] = "Membership Type not found!";
            }

            // Redirect to the Index or other appropriate page
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipTypeExists(int id)
        {
            return _context.MembershipTypes.Any(e => e.ID == id);
        }
    }
}
