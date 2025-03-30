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
using System.ComponentModel.DataAnnotations;

namespace CRMProject.Controllers
{
    public class BrainDumpController : Controller
    {
        private readonly CRMContext _context;

        public BrainDumpController(CRMContext context)
        {
            _context = context;
        }

        // GET: BrainDump
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Index( int? page, int? pageSizeID)
        {
            // Add breadcrumbs
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Brain Dumps", Url = "/BrainDump/Index", IsActive = true }
            };
            var brainDumps = _context.BrainDumps
                .AsNoTracking();
            ViewData["Breadcrumbs"] = breadcrumbs;

            // Populate BrainDumpStatus list
            ViewData["BrainDumpStatusList"] = new SelectList(Enum.GetValues(typeof(BrainDumpStatus))
                .Cast<BrainDumpStatus>()
                .Select(s => new { Value = s, Text = GetEnumDisplayName(s) }),
                "Value", "Text");

            // Populate BrainDumpTerm list
            ViewData["BrainDumpTermList"] = new SelectList(Enum.GetValues(typeof(BrainDumpTerm))
                .Cast<BrainDumpTerm>()
                .Select(t => new { Value = t, Text = GetEnumDisplayName(t) }),
                "Value", "Text");
            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<BrainDump>.CreateAsync(brainDumps.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: BrainDump/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brainDump = await _context.BrainDumps
                .FirstOrDefaultAsync(m => m.ID == id);
            if (brainDump == null)
            {
                return NotFound();
            }

            // Add breadcrumbs
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Brain Dumps", Url = "/BrainDump/Index", IsActive = false },
                new BreadcrumbItem { Title = brainDump.Activity, Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["BrainDumpId"] = brainDump.ID;

            return View(brainDump);
        }

        // GET: BrainDump/Create
        public IActionResult Create()
        {
            // Add breadcrumbs
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Brain Dumps", Url = "/BrainDump/Index", IsActive = false },
                new BreadcrumbItem { Title = "Create", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            // Populate BrainDumpStatus list for the dropdown
            ViewData["BrainDumpStatusList"] = new SelectList(Enum.GetValues(typeof(BrainDumpStatus))
                .Cast<BrainDumpStatus>()
                .Select(s => new { Value = s, Text = GetEnumDisplayName(s) }),
                "Value", "Text");

            // Populate BrainDumpTerm list for the dropdown
            ViewData["BrainDumpTermList"] = new SelectList(Enum.GetValues(typeof(BrainDumpTerm))
                .Cast<BrainDumpTerm>()
                .Select(t => new { Value = t, Text = GetEnumDisplayName(t) }),
                "Value", "Text");

            return View();
        }

        // POST: BrainDump/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Activity,Assignee,BrainDumpStatus,BrainDumpTerm,BrainDumpNotes")] BrainDump brainDump)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brainDump);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Add breadcrumbs
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Brain Dumps", Url = "/BrainDump/Index", IsActive = false },
                new BreadcrumbItem { Title = "Create", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            // Repopulate dropdowns on validation error
            ViewData["BrainDumpStatusList"] = new SelectList(Enum.GetValues(typeof(BrainDumpStatus))
                .Cast<BrainDumpStatus>()
                .Select(s => new { Value = s, Text = GetEnumDisplayName(s) }),
                "Value", "Text", brainDump.BrainDumpStatus);

            ViewData["BrainDumpTermList"] = new SelectList(Enum.GetValues(typeof(BrainDumpTerm))
                .Cast<BrainDumpTerm>()
                .Select(t => new { Value = t, Text = GetEnumDisplayName(t) }),
                "Value", "Text", brainDump.BrainDumpTerm);

            return View(brainDump);
        }

        // GET: BrainDump/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brainDump = await _context.BrainDumps.FindAsync(id);
            if (brainDump == null)
            {
                return NotFound();
            }

            // Add breadcrumbs
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Brain Dumps", Url = "/BrainDump/Index", IsActive = false },
                new BreadcrumbItem { Title = brainDump.Activity, Url = $"/BrainDump/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            // Populate BrainDumpStatus list for the dropdown
            ViewData["BrainDumpStatusList"] = new SelectList(Enum.GetValues(typeof(BrainDumpStatus))
                .Cast<BrainDumpStatus>()
                .Select(s => new { Value = s, Text = GetEnumDisplayName(s) }),
                "Value", "Text", brainDump.BrainDumpStatus);

            // Populate BrainDumpTerm list for the dropdown
            ViewData["BrainDumpTermList"] = new SelectList(Enum.GetValues(typeof(BrainDumpTerm))
                .Cast<BrainDumpTerm>()
                .Select(t => new { Value = t, Text = GetEnumDisplayName(t) }),
                "Value", "Text", brainDump.BrainDumpTerm);

            return View(brainDump);
        }

        // POST: BrainDump/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Activity,Assignee,BrainDumpStatus,BrainDumpTerm,BrainDumpNotes")] BrainDump brainDump)
        {
            if (id != brainDump.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brainDump);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrainDumpExists(brainDump.ID))
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

            // Add breadcrumbs
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Brain Dumps", Url = "/BrainDump/Index", IsActive = false },
                new BreadcrumbItem { Title = brainDump.Activity, Url = $"/BrainDump/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            // Repopulate dropdowns on validation error
            ViewData["BrainDumpStatusList"] = new SelectList(Enum.GetValues(typeof(BrainDumpStatus))
                .Cast<BrainDumpStatus>()
                .Select(s => new { Value = s, Text = GetEnumDisplayName(s) }),
                "Value", "Text", brainDump.BrainDumpStatus);

            ViewData["BrainDumpTermList"] = new SelectList(Enum.GetValues(typeof(BrainDumpTerm))
                .Cast<BrainDumpTerm>()
                .Select(t => new { Value = t, Text = GetEnumDisplayName(t) }),
                "Value", "Text", brainDump.BrainDumpTerm);

            return View(brainDump);
        }

        // GET: BrainDump/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brainDump = await _context.BrainDumps
                .FirstOrDefaultAsync(m => m.ID == id);
            if (brainDump == null)
            {
                return NotFound();
            }

            // Add breadcrumbs
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Brain Dumps", Url = "/BrainDump/Index", IsActive = false },
                new BreadcrumbItem { Title = brainDump.Activity, Url = $"/BrainDump/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(brainDump);
        }

        // POST: BrainDump/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brainDump = await _context.BrainDumps.FindAsync(id);
            if (brainDump != null)
            {
                _context.BrainDumps.Remove(brainDump);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string GetEnumDisplayName<T>(T enumValue)
        {
            var field = typeof(T).GetField(enumValue.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                                 .FirstOrDefault() as DisplayAttribute;
            return attribute?.Name ?? enumValue.ToString();
        }
        private bool BrainDumpExists(int id)
        {
            return _context.BrainDumps.Any(e => e.ID == id);
        }
    }
}
