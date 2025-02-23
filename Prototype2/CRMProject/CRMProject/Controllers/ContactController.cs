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
            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Contact", Url = "/Contact/Index", IsActive = false },
                    new BreadcrumbItem { Title = "Create", Url = "/Contact/Create", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
            return View();
        }

        // POST: Contact/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,ContactTitleRole,ContactPhone,ContactWebsite,ContactInteractions,ContactNotes")] Contact contact)
        {
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

                // Debug: Log model state errors (replace with your logging mechanism)
                // _logger.LogWarning("CreateContact: ModelState is invalid. Errors: {@Errors}", errors);
                return Json(new { success = false, message = "Invalid model state", errors });
            }

            try
            {
                // Debug: Log the incoming contact data
                // _logger.LogInformation("CreateContact: Adding contact {@Contact}", contact);

                // Add the contact and save to get the generated ID
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                // Debug: Check if contact ID has been generated
                if (contact.ID <= 0)
                {
                    // _logger.LogError("CreateContact: Contact ID was not generated properly.");
                    return Json(new { success = false, message = "Contact ID not generated after save." });
                }

                // Debug: Log the generated contact ID
                // _logger.LogInformation("CreateContact: Generated Contact ID {ContactID}", contact.ID);

                // Create and save the member-contact relationship
                var memberContact = new MemberContact
                {
                    MemberID = memberId,
                    ContactID = contact.ID
                };

                _context.MemberContacts.Add(memberContact);
                await _context.SaveChangesAsync();

                // Debug: Log successful creation of both contact and memberContact
                // _logger.LogInformation("CreateContact: Successfully created Contact and MemberContact relationship.");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Debug: Log the exception details (consider using a logging framework)
                // _logger.LogError(ex, "CreateContact: Exception occurred while creating contact.");

                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    stackTrace = ex.StackTrace // For debugging only; remove in production
                });
            }

            
        }





        // GET: Contact/Edit/5
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

        // POST: Contact/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,ContactTitleRole,ContactPhone,ContactWebsite,ContactInteractions,ContactNotes")] Contact contact)
        {
            if (id != contact.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Contact details updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = contact.ID });
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.ID))
                    {
                        // If the contact does not exist anymore, return NotFound
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
                    TempData["ErrorMessage"] = "An error occurred while updating the contact details.";
                }
            }

            // Set error message in case the model is invalid
            TempData["ErrorMessage"] = "Please check the input data and try again.";
            
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Contact", Url = "/Contact/Index", IsActive = false },
                new BreadcrumbItem { Title = contact.FirstName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["ContactId"] = contact.ID;
            return View(contact); // Return to the edit view if there are validation errors
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
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();

                // Set success message in TempData
                TempData["SuccessMessage"] = "Contact deleted successfully!";
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

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.ID == id);
        }
    }
}
