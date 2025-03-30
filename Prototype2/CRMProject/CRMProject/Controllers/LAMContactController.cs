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
    public class LAMContactController : Controller
    {
        private readonly CRMContext _context;

        public LAMContactController(CRMContext context)
        {
            _context = context;
        }

        // GET: LAMContact
        public async Task<IActionResult> Index( int? page, int? pageSizeID)
        {
            var cRMContext = _context.LAMContacts.Include(l => l.Contact);

            var breadcrumbs = new List<BreadcrumbItem>
            {
            new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
            new BreadcrumbItem { Title = "LAMContacts", Url = "/LAMContact/Index", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;
            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<LAMContact>.CreateAsync(cRMContext.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);


        }

        // GET: LAMContact/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAMContact = await _context.LAMContacts
                .Include(l => l.Contact)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (lAMContact == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "LAMContacts", Url = "/LAMContact/Index", IsActive = false },
                new BreadcrumbItem { Title = lAMContact.Contact.FirstName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["LAMContactsId"] = lAMContact.ID;

            return View(lAMContact);
        }

        // GET: LAMContact/Create
        public IActionResult Create()
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "LAMContacts", Url = "/LAMContact/Index", IsActive = false },
                new BreadcrumbItem { Title = "Create", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["ContactID"] = new SelectList(
                _context.Contacts
                    .OrderBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .Select(c => new
                    {
                        ID = c.ID,
                        FullName = c.FirstName + " " + c.LastName
                    }),
                "ID",
                "FullName"
            );

            return View();
        }

        // POST: LAMContact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Municipality,Position,Notes,ContactID")] LAMContact lAMContact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lAMContact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "LAMContacts", Url = "/LAMContact/Index", IsActive = false },
                new BreadcrumbItem { Title = "Create", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["ContactID"] = new SelectList(
                _context.Contacts
                    .OrderBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .Select(c => new
                    {
                        ID = c.ID,
                        FullName = c.FirstName + " " + c.LastName
                    }),
                "ID",
                "FullName",
                lAMContact.ContactID
            );

            return View(lAMContact);
        }

        // GET: LAMContact/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAMContact = await _context.LAMContacts
                .Include(l => l.Contact)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (lAMContact == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "LAMContacts", Url = "/LAMContact/Index", IsActive = false },
                new BreadcrumbItem { Title = lAMContact.Contact.FirstName, Url = $"/LAMContact/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["ContactID"] = new SelectList(
                _context.Contacts
                    .OrderBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .Select(c => new
                    {
                        ID = c.ID,
                        FullName = c.FirstName + " " + c.LastName
                    }),
                "ID",
                "FullName",
                lAMContact.ContactID
            );

            return View(lAMContact);
        }

        // POST: LAMContact/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Municipality,Position,Notes,ContactID")] LAMContact lAMContact)
        {
            if (id != lAMContact.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lAMContact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LAMContactExists(lAMContact.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Get contact information for breadcrumbs
            var contact = await _context.Contacts.FindAsync(lAMContact.ContactID);

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "LAMContacts", Url = "/LAMContact/Index", IsActive = false },
                new BreadcrumbItem { Title = contact?.FirstName ?? "Contact", Url = $"/LAMContact/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["ContactID"] = new SelectList(
                _context.Contacts
                    .OrderBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .Select(c => new
                    {
                        ID = c.ID,
                        FullName = c.FirstName + " " + c.LastName
                    }),
                "ID",
                "FullName",
                lAMContact.ContactID
            );

            return View(lAMContact);
        }

        // GET: LAMContact/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAMContact = await _context.LAMContacts
                .Include(l => l.Contact)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (lAMContact == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "LAMContacts", Url = "/LAMContact/Index", IsActive = false },
                new BreadcrumbItem { Title = lAMContact.Contact.FirstName, Url = $"/LAMContact/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(lAMContact);
        }

        // POST: LAMContact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lAMContact = await _context.LAMContacts.FindAsync(id);
            if (lAMContact != null)
            {
                _context.LAMContacts.Remove(lAMContact);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LAMContactExists(int id)
        {
            return _context.LAMContacts.Any(e => e.ID == id);
        }
    }
}
