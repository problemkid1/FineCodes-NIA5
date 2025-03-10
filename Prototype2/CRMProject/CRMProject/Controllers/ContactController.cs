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

namespace CRMProject.Controllers
{
    public class ContactController : Controller
    {
        private readonly CRMContext _context;

        public ContactController(CRMContext context)
        {
            _context = context;
        }

        // GET: Contact
        public async Task<IActionResult> Index(string? SearchString, string? FirstName, string? LastName, string? ContactPhone, string? ContactTitleRole)
        {
            // Initialize the queryable contacts dataset
            var contacts = _context.Contacts
                .Include(o => o.Opportunities)
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
                new BreadcrumbItem { Title = contact.FirstName, Url = "#", IsActive = true }
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
        public async Task<IActionResult> CreateContact([FromForm] Contact contact, int memberId)
        {
            // Check if model binding succeeded
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return Json(new { success = false, message = "Invalid model state", errors });
            }

            try
            {

                // Add the contact and save to get the generated ID
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                // Debug: Check if contact ID has been generated
                if (contact.ID <= 0)
                {
                    // _logger.LogError("CreateContact: Contact ID was not generated properly.");
                    return Json(new { success = false, message = "Contact ID not generated after save." });
                }

                // Create and save the member-contact relationship
                var memberContact = new MemberContact
                {
                    MemberID = memberId,
                    ContactID = contact.ID
                };

                _context.MemberContacts.Add(memberContact);
                await _context.SaveChangesAsync();

               

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
               

                return Json(new
                {
                    success = false,
                    message = ex.Message
                   
                });
            }

            
        }




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
                    new BreadcrumbItem { Title = contact.FirstName, Url = "#", IsActive = true }
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
        new BreadcrumbItem { Title = contact.FirstName, Url = "#", IsActive = true }
    };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["ContactId"] = contact.ID;
            PopulateAssignedMemberData(contact);
            return View(contactToUpdate);
        }






        // GET: Contact/Delete/5
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
                new BreadcrumbItem { Title = contact.FirstName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["ContactId"] = contact.ID;
            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts
                .Include(c => c.Opportunities)
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
                new BreadcrumbItem { Title = contact.FirstName, Url = "#", IsActive = true }
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
