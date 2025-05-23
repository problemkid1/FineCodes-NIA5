﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRMProject.Data;
using CRMProject.Models;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authorization;
using CRMProject.Utilities;
using CRMProject.Data.CRMMigrations;

namespace CRMProject.Controllers
{
    [Authorize(Roles = "Super, Admin, User")]
    public class OpportunityController : Controller
    {
        private readonly CRMContext _context;

        public OpportunityController(CRMContext context)
        {
            _context = context;
        }

        // GET: Opportunity
        [Authorize(Roles = "Super, Admin, User")]
        public async Task<IActionResult> Index(string? OpportunityName, OpportunityStatus? OpportunityStatus, string? OpportunityPriority, int? page, int? pageSizeID)
        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            var opportunities = _context.Opportunities
                .Include(o => o.OpportunityContacts).ThenInclude(oc => oc.Contact)
                .AsNoTracking();

            //By default, exclude closed opportunities
            if (!OpportunityStatus.HasValue || (OpportunityStatus != CRMProject.Models.OpportunityStatus.ClosedNewMember && OpportunityStatus != CRMProject.Models.OpportunityStatus.ClosedNotInterested))
            {
                opportunities = opportunities.Where(o => o.OpportunityStatus != CRMProject.Models.OpportunityStatus.ClosedNewMember && o.OpportunityStatus != CRMProject.Models.OpportunityStatus.ClosedNotInterested);
            }
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
            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunity", Url = "/Opportunity/Index", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;

            // Get Opportunity name list for dropdown
            ViewBag.OpportunitiesList = _context.Opportunities
                .Select(a => new { Value = a.OpportunityName, Text = a.OpportunityName })
                .Where(a => !string.IsNullOrEmpty(a.Value))
                .Distinct()
                .OrderBy(mt => mt.Text)
                .ToList();


            // Get Priorities list for dropdown
            ViewBag.PrioritiesList = _context.Opportunities
                .Select(a => new { Value = a.OpportunityPriority, Text = a.OpportunityPriority })
                .Where(a => !string.IsNullOrEmpty(a.Value))
                .Distinct()
                .OrderBy(mt => mt.Text)
                .ToList();
            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Opportunity>.CreateAsync(opportunities.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }


        // GET: Opportunity/Details/5
        [Authorize(Roles = "Super, Admin, User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .Include(m => m.OpportunityContacts).ThenInclude(mc => mc.Contact)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (opportunity == null)
            {
                return NotFound();
            }
            var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunity", Url = "/Opportunity/Index", IsActive = false },
                    new BreadcrumbItem { Title = opportunity.OpportunityName, Url = "#", IsActive = true }

                };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["OpportunityId"] = opportunity.ID;
            return View(opportunity);
        }

        // GET: Opportunity/Create
        [Authorize(Roles = "Super, Admin")]
        public IActionResult Create()
        {
            Opportunity opportunity = new Opportunity
            {
                OpportunityStatus = OpportunityStatus.Qualification
            };

            // Create an empty MultiSelectList for the contact selection partial view
            ViewData["selOpts"] = new MultiSelectList(new List<SelectListItem>(), "Value", "Text");

            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunity", Url = "/Opportunity/Index", IsActive = false },
                    new BreadcrumbItem { Title = "Create", Url = "/Opportunity/Create", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["OpportunityId"] = opportunity.ID;

            return View();
        }

        // POST: Opportunity/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super, Admin")]
        public async Task<IActionResult> Create([Bind("ID,OpportunityName,OpportunityStatus,OpportunityPriority,OpportunityAction,OpportunityContact,OpportunityAccount,OpportunityLastContactDate,OpportunityInteractions")] Opportunity opportunity,
            string[] selectedContact)
        {
            // Check if any contacts are selected
            if (selectedContact == null || selectedContact.Length == 0)
            {
                ModelState.AddModelError("OpportunityContacts", "Select at least one contact.");

                // Create an empty MultiSelectList for the view
                ViewData["selOpts"] = new MultiSelectList(new List<SelectListItem>(), "Value", "Text");

                var opportunityBreadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunity", Url = "/Opportunity/Index", IsActive = false },
                    new BreadcrumbItem { Title = opportunity.OpportunityName, Url = "#", IsActive = true }
                };
                ViewData["Breadcrumbs"] = opportunityBreadcrumbs;
                ViewData["OpportunityId"] = opportunity.ID;

                // Set error message
                TempData["ErrorMessage"] = "Please select at least one contact.";

                return View(opportunity);
            }

            UpdateContact(selectedContact, opportunity);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingopportunity = await _context.Opportunities
                        .FirstOrDefaultAsync(i => i.OpportunityName == opportunity.OpportunityName);

                    if (existingopportunity != null)
                    {
                        ModelState.AddModelError("OpportunityName", "opportunity with this OpportunityName already exists.");

                        // Create an empty MultiSelectList for the view
                        ViewData["selOpts"] = new MultiSelectList(new List<SelectListItem>(), "Value", "Text");

                        return View(opportunity);
                    }

                    _context.Add(opportunity);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Opportunity created successfully!";
                    return RedirectToAction(nameof(Details), new { id = opportunity.ID });
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("OpportunityName", "Unable to save changes. Remember, Opportunity Name must be unique.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while saving the Opportunity.";
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while creating the Opportunity.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please check the input data and try again.";
            }

            // If we get here, something went wrong - repopulate the contact selection
            // Get the selected contacts to repopulate the view
            var selectedContactItems = new List<SelectListItem>();
            if (selectedContact != null && selectedContact.Length > 0)
            {
                var contactIds = selectedContact.Select(id => int.Parse(id)).ToList();
                var contacts = await _context.Contacts
                    .Where(c => contactIds.Contains(c.ID))
                    .Select(c => new SelectListItem
                    {
                        Value = c.ID.ToString(),
                        Text = c.FirstName + " " + c.LastName
                    })
                    .ToListAsync();
                selectedContactItems = contacts;
            }
            ViewData["selOpts"] = new MultiSelectList(selectedContactItems, "Value", "Text");

            var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunity", Url = "/Opportunity/Index", IsActive = false },
                    new BreadcrumbItem { Title = opportunity.OpportunityName, Url = "#", IsActive = true }
                };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["OpportunityId"] = opportunity.ID;
            return View(opportunity);
        }

        // GET: Opportunity/Edit/5
        [Authorize(Roles = "Super, Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .Include(m => m.OpportunityContacts).ThenInclude(mc => mc.Contact)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (opportunity == null)
            {
                return NotFound();
            }

            // Create a MultiSelectList for the existing contacts
            var selectedContacts = opportunity.OpportunityContacts
                .Select(oc => new SelectListItem
                {
                    Value = oc.ContactID.ToString(),
                    Text = oc.Contact.FirstName + " " + oc.Contact.LastName
                })
                .ToList();

            ViewData["selOpts"] = new MultiSelectList(selectedContacts, "Value", "Text");

            var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunity", Url = "/Opportunity/Index", IsActive = false },
                    new BreadcrumbItem { Title = opportunity.OpportunityName, Url = $"/Opportunity/Details/{id}", IsActive = false },
                    new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
                };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["OpportunityId"] = opportunity.ID;
            return View(opportunity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super, Admin")]
        public async Task<IActionResult> Edit(int id, string[] selectedContact)
        {
            var opportunityToUpdate = await _context.Opportunities
                .Include(o => o.OpportunityContacts).ThenInclude(oc => oc.Contact)
                .FirstOrDefaultAsync(o => o.ID == id);

            if (opportunityToUpdate == null)
            {
                return NotFound();
            }

            // Validate selected contacts
            if (selectedContact == null || selectedContact.Length == 0)
            {
                ModelState.AddModelError("OpportunityContacts", "Please select at least one contact.");

                // Create a MultiSelectList for the existing contacts
                var currentContacts = opportunityToUpdate.OpportunityContacts
                    .Select(oc => new SelectListItem
                    {
                        Value = oc.ContactID.ToString(),
                        Text = oc.Contact.FirstName + " " + oc.Contact.LastName
                    })
                    .ToList();

                ViewData["selOpts"] = new MultiSelectList(currentContacts, "Value", "Text");

                ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunities", Url = "/Opportunity/Index", IsActive = false },
                    new BreadcrumbItem { Title = opportunityToUpdate.OpportunityName, Url = $"/Opportunity/Details/{id}", IsActive = false },
                    new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
                };

                ViewData["OpportunityId"] = opportunityToUpdate.ID;
                TempData["ErrorMessage"] = "Please select at least one contact.";

                return View(opportunityToUpdate);
            }

            // Update many-to-many relationship
            UpdateContact(selectedContact, opportunityToUpdate);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                // Create a MultiSelectList for the selected contacts
                var selectedContactItems = new List<SelectListItem>();
                if (selectedContact != null && selectedContact.Length > 0)
                {
                    var contactIds = selectedContact.Select(cid => int.Parse(cid)).ToList();
                    var contacts = await _context.Contacts
                        .Where(c => contactIds.Contains(c.ID))
                        .Select(c => new SelectListItem
                        {
                            Value = c.ID.ToString(),
                            Text = c.FirstName + " " + c.LastName
                        })
                        .ToListAsync();
                    selectedContactItems = contacts;
                }
                ViewData["selOpts"] = new MultiSelectList(selectedContactItems, "Value", "Text");

                ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunities", Url = "/Opportunity/Index", IsActive = false },
                    new BreadcrumbItem { Title = opportunityToUpdate.OpportunityName, Url = $"/Opportunity/Details/{id}", IsActive = false },
                    new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
                };

                ViewData["Opportunity"] = opportunityToUpdate.ID;
                return View(opportunityToUpdate);
            }

            // Attempt update
            if (await TryUpdateModelAsync<Opportunity>(opportunityToUpdate, "",
                o => o.OpportunityName, o => o.OpportunityStatus,
                o => o.OpportunityPriority, o => o.OpportunityAction,
                o => o.OpportunityLastContactDate, o => o.OpportunityInteractions))
            {
                try
                {
                    _context.Update(opportunityToUpdate);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Opportunity details updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = opportunityToUpdate.ID });
                }
                catch (DbUpdateException dex)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists see your system administrator.");
                    Console.WriteLine(dex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["ErrorMessage"] = "An unexpected error occurred.";
                }
            }

            // Repopulate view data on failure
            // Create a MultiSelectList for the selected contacts
            var selectedContactsForView = new List<SelectListItem>();
            if (selectedContact != null && selectedContact.Length > 0)
            {
                var contactIds = selectedContact.Select(cid => int.Parse(cid)).ToList();
                var contacts = await _context.Contacts
                    .Where(c => contactIds.Contains(c.ID))
                    .Select(c => new SelectListItem
                    {
                        Value = c.ID.ToString(),
                        Text = c.FirstName + " " + c.LastName
                    })
                    .ToListAsync();
                selectedContactsForView = contacts;
            }
            ViewData["selOpts"] = new MultiSelectList(selectedContactsForView, "Value", "Text");

            ViewData["Breadcrumbs"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Opportunities", Url = "/Opportunity/Index", IsActive = false },
                new BreadcrumbItem { Title = opportunityToUpdate.OpportunityName, Url = $"/Opportunity/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
            };

            ViewData["OpportunityId"] = opportunityToUpdate.ID;
            return View(opportunityToUpdate);
        }

        // GET: Opportunity/Delete/5
        [Authorize(Roles = "Super, Admin")]
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
            var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunity", Url = "/Opportunity/Index", IsActive = false },
                    new BreadcrumbItem { Title = opportunity.OpportunityName, Url = $"/Opportunity/Details/{id}", IsActive = false },
                    new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }
                };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["OpportunityId"] = opportunity.ID;
            return View(opportunity);
        }

        [HttpPost]
        [Authorize(Roles = "Super, Admin")]
        public async Task<IActionResult> AddContact(Contact contact, int opportunityId)
        {
            if (ModelState.IsValid)
            {
                var opportunity = await _context.Opportunities
                    .Include(o => o.OpportunityContacts)
                    .FirstOrDefaultAsync(o => o.ID == opportunityId);

                if (opportunity != null)
                {
                    // Add new contact
                    _context.Contacts.Add(contact);
                    await _context.SaveChangesAsync(); // Save first to generate contact.ID

                    // Add the relationship
                    var opportunityContact = new Models.OpportunityContact
                    {
                        OpportunityID = opportunity.ID,
                        ContactID = contact.ID
                    };
                    _context.OpportunityContacts.Add(opportunityContact);

                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Contact added successfully" });
                }
            }

            // Return validation errors in the format expected by the client
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return Json(new { success = false, message = "Failed to add contact", errors = errors });
        }

        // POST: Opportunity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super, Admin")]
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
                // If Opportunity not found, set an error message
                TempData["ErrorMessage"] = "Opportunity not found!";
            }

            // Redirect to the Index or other appropriate page
            return RedirectToAction(nameof(Index));
        }

        // GET: Opportunity/RemoveContact/5
        [Authorize(Roles = "Super, Admin")]
        public async Task<IActionResult> RemoveContact(int contactID, int opportunityID)
        {
            var opportunityContact = await _context.OpportunityContacts
                .Include(mc => mc.Opportunity)
                .Include(mc => mc.Contact)
                .FirstOrDefaultAsync(mc => mc.ContactID == contactID && mc.OpportunityID == opportunityID);

            if (opportunityContact == null)
            {
                TempData["ErrorMessage"] = "Contact relationship not found!";
                return RedirectToAction("Details", new { id = opportunityID });
            }

            return View(opportunityContact); // Show confirmation view
        }

        // POST: Opportunity/RemoveContact/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super, Admin")]
        public async Task<IActionResult> RemoveContactConfirmed(int contactID, int opportunityID)
        {
            var opportunityContact = await _context.OpportunityContacts
                .FirstOrDefaultAsync(mc => mc.ContactID == contactID && mc.OpportunityID == opportunityID);

            if (opportunityContact != null)
            {
                _context.OpportunityContacts.Remove(opportunityContact);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contact removed from the opportunity successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Contact relationship not found!";
            }

            return RedirectToAction("Details", new { id = opportunityID });
        }

        private bool OpportunityExists(int id)
        {
            return _context.Opportunities.Any(e => e.ID == id);
        }

        // Action to convert Opportunity to Member
        [Authorize(Roles = "Super, Admin")]
        public IActionResult ConvertToMember(int opportunityId)
        {
            try
            {
                // Step 1: Find the Opportunity by ID, including related Contact
                var opportunity = _context.Opportunities
                    .Include(o => o.OpportunityContacts) // Ensure Contact is loaded
                    .FirstOrDefault(o => o.ID == opportunityId);

                if (opportunity == null)
                {
                    TempData["ErrorMessage"] = "The opportunity was not found.";
                    return RedirectToAction("Index", "Opportunity");
                }

                // Step 2: Store the necessary data in TempData
                TempData["MemberName"] = opportunity.OpportunityName;
                TempData["MemberStatus"] = MemberStatus.GoodStanding;
                TempData["MemberStartDate"] = DateTime.Today;
                TempData["MemberLastContactDate"] = opportunity.OpportunityLastContactDate;
                TempData["MemberNotes"] = opportunity.OpportunityAction;

                // Step 3: Set success message and redirect to Create action
                TempData["SuccessMessage"] = "Edit and save the new member!";
                return RedirectToAction("Create", "Member");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return RedirectToAction("Error", "Home");
            }
        }

        private SelectList ContactSelectList(string skip)
        {
            var contactQuery = _context.Contacts.AsNoTracking();

            if (!string.IsNullOrEmpty(skip))
            {
                // Convert the string to an array of integers
                string[] avoidStrings = skip.Split('|');
                int[] skipKeys = Array.ConvertAll(avoidStrings, s => int.Parse(s));
                contactQuery = contactQuery.Where(c => !skipKeys.Contains(c.ID));
            }

            return new SelectList(contactQuery.OrderBy(c => c.LastName), "ID", "ContactType");
        }

        [HttpGet]
        [Authorize(Roles = "Super, Admin")]
        public JsonResult GetContacts(string searchKeyword, string skip)
        {
            var contacts = _context.Contacts
                .Where(c => c.ContactType.Contains(searchKeyword))  // Modify this line based on how you search
                .AsNoTracking()
                .OrderBy(c => c.LastName)
                .ToList();

            var result = contacts.Select(c => new
            {
                c.ID,
                c.ContactType,
                c.LastName,
                c.FirstName
            });

            return Json(result);
        }

        private void UpdateContact(string[] selectedOptions, Opportunity opportunityToUpdate)
        {
            // Only initialize if null, don't clear existing data
            if (opportunityToUpdate.OpportunityContacts == null)
            {
                opportunityToUpdate.OpportunityContacts = new List<Models.OpportunityContact>();
            }

            if (selectedOptions == null || selectedOptions.Length == 0)
            {
                return; // Don't clear existing data
            }

            // Rest of the method remains the same
            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(opportunityToUpdate.OpportunityContacts.Select(b => b.ContactID));
            foreach (var s in _context.Contacts)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString()))
                {
                    if (!currentOptionsHS.Contains(s.ID))
                    {
                        opportunityToUpdate.OpportunityContacts.Add(new Models.OpportunityContact
                        {
                            ContactID = s.ID,
                            OpportunityID = opportunityToUpdate.ID
                        });
                    }
                }
                else
                {
                    if (currentOptionsHS.Contains(s.ID))
                    {
                        Models.OpportunityContact specToRemove = opportunityToUpdate.OpportunityContacts.FirstOrDefault(d => d.ContactID == s.ID);
                        if (specToRemove != null)
                        {
                            _context.Remove(specToRemove);
                        }
                    }
                }
            }
        }
    }
}
