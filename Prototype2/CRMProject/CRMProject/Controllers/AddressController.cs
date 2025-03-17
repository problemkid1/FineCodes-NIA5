using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRMProject.Data;
using CRMProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace CRMProject.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly CRMContext _context;

        public AddressController(CRMContext context)
        {
            _context = context;
        }

        // GET: Address
        public async Task<IActionResult> Index(
    string? AddressCity,
    Province? Province,
    string? PostalCode,
    MemberStatus? memberStatus,
    string? memberSize,
    string? searchString,
    string? membershipTypeName)
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
                addresses = addresses.Where(a => a.AddressCity.ToLower().Contains(AddressCity.ToLower()));
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
                addresses = addresses.Where(a => a.PostalCode.ToLower().Contains(PostalCode.ToLower()));
                numberFilters++;
            }

            // Filter by Member Status
            if (memberStatus.HasValue)
            {
                addresses = addresses.Where(a => a.Member.MemberStatus == memberStatus.Value);
                numberFilters++;
            }

            // Filter by Member Size
            if (!string.IsNullOrEmpty(memberSize) && int.TryParse(memberSize, out int size))
            {
                addresses = addresses.Where(a => a.Member.MemberSize == size);
                numberFilters++;
            }

            // Search by Member Name
            if (!string.IsNullOrEmpty(searchString))
            {
                addresses = addresses.Where(a => a.Member.MemberName.ToLower().Contains(searchString.ToLower()));
                numberFilters++;
            }

            if (!string.IsNullOrEmpty(membershipTypeName))
            {
                addresses = addresses.Where(a => a.Member.MemberMembershipTypes
                    .Any(mt => mt.MembershipType.MembershipTypeName.ToLower() == membershipTypeName.ToLower()));
                numberFilters++;
            }

            // Ensure each member has only one address
            addresses = addresses.GroupBy(a => a.MemberID)
                                 .Select(g => g.FirstOrDefault());

            // Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                ViewData["Filtering"] = "btn-danger";
                ViewData["numberFilters"] = "(" + numberFilters.ToString() + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
                ViewData["ShowFilter"] = "show";
            }
            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Address", Url = "/Address/Index", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
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
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Address", Url = "/Address/Index", IsActive = false },
                new BreadcrumbItem { Title = address.Summary, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["AddressId"] = address.ID;
            return View(address); 
        }



        // GET: Address/Create
        public IActionResult Create(int memberId)
        {
            if (memberId == 0)
            {
                TempData["ErrorMessage"] = "Member ID is required!";
                return RedirectToAction("Index", "Member");
            }

            var address = new Address
            {
                MemberID = memberId
            };

            ViewData["MemberId"] = memberId; // Ensure Member ID is passed to View
            PopulateDropDownLists();
            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Address", Url = "/Address/Index", IsActive = true },
                    new BreadcrumbItem { Title = "Create", Url = "/Address/Create", IsActive = true },
                     new BreadcrumbItem { Title = memberId.ToString(), Url = "/Address/Create", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
            return View(address);
        }




        // POST: Address/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,AddressLine1,AddressLine2,AddressCity,Province,PostalCode,AddressType,MemberID")] Address address)
        {
            if (address.MemberID == 0)
            {
                TempData["ErrorMessage"] = "Member ID is required!";
                ViewData["MemberId"] = address.MemberID;
                PopulateDropDownLists();
                return View(address);
            }

            if (ModelState.IsValid)
            {
                // Check if the member already has an address
                var existingAddress = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.MemberID == address.MemberID);

                if (existingAddress != null)
                {
                    TempData["ErrorMessage"] = "This member already has an address.";
                    ViewData["MemberId"] = address.MemberID;
                    PopulateDropDownLists();
                    return View(address);
                }

                try
                {
                    _context.Addresses.Add(address);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Address created successfully!";
                    return RedirectToAction("Details", "Member", new { id = address.MemberID });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again.");
                }
            }

                 var breadcrumbs = new List<BreadcrumbItem>
                 {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Address", Url = "/Address/Index", IsActive = false },
                    new BreadcrumbItem { Title = address.Summary, Url = "#", IsActive = true }
                 };

                    ViewData["Breadcrumbs"] = breadcrumbs;
                    ViewData["MemberId"] = address.MemberID;
                    ViewData["AddressId"] = address.ID;
                    PopulateDropDownLists();

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
            
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Address", Url = "/Address/Index", IsActive = false },
                new BreadcrumbItem { Title = address.Summary, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["AddressId"] = address.ID;

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

                    // Redirect to the Member's Details page instead of Address Details
                    return RedirectToAction("Details", "Member", new { id = address.MemberID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating this Address.";
                }
            }

            TempData["ErrorMessage"] = "Please check the input data and try again.";

            // Ensure dropdown lists are repopulated
            PopulateDropDownLists();

            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Address", Url = "/Address/Index", IsActive = false },
                new BreadcrumbItem { Title = address.Summary, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["AddressId"] = address.ID;

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
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Address", Url = "/Address/Index", IsActive = false },
                new BreadcrumbItem { Title = address.Summary, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["AddressId"] = address.ID;
            return View(address);
        }

        // POST: Address/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            try
            {
                if (address != null)
                {
                    int memberId = address.MemberID; // Store MemberID before deleting

                    _context.Addresses.Remove(address);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Address deleted successfully!";

                    // Redirect to the Member's Details page
                    return RedirectToAction("Details", "Member", new { id = memberId });
                }


                else
                {
                    // If Address not found, set an error message
                    TempData["ErrorMessage"] = "Address not found!";
                }
            }
            catch (DbUpdateException)
            {
                //Note: there is really no reason a delete should fail if you can "talk" to the database.
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Address", Url = "/Address/Index", IsActive = false },
                new BreadcrumbItem { Title = address.Summary, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["AddressId"] = address.ID;
            // Redirect to the Member's Details page as a fallback
            return RedirectToAction("Index", "Member");
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
