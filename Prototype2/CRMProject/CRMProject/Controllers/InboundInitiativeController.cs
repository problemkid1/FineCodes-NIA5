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
    [Authorize(Roles = "Admin")]
    public class InboundInitiativeController : Controller
    {
        private readonly CRMContext _context;

        public InboundInitiativeController(CRMContext context)
        {
            _context = context;
        }

        // GET: InboundInitiative
        public async Task<IActionResult> Index(int? page, int? pageSizeID)
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Inbound Initiatives", Url = "/InboundInitiative/Index", IsActive = true }
            };
            var inboundInitiatives = _context.InboundInitiatives.AsNoTracking();
            ViewData["Breadcrumbs"] = breadcrumbs;
            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<InboundInitiative>.CreateAsync(inboundInitiatives.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: InboundInitiative/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inboundInitiative = await _context.InboundInitiatives
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inboundInitiative == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Inbound Initiatives", Url = "/InboundInitiative/Index", IsActive = false },
                new BreadcrumbItem { Title = inboundInitiative.Initiative, Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["InboundInitiativeId"] = inboundInitiative.ID;

            return View(inboundInitiative);
        }

        // GET: InboundInitiative/Create
        public IActionResult Create()
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Inbound Initiatives", Url = "/InboundInitiative/Index", IsActive = false },
                new BreadcrumbItem { Title = "Create", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View();
        }

        // POST: InboundInitiative/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Initiative,InboundInitiativeNotes")] InboundInitiative inboundInitiative)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inboundInitiative);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Inbound Initiatives", Url = "/InboundInitiative/Index", IsActive = false },
                new BreadcrumbItem { Title = "Create", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(inboundInitiative);
        }

        // GET: InboundInitiative/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inboundInitiative = await _context.InboundInitiatives.FindAsync(id);
            if (inboundInitiative == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Inbound Initiatives", Url = "/InboundInitiative/Index", IsActive = false },
                new BreadcrumbItem { Title = inboundInitiative.Initiative, Url = $"/InboundInitiative/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(inboundInitiative);
        }

        // POST: InboundInitiative/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Initiative,InboundInitiativeNotes")] InboundInitiative inboundInitiative)
        {
            if (id != inboundInitiative.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inboundInitiative);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InboundInitiativeExists(inboundInitiative.ID))
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

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Inbound Initiatives", Url = "/InboundInitiative/Index", IsActive = false },
                new BreadcrumbItem { Title = inboundInitiative.Initiative, Url = $"/InboundInitiative/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(inboundInitiative);
        }

        // GET: InboundInitiative/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inboundInitiative = await _context.InboundInitiatives
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inboundInitiative == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Inbound Initiatives", Url = "/InboundInitiative/Index", IsActive = false },
                new BreadcrumbItem { Title = inboundInitiative.Initiative, Url = $"/InboundInitiative/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(inboundInitiative);
        }

        // POST: InboundInitiative/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inboundInitiative = await _context.InboundInitiatives.FindAsync(id);
            if (inboundInitiative != null)
            {
                _context.InboundInitiatives.Remove(inboundInitiative);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InboundInitiativeExists(int id)
        {
            return _context.InboundInitiatives.Any(e => e.ID == id);
        }
    }
}
