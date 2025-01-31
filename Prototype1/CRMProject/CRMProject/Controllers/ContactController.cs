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

            return View(contact);
        }

        // GET: Contact/Create
        public IActionResult Create()
        {
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

            // Return to the Create view in case of failure or validation errors
            return View(contact);
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

            return View(contact); // Return to the edit view if there are validation errors
        }

        // GET: Contact/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            // Redirect to the Index or other appropriate page
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.ID == id);
        }
    }
}
