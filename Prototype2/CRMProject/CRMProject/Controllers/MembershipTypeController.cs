﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRMProject.Data;
using CRMProject.Models;
using CRMProject.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace CRMProject.Controllers
{
    [Authorize(Roles = "Super, Admin")]
    public class MembershipTypeController : Controller
    {
        private readonly CRMContext _context;

        public MembershipTypeController(CRMContext context)
        {
            _context = context;
        }

        // GET: MembershipType
        public async Task<IActionResult> Index(string? MembershipTypeName, string? MembershipTypeFee, int? page, int? pageSizeID)

        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            var membershipTypes = _context.MembershipTypes
                .Include(mt => mt.MemberMembershipTypes)
                .ThenInclude(mt => mt.Member)
                .AsNoTracking();

            // Create a SelectList for MembershipTypeName enum to be used in the dropdown
            ViewData["MembershipTypeNameList"] = new SelectList(membershipTypes
                                                    .Select(mt => mt.MembershipTypeName)
                                                    .Distinct()
                                                    .OrderBy(name => name));

            // Filter by Membership Type Name (if selected from dropdown)
            if (!string.IsNullOrEmpty(MembershipTypeName))
            {
                membershipTypes = membershipTypes.Where(mt => mt.MembershipTypeName
                                                               .ToLower() == MembershipTypeName.ToLower());
                numberFilters++;
            }

            // Filter by Membership Type Fee as a string for flexible matching
            if (!string.IsNullOrEmpty(MembershipTypeFee))
            {
                if (decimal.TryParse(MembershipTypeFee, out decimal fee))
                {
                    membershipTypes = membershipTypes.Where(mt => mt.MembershipTypeFee == (double)fee);
                    numberFilters++;
                }
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

            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Membership Types", Url = "/MembershipType/Index", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewBag.MembershipTypeNameList = _context.MembershipTypes
            .Select(mt => new { Value = mt.MembershipTypeName, Text = mt.MembershipTypeName })
            .Distinct()
            .OrderBy(mt => mt.Text)
            .ToList();
            ViewBag.MembershipTypeFee = _context.MembershipTypes
            .Select(mt => new { Value = mt.MembershipTypeFee, Text = mt.MembershipTypeFee })
            .Distinct()
            .OrderBy(mt => mt.Text)
            .ToList();
            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<MembershipType>.CreateAsync(membershipTypes.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: MembershipType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var membershipType = await _context.MembershipTypes
                 .Include(i => i.MemberMembershipTypes).ThenInclude(i => i.Member)
                .FirstOrDefaultAsync(m => m.ID == id);

                if (membershipType == null)
                {
                    return NotFound();
                }
                var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Membership Type", Url = "/MembershipType/Index", IsActive = false },
                    new BreadcrumbItem { Title = membershipType.MembershipTypeName, Url = "#", IsActive = true }

                };

                ViewData["Breadcrumbs"] = breadcrumbs;

                ViewData["Membership Type"] = membershipType.ID;
                return View(membershipType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while retrieving the membership details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: MembershipType/Create
        public IActionResult Create()
        {
            ViewBag.MemberID = new SelectList(_context.Members, "ID", "MemberName");

            ViewBag.MembershipTypeNameList = new SelectList(
                _context.MembershipTypes
                    .Select(mt => mt.MembershipTypeName)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList()
            );
            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Membership Type", Url = "/MembershipType/Index", IsActive = false },
                    new BreadcrumbItem { Title = "Create", Url = "/Membership Type/Create", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
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
                try
                {
                    //Check for unique Membership Type Name
                    var existingMembershipTypeName = await _context.MembershipTypes
                        .FirstOrDefaultAsync(i => i.MembershipTypeName.ToLower() == membershipType.MembershipTypeName.ToLower());

                    if (existingMembershipTypeName != null)
                    {
                        // Return a validation error if the Name already exists
                        ModelState.AddModelError("MembershipTypeName", "MembershipType with this name already exists.");
                        return View(membershipType);
                    }
                    membershipType.MembershipTypeName = membershipType.MembershipTypeName.Trim();

                    // Add the new member to the context and save changes
                    _context.Add(membershipType);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Membership Type created successfully!";
                    return RedirectToAction(nameof(Details), new { id = membershipType.ID });
                }
                catch (DbUpdateException dex)
                {
                    // Directly handling the exception without ExceptionMessageVM
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("MembershipTypeName", "Unable to save changes. Remember, membership type name  must be unique.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while saving the Membership Type.";
                    }
                }
                catch (Exception)
                {
                    // General error handling
                    TempData["ErrorMessage"] = "An error occurred while creating the Membership Type.";
                }
            }
            else
            {
                // If model validation fails, set an error message
                TempData["ErrorMessage"] = "Please check the input data and try again.";
            }
            var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Membership Type", Url = "/MembershipType/Index", IsActive = false },
                    new BreadcrumbItem { Title = membershipType.MembershipTypeName, Url = "#", IsActive = true }
                };
            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["Membership Type"] = membershipType.ID;
            ViewBag.MembershipTypeNameList = new SelectList(
                _context.MembershipTypes
                    .Select(mt => mt.MembershipTypeName)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList()
            );

            return View(membershipType);
        }

        // GET: MembershipType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var membershipType = await _context.MembershipTypes.FindAsync(id);
                if (membershipType == null)
                {
                    return NotFound();
                }

                var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Membership Type", Url = "/MembershipType/Index", IsActive = false },
                    new BreadcrumbItem { Title = membershipType.MembershipTypeName, Url = $"/MembershipType/Details/{id}", IsActive = false },
                     new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }


                };

                ViewData["Breadcrumbs"] = breadcrumbs;

                ViewBag.MembershipTypeNameList = new SelectList(
                    _context.MembershipTypes
                        .Select(mt => mt.MembershipTypeName)
                        .Distinct()
                        .OrderBy(name => name)
                        .ToList()
                );

                return View(membershipType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while fetching the membership for editing: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
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
                    //Check for unique Membership Type Name before updating
                    var existingMembershipTypeName = await _context.MembershipTypes
                        .FirstOrDefaultAsync(i => i.MembershipTypeName == membershipType.MembershipTypeName && i.ID != membershipType.ID);

                    if (existingMembershipTypeName != null)
                    {
                        // Return a validation error if the Name already exists
                        ModelState.AddModelError("MembershipTypeName", "MembershipType with this name already exists.");
                        return View(membershipType);
                    }

                    // Update membershipType to the context and save changes
                    _context.Update(membershipType);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Membership Type updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = membershipType.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipTypeExists(membershipType.ID))
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
                        ModelState.AddModelError("MembershipTypeName", "Unable to save changes. Remember, membership type name must be unique.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while saving the Membership.";
                    }
                }
                catch (Exception)
                {
                    // Set error message for generic errors
                    TempData["ErrorMessage"] = "An error occurred while updating the Membership Type details.";
                }
            }
            var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Membership Type", Url = "/MembershipType/Index", IsActive = false },
                    new BreadcrumbItem { Title = membershipType.MembershipTypeName, Url = $"/MembershipType/Details/{id}", IsActive = false },
                     new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
                   

                };
            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["Membership Type"] = membershipType.ID;
            TempData["ErrorMessage"] = "Please check the input data and try again.";

            ViewBag.MembershipTypeNameList = new SelectList(
            _context.MembershipTypes
                .Select(mt => mt.MembershipTypeName)
                .Distinct()
                .OrderBy(name => name)
                .ToList()
            );

            return View(membershipType);
        }

        // GET: MembershipType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var membershipType = await _context.MembershipTypes
                .Include(mt => mt.MemberMembershipTypes)
                .ThenInclude(mt => mt.Member)
                .FirstOrDefaultAsync(m => m.ID == id);
                if (membershipType == null)
                {
                    return NotFound();
                }
                var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Membership Type", Url = "/MembershipType/Index", IsActive = false },
                    new BreadcrumbItem { Title = membershipType.MembershipTypeName, Url = $"/MembershipType/Details/{id}", IsActive = false },
                    new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }



                };

                ViewData["Breadcrumbs"] = breadcrumbs;
                ViewData["Membership Type"] = membershipType.ID;
                return View(membershipType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while fetching the industry for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: MembershipType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membershipType = await _context.MembershipTypes
                .Include(mt => mt.MemberMembershipTypes)
                .ThenInclude(mt => mt.Member)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (membershipType == null)
            {
                TempData["ErrorMessage"] = "Membership not found!";
                return RedirectToAction(nameof(Index));
            }
            // Check if the Membership has members
            if (membershipType.MemberMembershipTypes.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete membership because it has associated member(s).";
                return RedirectToAction(nameof(Index));
            }
            _context.MembershipTypes.Remove(membershipType);
            await _context.SaveChangesAsync();

            var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Membership Type", Url = "/MembershipType/Index", IsActive = false },
                    new BreadcrumbItem { Title = membershipType.MembershipTypeName, Url = $"/MembershipType/Details/{id}", IsActive = false },
                    new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }
                                   
                

                };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["Membership Type"] = membershipType.ID;
            // Set success message in TempData
            TempData["SuccessMessage"] = "Membership Type deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: MembershipType/GetAllMembershipTypes
        [HttpGet]
        public IActionResult GetAllMembershipTypes()
        {
            try
            {
                // Retrieve the basic data from the database
                var membershipTypesData = _context.MembershipTypes
                    .Select(mt => new {
                        mt.ID,
                        mt.MembershipTypeName
                    })
                    .OrderBy(mt => mt.MembershipTypeName)
                    .Take(100) // Limit to 100 membership types for performance
                    .ToList(); // Execute the query and bring data to memory

                // Format the data on the client side
                var formattedMembershipTypes = membershipTypesData.Select(mt => new {
                    id = mt.ID,
                    label = mt.MembershipTypeName
                }).ToList();

                System.Diagnostics.Debug.WriteLine($"Returning all {formattedMembershipTypes.Count} membership types");
                return Json(formattedMembershipTypes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAllMembershipTypes: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        // GET: MembershipType/SearchMembershipTypes
        [HttpGet]
        public IActionResult SearchMembershipTypes(string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    return GetAllMembershipTypes();
                }

                var membershipTypes = _context.MembershipTypes
                    .Where(mt => mt.MembershipTypeName.Contains(term))
                    .Select(mt => new {
                        id = mt.ID,
                        label = mt.MembershipTypeName
                    })
                    .OrderBy(mt => mt.label)
                    .Take(10)
                    .ToList();

                return Json(membershipTypes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SearchMembershipTypes: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }


        private bool MembershipTypeExists(int id)
        {
            return _context.MembershipTypes.Any(e => e.ID == id);
        }
    }
}
