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
    public class AddressController : Controller
    {
        private readonly CRMContext _context;

        public AddressController(CRMContext context)
        {
            _context = context;
        }

        // GET: Address
        public async Task<IActionResult> Index(string? AddressCity, Province? Province, string? PostalCode)
        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            var addresses = _context.Addresses
                .Include(a => a.Member)
                .AsNoTracking();

            // Filter by City
            if (!string.IsNullOrEmpty(AddressCity))
            {
                addresses = addresses.Where(a => a.AddressCity.Contains(AddressCity));
                numberFilters++;
            }

            // Filter by Province
            if (Province.HasValue)
            {
                addresses = addresses.Where(a => a.Province == Province.Value);
                numberFilters++;
            }

            // Filter by Postal Code
            if (!string.IsNullOrEmpty(PostalCode))
            {
                addresses = addresses.Where(a => a.PostalCode.Contains(PostalCode));
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

            return View(await addresses.ToListAsync());
        }


        // GET: Address/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses
                .Include(a => a.Member)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // GET: Address/Create
        public IActionResult Create()
        {

            PopulateDropDownLists();
            return View();
        }

        // POST: Address/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,AddressLine1,AddressLine2,AddressCity,Province,PostalCode,AddressType,MemberID")] Address address)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Add the new member to the context and save changes
                    _context.Add(address);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Address created successfully!";
                    return RedirectToAction(nameof(Details), new { id = address.ID });
                }
                catch (Exception)
                {
                    // Set error message in case of failure
                    TempData["ErrorMessage"] = "An error occurred while creating the Addess.";
                }
            }
            else
            {
                // If model validation fails, set an error message
                TempData["ErrorMessage"] = "Please check the input data and try again.";
            }

            // Return to the Create view in case of failure or validation errors
            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberAccountsPayableEmail", address.MemberID);
            return View(address);
        }

        // GET: Address/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            PopulateDropDownLists();
            return View(address);
        }

        // POST: Address/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,AddressLine1,AddressLine2,AddressCity,Province,PostalCode,AddressType,MemberID")] Address address)
        {
            if (id != address.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(address);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Address updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = address.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.ID))
                    {
                        // If the industry does not exist anymore, return NotFound
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
                    TempData["ErrorMessage"] = "An error occurred while updating this Address.";
                }

            }

            // Set error message in case the model is invalid
            TempData["ErrorMessage"] = "Please check the input data and try again.";

            PopulateDropDownLists();
            return View(address);
        }

        // GET: Address/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses
                .Include(a => a.Member)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // POST: Address/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();

                // Set success message in TempData
                TempData["SuccessMessage"] = "Address deleted successfully!";
            }
            else
            {
                // If Address not found, set an error message
                TempData["ErrorMessage"] = "Address not found!";
            }

            // Redirect to the Index or other appropriate page
            return RedirectToAction(nameof(Index));
        }

        private SelectList MemberSelectList(int? selectedId)
        {
            return new SelectList(_context.Members
                .OrderBy(m => m.MemberName)
                .ThenBy(m => m.MemberName), "ID", "MemberName", selectedId);
        }

        private void PopulateDropDownLists(Address? address = null)
        {
            ViewData["MemberID"] = MemberSelectList(address?.MemberID);

        }
        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.ID == id);
        }
    }
}
