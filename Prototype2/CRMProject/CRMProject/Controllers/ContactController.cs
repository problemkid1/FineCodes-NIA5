using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRMProject.Data;
using CRMProject.Models;
using CRMProject.Utilities;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;

namespace CRMProject.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class ContactController : Controller
    {
        private readonly CRMContext _context;

        public ContactController(CRMContext context)
        {
            _context = context;
        }

        // GET: Contact
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Index(string? SearchString, string? FirstName, string? LastName, string? ContactPhone, string? ContactTitleRole)
        {
            // Initialize the queryable contacts dataset
            var contacts = _context.Contacts
                .Include(c => c.OpportunityContacts).ThenInclude(o => o.Opportunity)
                .Include(c => c.MemberContacts)                
                .ThenInclude(mc => mc.Member)
                .AsNoTracking();

            // Count the number of filters applied
            int numberFilters = 0;

            // Filter by Search String
            if (!string.IsNullOrEmpty(SearchString))
            {
                contacts = contacts.Where(c => c.Summary.Contains(SearchString));
                numberFilters++;
            }

            // Filter by First Name
            if (!string.IsNullOrEmpty(FirstName))
            {
                contacts = contacts.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
                numberFilters++;
            }

            // Filter by Last Name
            if (!string.IsNullOrEmpty(LastName))
            {
                contacts = contacts.Where(c => c.LastName.ToLower().Contains(LastName.ToLower()));
                numberFilters++;
            }

            // Filter by Contact Phone
            if (!string.IsNullOrEmpty(ContactPhone))
            {
                contacts = contacts.Where(c => c.ContactPhone.Contains(ContactPhone));
                numberFilters++;
            }

            // Filter by Contact Title/Role
            if (!string.IsNullOrEmpty(ContactTitleRole))
            {
                contacts = contacts.Where(c => c.ContactTitleRole.ToLower().Contains(ContactTitleRole.ToLower()));
                numberFilters++;
            }

            // Provide feedback about filtering in ViewData
            if (numberFilters > 0)
            {
                ViewData["Filtering"] = "btn-danger"; // Highlight the filter button
                ViewData["numberFilters"] = $"({numberFilters} Filter{(numberFilters > 1 ? "s" : "")} Applied)";
                ViewData["ShowFilter"] = "show"; // Keep Bootstrap collapse open
            }
            else
            {
                ViewData["Filtering"] = "btn-outline-secondary";
                ViewData["numberFilters"] = ""; // No filters applied
                ViewData["ShowFilter"] = ""; // Collapse closed
            }

            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Contact", Url = "/Contact/Index", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;

            // Execute the query and pass the results to the view
            return View(await contacts.ToListAsync());
        }


        // GET: Contact/Details/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contact == null)
            {
                return NotFound();
            }
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Contact", Url = "/Contact/Index", IsActive = false },
                new BreadcrumbItem { Title = contact.FirstName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["ContactId"] = contact.ID;
            return View(contact);
        }

        // GET: Contact/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            Contact contact = new Contact();
            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Contact", Url = "/Contact/Index", IsActive = false },
                    new BreadcrumbItem { Title = "Create", Url = "/Contact/Create", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
            PopulateAssignedMemberData(contact);
            return View();
        }

        // POST: Contact/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,ContactTitleRole,ContactPhone,ContactEmailAddress,ContactWebsite,ContactInteractions,ContactNotes")] Contact contact,
            string[] selectedMember)
        {
            UpdateMember(selectedMember, contact);
            if (ModelState.IsValid)
            {
                try
                {
                    // Add the new member to the context and save changes
                    _context.Add(contact);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Contact created successfully!";
                    return RedirectToAction(nameof(Details), new { id = contact.ID });
                }
                catch (Exception)
                {
                    // Set error message in case of failure
                    TempData["ErrorMessage"] = "An error occurred while creating the contact.";
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
                new BreadcrumbItem { Title = "Contact", Url = "/Contact/Index", IsActive = false },
                new BreadcrumbItem { Title = contact.FirstName, Url = "#", IsActive = false },
                new BreadcrumbItem { Title = "Create", Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["ContactId"] = contact.ID;
            PopulateAssignedMemberData(contact);
            // Return to the Create view in case of failure or validation errors
            return View(contact);
        }

        //Adding a new contactmember
        // Add this new action to your existing ContactController
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateContact([FromForm] Contact contact, int memberId)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(kvp => kvp.Value.Errors.Any())
                                       .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());

                return Json(new { success = false, message = "Invalid input data", errors });
            }

            try
            {
                // Add the contact and save to generate the ID
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                if (contact.ID <= 0)
                {
                    return Json(new { success = false, message = "Failed to create contact. Please try again." });
                }

                // Create and save the member-contact relationship
                var memberContact = new MemberContact
                {
                    MemberID = memberId,
                    ContactID = contact.ID
                };

                _context.MemberContacts.Add(memberContact);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Contact created successfully." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while creating the contact. Please try again." });
            }
        }


        //Partial Contact for creating member
        //[HttpPost]
        //public async Task<IActionResult> CreateContactAjax([FromBody] Contact contact)
        //{
        //    Console.WriteLine($"Received contact data: {contact?.FirstName} {contact?.LastName}");

        //    if (contact == null)
        //    {
        //        return Json(new { success = false, message = "No contact data received" });
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Ensure no null values for non-required fields
        //            contact.ContactTitleRole = contact.ContactTitleRole ?? string.Empty;
        //            contact.ContactPhone = contact.ContactPhone ?? string.Empty;
        //            contact.ContactNotes = contact.ContactNotes ?? string.Empty;

        //            _context.Add(contact);
        //            await _context.SaveChangesAsync();

        //            return Json(new
        //            {
        //                success = true,
        //                contactId = contact.ID,
        //                contactName = $"{contact.FirstName} {contact.LastName}" +
        //                    (string.IsNullOrEmpty(contact.ContactTitleRole) ? "" : $" - {contact.ContactTitleRole}")
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error creating contact: {ex.Message}");
        //            return Json(new
        //            {
        //                success = false,
        //                message = "Error creating contact: " + ex.Message
        //            });
        //        }
        //    }

        //    var errors = ModelState.ToDictionary(
        //        kvp => kvp.Key,
        //        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
        //    );

        //    return Json(new
        //    {
        //        success = false,
        //        errors = errors
        //    });
        //}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            // Retrieve MemberContactID from the bridge table
            var memberContact = await _context.MemberContacts
                .FirstOrDefaultAsync(mc => mc.ContactID == contact.ID);

            if (memberContact != null)
            {
                ViewData["MemberContactID"] = memberContact.MemberID; // Pass MemberContactID to the view
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Contact", Url = "/Contact/Index", IsActive = false },
                new BreadcrumbItem { Title = contact.FirstName, Url = $"/Contact/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }

            };

            ViewData["Breadcrumbs"] = breadcrumbs;
                    ViewData["ContactId"] = contact.ID;
            PopulateAssignedMemberData(contact);
            return View(contact);
        }

        // POST: Contact/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,ContactTitleRole,ContactPhone,ContactWebsite,ContactEmailAddress,ContactInteractions,ContactNotes")] Contact contact, string[] selectedOptions)
        {
            var contactToUpdate = await _context.Contacts
                .Include(d => d.MemberContacts)
                .ThenInclude(d => d.Member)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (contactToUpdate == null)
            {
                return NotFound();
            }

            // Update the contactToUpdate properties
            contactToUpdate.FirstName = contact.FirstName;
            contactToUpdate.LastName = contact.LastName;
            contactToUpdate.ContactTitleRole = contact.ContactTitleRole;
            contactToUpdate.ContactPhone = contact.ContactPhone;            
            contactToUpdate.ContactEmailAddress = contact.ContactEmailAddress;
            contactToUpdate.ContactInteractions = contact.ContactInteractions;
            contactToUpdate.ContactNotes = contact.ContactNotes;

            UpdateMember(selectedOptions, contactToUpdate);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(contactToUpdate).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Contact updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = contact.ID });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ContactExists(contact.ID))
                    {
                        return NotFound();
                    }
                    Console.WriteLine($"Concurrency error: {ex.Message}");
                    TempData["ErrorMessage"] = "The contact was modified by another user. Please refresh and try again.";
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    TempData["ErrorMessage"] = "A database error occurred. Please ensure all required fields are filled correctly.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General error: {ex.Message}");
                    TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
                }
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Contact", Url = "/Contact/Index", IsActive = false },
                new BreadcrumbItem { Title = contact.FirstName, Url = $"/Contact/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
                
            };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["ContactId"] = contact.ID;
            PopulateAssignedMemberData(contact);
            return View(contactToUpdate);
        }

        [HttpGet]
        public IActionResult TestContacts()
        {
            var allContacts = _context.Contacts
                .Select(c => new {
                    id = c.ID,
                    name = $"{c.FirstName} {c.LastName}",
                    title = c.ContactTitleRole
                })
                .Take(10)
                .ToList();

            return Json(allContacts);
        }

        [HttpGet]
        [ActionName("SearchContacts")]
        public IActionResult SearchContacts(string term)
        {
            System.Diagnostics.Debug.WriteLine($"SearchContacts called with term: {term}");

            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new object[0]);
            }

            try
            {
                // First, check if we have any contacts at all
                var totalContacts = _context.Contacts.Count();
                System.Diagnostics.Debug.WriteLine($"Total contacts in database: {totalContacts}");

                // Log the search condition
                var searchCondition = $"Searching for FirstName.Contains('{term}') OR LastName.Contains('{term}') OR (FirstName + ' ' + LastName).Contains('{term}')";
                System.Diagnostics.Debug.WriteLine(searchCondition);

                // Try a more lenient search
                var contacts = _context.Contacts
                    .Where(c => c.FirstName.ToLower().Contains(term.ToLower()) ||
                                c.LastName.ToLower().Contains(term.ToLower()) ||
                                (c.FirstName + " " + c.LastName).ToLower().Contains(term.ToLower()))
                    .Select(c => new {
                        id = c.ID,
                        label = $"{c.FirstName} {c.LastName}" + (string.IsNullOrEmpty(c.ContactTitleRole) ? "" : $" - {c.ContactTitleRole}")
                    })
                    .Take(10)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {contacts.Count} contacts");

                // If no results, return a test contact for debugging
                if (contacts.Count == 0 && totalContacts > 0)
                {
                    System.Diagnostics.Debug.WriteLine("No matches found, returning a test contact for debugging");
                    return Json(new[] {
                new {
                    id = 999,
                    label = "Test Contact - No actual matches found"
                }
            });
                }

                return Json(contacts);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SearchContacts: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllContacts(int page = 1, int pageSize = 50)
        {
            try
            {
                // Get total count for pagination info
                var totalCount = _context.Contacts.Count();

                // Retrieve paged data
                var contactsData = _context.Contacts
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new {
                        c.ID,
                        c.FirstName,
                        c.LastName,
                        c.ContactTitleRole
                    })
                    .ToList();

                // Format the data
                var formattedContacts = contactsData.Select(c => new {
                    id = c.ID,
                    label = $"{c.FirstName} {c.LastName}" + (string.IsNullOrEmpty(c.ContactTitleRole) ? "" : $" - {c.ContactTitleRole}")
                }).ToList();

                return Json(new
                {
                    contacts = formattedContacts,
                    totalCount = totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    currentPage = page
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAllContacts: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }





        // GET: Contact/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            var contact = await _context.Contacts

               .FirstOrDefaultAsync(m => m.ID == id);
            try
            {

                if (contact == null)
                {
                    return NotFound();
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
                new BreadcrumbItem { Title = "Contact", Url = "/Contact/Index", IsActive = false },
                new BreadcrumbItem { Title = contact.FirstName, Url = $"/Contact/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }
                
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["ContactId"] = contact.ID;
            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts
                .Include(c => c.OpportunityContacts)
                .ThenInclude(c => c.Opportunity)
                .Include(c => c.MemberContacts)
                .ThenInclude(c => c.Member)
                .FirstOrDefaultAsync(c => c.ID == id);

            // Check if the contact has members
            //if (Member != null)
            //{
            //    TempData["ErrorMessage"] = "Cannot delete contact because it has associated member(s).";
            //    return RedirectToAction(nameof(Index));
            //}

            //// Check if the contact has opportunities
            //if (contact.Opportunities.Any())
            //{
            //    TempData["ErrorMessage"] = "Cannot delete contact because it has associated opportunities.";
            //    return RedirectToAction(nameof(Index));
            //}

            if (contact != null)
            {
                // Retrieve the associated member's ID
                var memberContact = await _context.MemberContacts
                    .FirstOrDefaultAsync(mc => mc.ContactID == contact.ID);

                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();

                // Set success message in TempData
                TempData["SuccessMessage"] = "Contact deleted successfully!";

                if (memberContact != null)
                {
                    // Redirect to the member's details page
                    return RedirectToAction("Details", "Member", new { id = memberContact.MemberID });
                }
            }
            else
            {
                // If contact not found, set an error message
                TempData["ErrorMessage"] = "Contact not found!";
            }
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Contact", Url = "/Contact/Index", IsActive = false },
                new BreadcrumbItem { Title = contact.FirstName, Url = $"/Contact/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }

             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["ContactId"] = contact.ID;
            // Redirect to the Index or other appropriate page
            return RedirectToAction(nameof(Index));
        }

        private void PopulateAssignedMemberData(Contact contact)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.Members;
            var currentOptionsHS = new HashSet<int>(contact.MemberContacts.Select(b => b.MemberID));
            //Instead of one list with a boolean, we will make two lists
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach (var s in allOptions)
            {
                if (currentOptionsHS.Contains(s.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.MemberName
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.MemberName
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }

        private void UpdateMember(string[] selectedOptions, Contact contactToUpdate)
        {
            if (selectedOptions == null)
            {
                contactToUpdate.MemberContacts = new List<MemberContact>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(contactToUpdate.MemberContacts.Select(b => b.MemberID));
            foreach (var s in _context.Members)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString()))//it is selected
                {
                    if (!currentOptionsHS.Contains(s.ID))//but not currently in the Doctor's collection - Add it!
                    {
                        contactToUpdate.MemberContacts.Add(new MemberContact
                        {
                            MemberID = s.ID,
                            ContactID = contactToUpdate.ID
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(s.ID))//but is currently in the Doctor's collection - Remove it!
                    {
                        MemberContact? specToRemove = contactToUpdate.MemberContacts.FirstOrDefault(d => d.MemberID == s.ID);
                        if (specToRemove != null)
                        {
                            _context.Remove(specToRemove);
                        }
                    }
                }
            }
        }



        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.ID == id);
        }
    }
}
