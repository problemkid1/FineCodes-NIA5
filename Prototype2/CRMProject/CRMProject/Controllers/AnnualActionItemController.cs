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
    public class AnnualActionItemController : Controller
    {
        private readonly CRMContext _context;

        public AnnualActionItemController(CRMContext context)
        {
            _context = context;
        }

        // GET: AnnualActionItem
        public async Task<IActionResult> Index()
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Annual Action Items", Url = "/AnnualActionItem/Index", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(await _context.AnnualActionItems.ToListAsync());
        }

        // GET: AnnualActionItem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annualActionItem = await _context.AnnualActionItems
                .FirstOrDefaultAsync(m => m.ID == id);
            if (annualActionItem == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Annual Action Items", Url = "/AnnualActionItem/Index", IsActive = false },
                new BreadcrumbItem { Title = annualActionItem.ActionItem, Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["AnnualActionItemId"] = annualActionItem.ID;

            return View(annualActionItem);
        }

        // GET: AnnualActionItem/Create
        public IActionResult Create()
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Annual Action Items", Url = "/AnnualActionItem/Index", IsActive = false },
                new BreadcrumbItem { Title = "Create", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View();
        }

        // POST: AnnualActionItem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ActionItem,Assignee,DueDate,Status,Notes")] AnnualActionItem annualActionItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(annualActionItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Annual Action Items", Url = "/AnnualActionItem/Index", IsActive = false },
                new BreadcrumbItem { Title = "Create", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(annualActionItem);
        }

        // GET: AnnualActionItem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annualActionItem = await _context.AnnualActionItems.FindAsync(id);
            if (annualActionItem == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Annual Action Items", Url = "/AnnualActionItem/Index", IsActive = false },
                new BreadcrumbItem { Title = annualActionItem.ActionItem, Url = $"/AnnualActionItem/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(annualActionItem);
        }

        // POST: AnnualActionItem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ActionItem,Assignee,DueDate,Status,Notes")] AnnualActionItem annualActionItem)
        {
            if (id != annualActionItem.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(annualActionItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnualActionItemExists(annualActionItem.ID))
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
                new BreadcrumbItem { Title = "Annual Action Items", Url = "/AnnualActionItem/Index", IsActive = false },
                new BreadcrumbItem { Title = annualActionItem.ActionItem, Url = $"/AnnualActionItem/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(annualActionItem);
        }

        // GET: AnnualActionItem/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annualActionItem = await _context.AnnualActionItems
                .FirstOrDefaultAsync(m => m.ID == id);
            if (annualActionItem == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Annual Action Items", Url = "/AnnualActionItem/Index", IsActive = false },
                new BreadcrumbItem { Title = annualActionItem.ActionItem, Url = $"/AnnualActionItem/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(annualActionItem);
        }

        // POST: AnnualActionItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var annualActionItem = await _context.AnnualActionItems.FindAsync(id);
            if (annualActionItem != null)
            {
                _context.AnnualActionItems.Remove(annualActionItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnnualActionItemExists(int id)
        {
            return _context.AnnualActionItems.Any(e => e.ID == id);
        }
    }
}
