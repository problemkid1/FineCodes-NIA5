using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRMProject.Data;
using CRMProject.Models;
using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace CRMProject.Controllers
{
    public class StatusHistoryController : Controller
    {
        private readonly CRMContext _context;

        public StatusHistoryController(CRMContext context)
        {
            _context = context;
        }

        // GET: StatusHistory
        public async Task<IActionResult> Index(string? SearchString, DateTime? Date, string? Status, string? Reason, string? Notes, DateTime StartDate, DateTime EndDate)
        {
            // If first time loading the page, set date range based on DB values
            if (EndDate == DateTime.MinValue)
            {
                StartDate = _context.StatusHistories.Min(c => c.Date).Date;
                EndDate = _context.StatusHistories.Max(c => c.Date).Date;
            }

            // Swap dates if out of order
            if (EndDate < StartDate)
            {
                DateTime temp = EndDate;
                EndDate = StartDate;
                StartDate = temp;
            }

            // Pass dates to the view
            ViewData["StartDate"] = StartDate.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = EndDate.ToString("yyyy-MM-dd");


            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            var statusHistories = _context.StatusHistories.Include(c => c.Member)
                .Where(c => c.Date >= StartDate && c.Date <= EndDate.AddDays(1))
                .AsNoTracking(); // Eager loading the related Member entity

            // Filter by Member Name (case-insensitive)
            if (!string.IsNullOrEmpty(SearchString))
            {
                statusHistories = statusHistories.Where(m => m.Member.MemberName.ToLower().Contains(SearchString.ToLower()));
                numberFilters++;
            }
            // Filter by StatusHistory Date
            if (Date.HasValue)
            {
                statusHistories = statusHistories.Where(c => c.Date == Date.Value);
                numberFilters++;
            }

            // Filter by StatusHistory Status
            if (!string.IsNullOrEmpty(Status))
            {
                statusHistories = statusHistories.Where(c => c.Status.ToLower().Contains(Status.ToLower()));
                numberFilters++;
            }

            // Filter by StatusHistory Reason
            if (!string.IsNullOrEmpty(Reason))
            {
                statusHistories = statusHistories.Where(c => c.Reason.ToLower().Contains(Reason.ToLower()));
                numberFilters++;
            }

            // Filter by StatusHistory Notes
            if (!string.IsNullOrEmpty(Notes))
            {
                statusHistories = statusHistories.Where(c => c.Notes.ToLower().Contains(Notes.ToLower()));
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
                    new BreadcrumbItem { Title = "Status History", Url = "/Status History/Index", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
            // Return the filtered list of status histories with associated Member data
            return View(await statusHistories.ToListAsync());
        }


        // GET: StatusHistory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }                       

            var statusHistory = await _context.StatusHistories
                .Include(c => c.Member)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (statusHistory == null)
            {
                return NotFound();
            }
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Status History", Url = "/StatusHistory/Index", IsActive = false },
                new BreadcrumbItem { Title = statusHistory.Status, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Status History"] = statusHistory.ID;
            return View(statusHistory);
        }

        // GET: StatusHistory/Create
        public IActionResult Create()
        {
            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Status History", Url = "/StatusHistory/Index", IsActive = true },
                    new BreadcrumbItem { Title = "Create", Url = "/StatusHistory/Create", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberName");
            return View();
        }

        // POST: StatusHistory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Date,Status,Reason,Notes,MemberID")] StatusHistory statusHistory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Add the new statusHistory to the context and save changes
                    _context.Add(statusHistory);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Status History record created successfully!";
                    return RedirectToAction(nameof(Details), new { id = statusHistory.ID });
                }
                catch (Exception)
                {
                    // Set error message in case of failure
                    TempData["ErrorMessage"] = "An error occurred while creating status history.";
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
                new BreadcrumbItem { Title = "Status History", Url = "/StatusHistory/Index", IsActive = false },
                new BreadcrumbItem { Title = statusHistory.Status, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Status History"] = statusHistory.ID;
            // Return to the Create view in case of failure or validation errors
            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberName", statusHistory.MemberID);
            return View(statusHistory);
        }

        // GET: StatusHistory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusHistory = await _context.StatusHistories.FindAsync(id);
            if (statusHistory == null)
            {
                return NotFound();
            }

            // Get the Member Name from the database using the MemberID
            ViewBag.MemberName = _context.Members
                .Where(m => m.ID == statusHistory.MemberID)
                .Select(m => m.MemberName)
                .FirstOrDefault();

            ViewBag.Status = statusHistory.Status;

            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Status History", Url = "/StatusHistory/Index", IsActive = false },
                new BreadcrumbItem { Title = statusHistory.Status, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Status History"] = statusHistory.ID;

            //ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberName", statusHistory.MemberID);
            return View(statusHistory);
        }

        // POST: StatusHistory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Date,Reason,Notes,MemberID")] StatusHistory statusHistory)
        {
            if (id != statusHistory.ID)
            {
                return NotFound();
            }

            var existingStatusHistory = await _context.StatusHistories.AsNoTracking().FirstOrDefaultAsync(sh => sh.ID == id);

            if (existingStatusHistory == null)
            {
                return NotFound();
            }

            // Retain the original Status value
            statusHistory.Status = existingStatusHistory.Status;

            // Explicitly set MemberID to the existing MemberID in the database record
            statusHistory.MemberID = existingStatusHistory.MemberID;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statusHistory);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Status History updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = statusHistory.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusHistoryExists(statusHistory.ID))
                    {
                        // If the status history does not exist anymore, return NotFound
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
                    TempData["ErrorMessage"] = "An error occurred while updating status history.";
                }

            }
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Status History", Url = "/StatusHistory/Index", IsActive = false },
                new BreadcrumbItem { Title = statusHistory.Status, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Status History"] = statusHistory.ID;
            // Set error message in case the model is invalid
            TempData["ErrorMessage"] = "Please check the input data and try again.";

            // Get the Member Name from the database using the MemberID
            ViewBag.MemberName = _context.Members
                .Where(m => m.ID == statusHistory.MemberID)
                .Select(m => m.MemberName)
                .FirstOrDefault();

            // Get the Member Status from the database
            ViewBag.Status = statusHistory.Status;
            //ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberName", statusHistory.MemberID);
            return View(statusHistory);
        }

        // GET: StatusHistory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusHistory = await _context.StatusHistories
                .Include(c => c.Member)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (statusHistory == null)
            {
                return NotFound();
            }
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Status History", Url = "/StatusHistory/Index", IsActive = false },
                new BreadcrumbItem { Title = statusHistory.Status, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Status History"] = statusHistory.ID;
            return View(statusHistory);
        }

        // POST: StatusHistory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var statusHistory = await _context.StatusHistories.FindAsync(id);
            if (statusHistory != null)
            {
                _context.StatusHistories.Remove(statusHistory);
                await _context.SaveChangesAsync();

                // Set success message in TempData
                TempData["SuccessMessage"] = "Status History deleted successfully!";
            }
            else
            {
                // If Address not found, set an error message
                TempData["ErrorMessage"] = "Status History not found!";
            }
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Status History", Url = "/StatusHistory/Index", IsActive = false },
                new BreadcrumbItem { Title = statusHistory.Status, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Status History"] = statusHistory.ID;
            // Redirect to the Index or other appropriate page
            return RedirectToAction(nameof(Index));
        }

        private bool StatusHistoryExists(int id)
        {
            return _context.StatusHistories.Any(e => e.ID == id);
        }
    }
}
