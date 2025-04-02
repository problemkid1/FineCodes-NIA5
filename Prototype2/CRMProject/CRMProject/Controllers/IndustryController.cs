using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRMProject.Data;
using CRMProject.Models;
using OfficeOpenXml;
using CRMProject.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace CRMProject.Controllers
{
    [Authorize(Roles = "Super, Admin")]
    public class IndustryController : Controller
    {
        private readonly CRMContext _context;

        public IndustryController(CRMContext context)
        {
            _context = context;
        }

        // GET: Industry
        public async Task<IActionResult> Index(string? IndustrySector, string? IndustrySubsector, string? IndustryNAICSCode, int? page, int? pageSizeID)

        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            // Include related MemberIndustries and Member data for each Industry
            var industries = _context.Industries
                .Include(i => i.MemberIndustries)
                .ThenInclude(i => i.Member)
                .AsNoTracking(); // Eager loading with .Include and .ThenInclude

            // Filter by Industry Sector
            if (!string.IsNullOrEmpty(IndustrySector))
            {
                industries = industries.Where(i => i.IndustrySector.ToLower().Contains(IndustrySector.ToLower()));
                numberFilters++;
            }

            // Filter by Industry NAICS Code
            if (!string.IsNullOrEmpty(IndustryNAICSCode))
            {
                industries = industries.Where(i => i.IndustryNAICSCode.Contains(IndustryNAICSCode));
                numberFilters++;
            }

            // Filter by Industry Subsector
            if (!string.IsNullOrEmpty(IndustrySubsector))
            {
                industries = industries.Where(i => i.IndustrySubsector.ToLower().Contains(IndustrySubsector.ToLower()));
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
                    new BreadcrumbItem { Title = "Industry", Url = "/Industry/Index", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Industry>.CreateAsync(industries.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }



        // GET: Industry/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var industry = await _context.Industries
                     .Include(i => i.MemberIndustries).ThenInclude(i => i.Member)
                    .FirstOrDefaultAsync(m => m.ID == id);

                if (industry == null)
                {
                    return NotFound();
                }
                var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Industry", Url = "/Industry/Index", IsActive = false },
                    new BreadcrumbItem { Title = industry.Summary, Url = "#", IsActive = true }

                };

                ViewData["Breadcrumbs"] = breadcrumbs;

                ViewData["Industry"] = industry.ID;

                return View(industry);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while retrieving the industry details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Industry/Create
        public IActionResult Create()
        {
            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Industry", Url = "/Industry/Index", IsActive = false },
                    new BreadcrumbItem { Title = "Create", Url = "/Industry/Create", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewBag.SectorList = _context.Industries
                .Select(i => new { Value = i.IndustrySector, Text = i.IndustrySector })
                .Distinct()
                .OrderBy(i => i.Text)
                .ToList();
            ViewBag.SubsectorList = _context.Industries
                .Select(i => new { Value = i.IndustrySubsector, Text = i.IndustrySubsector })
                .Distinct()
                .OrderBy(i => i.Text)
                .ToList();

            return View();
        }

        // POST: Industry/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,IndustrySector,IndustrySubsector,IndustryNAICSCode")] Industry industry)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check for unique NAICS Code before adding
                    var existingIndustry = await _context.Industries
                        .FirstOrDefaultAsync(i => i.IndustryNAICSCode == industry.IndustryNAICSCode);

                    if (existingIndustry != null)
                    {
                        // Return a validation error if the NAICS code already exists
                        ModelState.AddModelError("IndustryNAICSCode", "Industry with this NAICS Code already exists.");
                        return View(industry);
                    }

                    // Add the new member to the context and save changes
                    _context.Add(industry);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Industry created successfully!";
                    return RedirectToAction(nameof(Details), new { id = industry.ID });
                }
                catch (DbUpdateException dex)
                {
                    // Directly handling the exception without ExceptionMessageVM
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("IndustryNAICSCode", "Unable to save changes. Remember, Industry NAICS Code must be unique.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while saving the Industry.";
                    }
                }
                catch (Exception)
                {
                    // General error handling
                    TempData["ErrorMessage"] = "An error occurred while creating the Industry.";
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
                    new BreadcrumbItem { Title = "Industry", Url = "/Industry/Index", IsActive = false },
                    new BreadcrumbItem { Title = industry.Summary, Url = "#", IsActive = true }

                };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Industry"] = industry.ID;
            // Return to the Create view in case of failure or validation errors
            return View(industry);
        }

        // GET: Industry/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var industry = await _context.Industries.FindAsync(id);
                if (industry == null)
                {
                    return NotFound();
                }
                var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Industry", Url = "/Industry/Index", IsActive = false },
                    new BreadcrumbItem { Title = industry.IndustrySubsector, Url = $"/Industry/Details/{id}", IsActive = false },
                    new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
                     
                

                };

                ViewData["Breadcrumbs"] = breadcrumbs;

                ViewData["Industry"] = industry.ID;

                ViewData["IndustrySector"] = industry.IndustrySector;

                ViewData["IndustrySubsector"] = industry.IndustrySubsector;

                ViewBag.SectorList = _context.Industries
                .Select(i => new { Value = i.IndustrySector, Text = i.IndustrySector })
                .Distinct()
                .OrderBy(i => i.Text)
                .ToList();

                ViewBag.SubsectorList = _context.Industries
                    .Select(i => new { Value = i.IndustrySubsector, Text = i.IndustrySubsector })
                    .Distinct()
                    .OrderBy(i => i.Text)
                    .ToList();
                return View(industry);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while fetching the industry for editing: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Industry/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,IndustrySector,IndustrySubsector,IndustryNAICSCode")] Industry industry)
        {
            if (id != industry.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check for unique NAICS Code before updating
                    var existingIndustry = await _context.Industries
                        .FirstOrDefaultAsync(i => i.IndustryNAICSCode == industry.IndustryNAICSCode && i.ID != industry.ID);

                    if (existingIndustry != null)
                    {
                        // Return a validation error if the NAICS code already exists
                        ModelState.AddModelError("IndustryNAICSCode", "Industry with this NAICS Code already exists.");
                        return View(industry);
                    }

                    _context.Update(industry);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Industry details updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = industry.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IndustryExists(industry.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {
                    // Directly handling the exception without ExceptionMessageVM
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("IndustryNAICSCode", "Unable to save changes. Remember, Industry NAICS Code must be unique.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while saving the Industry.";
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the industry details.";
                }
            }
            var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Industry", Url = "/Industry/Index", IsActive = false },
                    new BreadcrumbItem { Title = industry.IndustrySubsector, Url = $"/Industry/Details/{id}", IsActive = false },
                    new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }



                };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Industry"] = industry.ID;
            TempData["ErrorMessage"] = "Please check the input data and try again.";
            return View(industry); // Return to the edit view if there are validation errors
        }

        // GET: Industry/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var industry = await _context.Industries
                    .Include(i => i.MemberIndustries).ThenInclude(i => i.Member)
                    .FirstOrDefaultAsync(m => m.ID == id);

                if (industry == null)
                {
                    return NotFound();
                }
                var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Industry", Url = "/Industry/Index", IsActive = false },
                    new BreadcrumbItem { Title = industry.IndustrySubsector, Url =  $"/Industry/Details/{id}", IsActive = false },
                    new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }
                    
                

                };

                ViewData["Breadcrumbs"] = breadcrumbs;

                ViewData["Industry"] = industry.ID;
                return View(industry);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while fetching the industry for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Industry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var industry = await _context.Industries
                .Include(i => i.MemberIndustries)
                .ThenInclude(mi => mi.Member)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (industry == null)
            {
                TempData["ErrorMessage"] = "Industry not found!";
                return RedirectToAction(nameof(Index));
            }

            // Check if the industry has members
            if (industry.MemberIndustries.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete industry because it has associated member(s).";
                return RedirectToAction(nameof(Index));
            }

            // Proceed with deletion
            _context.Industries.Remove(industry);
            await _context.SaveChangesAsync();

            var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Industry", Url = "/Industry/Index", IsActive = false },
                    new BreadcrumbItem { Title = industry.IndustrySubsector, Url =  $"/Industry/Details/{id}", IsActive = false },
                    new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }



                };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["Industry"] = industry.ID;
            TempData["SuccessMessage"] = "Industry deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Add these methods to your IndustryController class

        [HttpGet]
        public JsonResult SearchIndustries(string term)
        {
            var industries = _context.Industries
                .Where(i => i.IndustrySector.Contains(term) ||
                            i.IndustrySubsector.Contains(term) ||
                            i.IndustryNAICSCode.Contains(term))
                .Select(i => new {
                    id = i.ID,
                    label = i.IndustrySector + " - " + i.IndustrySubsector + " (" + i.IndustryNAICSCode + ")"
                })
                .Take(10)
                .ToList();

            return Json(industries);
        }


        [HttpGet]
        public JsonResult GetAllIndustries()
        {
            var industries = _context.Industries
                .ToList() // Execute the query and bring data into memory
                .Select(i => new { id = i.ID, label = i.Summary }) // Now use Summary in memory
                .OrderBy(i => i.label)
                .ToList();

            return Json(industries);
        }




        /// <summary>
        /// Planted the base code
        /// </summary>
        /// <param name="theExcel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            string feedBack = string.Empty;
            if (theExcel == null)
            {
                feedBack = "Error: No file uploaded.";
                TempData["Feedback"] = feedBack;
                return RedirectToAction(nameof(Index));
            }

            string mimeType = theExcel.ContentType;
            long fileLength = theExcel.Length;

            if (string.IsNullOrEmpty(mimeType) || fileLength == 0)
            {
                feedBack = "Error: The uploaded file is empty.";
                TempData["Feedback"] = feedBack;
                return RedirectToAction(nameof(Index));
            }

            if (mimeType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
                mimeType != "application/vnd.ms-excel")
            {
                feedBack = "Error: Invalid file type. Please upload an Excel spreadsheet.";
                TempData["Feedback"] = feedBack;
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var memoryStream = new MemoryStream();
                await theExcel.CopyToAsync(memoryStream);

                using var excel = new ExcelPackage(memoryStream);
                var workSheet = excel.Workbook.Worksheets[0];

                if (workSheet == null || workSheet.Dimension == null)
                {
                    feedBack = "Error: Empty or unreadable Excel file.";
                    TempData["Feedback"] = feedBack;
                    return RedirectToAction(nameof(Index));
                }

                var start = workSheet.Dimension.Start;
                var end = workSheet.Dimension.End;

                // Validate Header
                if (workSheet.Cells[1, 1].Text.Trim() != "Industries Sector" ||
                    workSheet.Cells[1, 2].Text.Trim() != "Industry Subsector" ||
                    workSheet.Cells[1, 3].Text.Trim() != "NAICS Code")
                {
                    feedBack = "Error: Incorrect file format. Ensure headers are 'Industries Sector', 'Industry Subsector', 'NAICS Code'.";
                    TempData["Feedback"] = feedBack;
                    return RedirectToAction(nameof(Index));
                }

                int successCount = 0, errorCount = 0;
                var existingNaicsCodes = new HashSet<string>(_context.Industries.Select(i => i.IndustryNAICSCode));
                var newIndustries = new List<Industry>();

                for (int row = start.Row + 1; row <= end.Row; row++)
                {
                    try
                    {
                        string sector = workSheet.Cells[row, 1].Text?.Trim();
                        string naicsCode = workSheet.Cells[row, 3].Text?.Trim();
                        string subsector = workSheet.Cells[row, 2].Text?.Trim();

                        if (string.IsNullOrEmpty(sector) || string.IsNullOrEmpty(naicsCode) || string.IsNullOrEmpty(subsector))
                        {
                            errorCount++;
                            feedBack += $"Error: Row {row} has missing values.<br />";
                            continue;
                        }

                        if (existingNaicsCodes.Contains(naicsCode))
                        {
                            errorCount++;
                            feedBack += $"Error: NAICS Code {naicsCode} already exists. Skipping.<br />";
                            continue;
                        }

                        newIndustries.Add(new Industry
                        {
                            IndustrySector = sector,
                            IndustryNAICSCode = naicsCode,
                            IndustrySubsector = subsector
                        });

                        existingNaicsCodes.Add(naicsCode); // Add to the set to prevent future duplicates
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        feedBack += $"Error: Row {row} caused an exception - {ex.Message}.<br />";
                    }
                }

                // Bulk insert to improve performance
                if (newIndustries.Count > 0)
                {
                    _context.Industries.AddRange(newIndustries);
                    await _context.SaveChangesAsync();
                    successCount = newIndustries.Count;
                }

                feedBack += $"Import complete: {successCount} records inserted, {errorCount} records rejected.";
            }
            catch (Exception ex)
            {
                feedBack = $"Error: An unexpected error occurred - {ex.Message}.";
            }

            TempData["Feedback"] = feedBack + "<br /><br />";
            return RedirectToAction(nameof(Index));
        }


        private bool IndustryExists(int id)
        {
            return _context.Industries.Any(e => e.ID == id);
        }
    }
}
