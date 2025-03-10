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
    public class OpportunityController : Controller
    {
        private readonly CRMContext _context;

        public OpportunityController(CRMContext context)
        {
            _context = context;
        }

        // GET: Opportunity
        public async Task<IActionResult> Index(string? OpportunityName, OpportunityStatus? OpportunityStatus, string? OpportunityPriority)
        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            var opportunities = _context.Opportunities
                .Include(o => o.Contact)
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
            return View(await opportunities.ToListAsync());
        }


        // GET: Opportunity/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                 .Include(o => o.Contact)
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

            ViewData["Opportunity"] = opportunity.ID;
            return View(opportunity);
        }

        // GET: Opportunity/Create
        public IActionResult Create()
        {
            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunity", Url = "/Opportunity/Index", IsActive = false },
                    new BreadcrumbItem { Title = "Create", Url = "/Oppurtunity/Create", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
            return View();
        }

        // POST: Opportunity/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,OpportunityName,OpportunityStatus,OpportunityPriority,OpportunityAction,OpportunityContact,OpportunityAccount,OpportunityLastContactDate,OpportunityInteractions,ContactID")] Opportunity opportunity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingopportunity = await _context.Opportunities
                        .FirstOrDefaultAsync(i => i.OpportunityName == opportunity.OpportunityName);

                    if (existingopportunity != null)
                    {
                        ModelState.AddModelError("OpportunityName", "opportunity with this OpportunityName already exists.");
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
            var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunity", Url = "/Opportunity/Index", IsActive = false },
                    new BreadcrumbItem { Title = opportunity.OpportunityName, Url = "#", IsActive = true }

                };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Opportunity"] = opportunity.ID;
            return View(opportunity);
        }

        // GET: Opportunity/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .Include(o => o.Contact)
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

            ViewData["Opportunity"] = opportunity.ID;
            return View(opportunity);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,OpportunityName,OpportunityStatus,OpportunityPriority,OpportunityAction,OpportunityLastContactDate,OpportunityInteractions,Contact")] Opportunity opportunity)
        {
            if (id != opportunity.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing opportunity with its contact
                    var existingOpportunity = await _context.Opportunities
                        .Include(o => o.Contact)
                        .FirstOrDefaultAsync(o => o.ID == id);

                    if (existingOpportunity == null)
                    {
                        return NotFound();
                    }

                    // Update opportunity fields
                    existingOpportunity.OpportunityName = opportunity.OpportunityName;
                    existingOpportunity.OpportunityStatus = opportunity.OpportunityStatus;
                    existingOpportunity.OpportunityPriority = opportunity.OpportunityPriority;
                    existingOpportunity.OpportunityAction = opportunity.OpportunityAction;
                    existingOpportunity.OpportunityLastContactDate = opportunity.OpportunityLastContactDate;
                    existingOpportunity.OpportunityInteractions = opportunity.OpportunityInteractions;

                    // Since Contact.Summary is read-only, we can't directly update it
                    // Instead, we need to maintain the relationship without trying to modify its properties

                    // Only update the Contact reference if needed
                    if (opportunity.Contact != null && opportunity.Contact.ID > 0)
                    {
                        // If the Contact ID is the same, we don't need to do anything
                        // If different, update the reference
                        if (existingOpportunity.Contact == null || existingOpportunity.Contact.ID != opportunity.Contact.ID)
                        {
                            // Attach the existing contact by ID
                            var contact = await _context.Contacts.FindAsync(opportunity.Contact.ID);
                            if (contact != null)
                            {
                                existingOpportunity.Contact = contact;
                            }
                        }
                    }

                    _context.Update(existingOpportunity);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Opportunity details updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = opportunity.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpportunityExists(opportunity.ID))
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
                    TempData["ErrorMessage"] = "An error occurred while updating the Opportunity details.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please check the input data and try again.";
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Opportunity", Url = "/Opportunity/Index", IsActive = false },
                new BreadcrumbItem { Title = opportunity.OpportunityName, Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["Opportunity"] = opportunity.ID;
            return View(opportunity);
        }


        // GET: Opportunity/Delete/5
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
                    new BreadcrumbItem { Title = opportunity.OpportunityName, Url = "#", IsActive = true }

                };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Opportunity"] = opportunity.ID;
            return View(opportunity);
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(Contact contact, int opportunityId)
        {
            if (ModelState.IsValid)
            {
                var opportunity = await _context.Opportunities.FindAsync(opportunityId);
                if (opportunity != null)
                {
                    _context.Contacts.Add(contact);
                    opportunity.Contact = contact;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Contact added successfully" });
                }
            }
            return Json(new { success = false, message = "Failed to add contact" });
        }



        // POST: Opportunity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
                // If Insudtry not found, set an error message
                TempData["ErrorMessage"] = "Opportunity not found!";
            }
            var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Opportunity", Url = "/Opportunity/Index", IsActive = false },
                    new BreadcrumbItem { Title = opportunity.OpportunityName, Url = "#", IsActive = true }

                };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Opportunity"] = opportunity.ID;
            // Redirect to the Index or other appropriate page
            return RedirectToAction(nameof(Index));
        }

        private bool OpportunityExists(int id)
        {
            return _context.Opportunities.Any(e => e.ID == id);
        }
    }
}
