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
using Humanizer;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;
using OfficeOpenXml;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Drawing.Printing;

namespace CRMProject.Controllers
{
    public class MemberController : Controller
    {
        private readonly CRMContext _context;

        public MemberController(CRMContext context)
        {
            _context = context;
        }

        // GET: Member
        public async Task<IActionResult> Index(string? SearchString, string? AddressCity, string? Contact ,int? MemberSize, int? page, int? pageSizeID, MemberStatus? MemberStatus, string? MembershipTypeName, DateTime StartDate, DateTime EndDate)
        {
            // Set date range if not specified
            if (EndDate == DateTime.MinValue)
            {
                StartDate = _context.Members.Min(m => m.MemberStartDate).Date;
                EndDate = _context.Members.Max(m => m.MemberStartDate).Date;
            }

            if (EndDate < StartDate)
            {
                DateTime temp = EndDate;
                EndDate = StartDate;
                StartDate = temp;
            }

            // Save to ViewData for form persistence
            ViewData["StartDate"] = StartDate.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = EndDate.ToString("yyyy-MM-dd");

            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            var members = _context.Members
                 .Include(p => p.MemberThumbnail)
                 .Include(m => m.Address)
                 .Include(m => m.MemberIndustries).ThenInclude(mi => mi.Industry)
                 .Include(m => m.MemberContacts).ThenInclude(mc => mc.Contact)
                 .Include(m => m.MemberMembershipTypes).ThenInclude(mmt => mmt.MembershipType)
                 .Where(m => m.MemberStartDate >= StartDate && m.MemberStartDate <= EndDate.AddDays(1))
                 .AsNoTracking();

            // Check if MemberStatus is provided (i.e., filter applied by user)
            if (MemberStatus.HasValue)
            {
                // Filter by the selected status (this overrides the default "GoodStanding")
                members = members.Where(m => m.MemberStatus == MemberStatus.Value);
                numberFilters++;
            }
            else
            {
                // Apply default filter for Good Standing if no status is provided
                members = members.Where(m => m.MemberStatus == 0);
            }

            //// Apply default filter for Good Standing if no MemberStatus is provided
            //if (!MemberStatus.HasValue)
            //{
            //    members = members.Where(m => m.MemberStatus == 0);
            //}

            // Filter by Member Name (case-insensitive)
            if (!string.IsNullOrEmpty(SearchString))
            {
                members = members.Where(m => m.MemberName.ToLower().Contains(SearchString.ToLower()));
                numberFilters++;
            }

            // Filter by City
            if (!string.IsNullOrEmpty(AddressCity))
            {
                members = members.Where(m => m.Address.AddressCity.ToLower() == AddressCity.ToLower());
                numberFilters++;
            }

            // Filter by City
            if (!string.IsNullOrEmpty(Contact))
            {
                members = members.Where(m => m.MemberContacts.Any(c => c.Contact.FirstName.ToLower().Contains(Contact.ToLower())));
                numberFilters++;
            }

            // Filter by Size
            if (MemberSize.HasValue)
            {
                members = members.Where(m => m.MemberSize == MemberSize.Value);
                numberFilters++;
            }

            // Filter by Status (overrides default if provided)
            //if (MemberStatus.HasValue)
            //{
            //    members = members.Where(m => m.MemberStatus == MemberStatus.Value);
            //    numberFilters++;
            //}

            // Filter by MembershipTypeName
            if (!string.IsNullOrEmpty(MembershipTypeName))
            {
                members = members.Where(m => m.MemberMembershipTypes
                                .Any(mmt => mmt.MembershipType.MembershipTypeName.ToLower() == MembershipTypeName.ToLower()));
                numberFilters++;
            }


            // Feedback for applied filters
            if (numberFilters != 0)
            {
                ViewData["Filtering"] = " btn-danger";
                ViewData["numberFilters"] = $"({numberFilters} Filter{(numberFilters > 1 ? "s" : "")} Applied)";
                ViewData["ShowFilter"] = " show";
            }

            ViewData["NewMemberCount"] = GetNewMemberCount();

            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Members", Url = "/Member/Index", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewBag.addressCityList = _context.Addresses
        .Select(a => new { Value = a.AddressCity, Text = a.AddressCity })
        .Distinct()
        .OrderBy(mt => mt.Text)
        .ToList();

            ViewBag.MembershipTypeNameList = _context.MembershipTypes
        .Select(mt => new { Value = mt.MembershipTypeName, Text = mt.MembershipTypeName })
        .Distinct()
        .OrderBy(mt => mt.Text)
        .ToList();

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Member>.CreateAsync(members.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(p => p.MemberPhoto)
                .Include(m => m.Address)
                .Include(m => m.MemberIndustries).ThenInclude(mi => mi.Industry)
                .Include(m => m.MemberContacts).ThenInclude(mi => mi.Contact)
                .Include(m => m.MemberMembershipTypes).ThenInclude(mi => mi.MembershipType)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (member == null)
            {
                return NotFound();
            }
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Members", Url = "/Member/Index", IsActive = false },
                new BreadcrumbItem { Title = member.MemberName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["MemberId"] = member.ID;

            return View(member);
        }


        // GET: Member/Create
        public IActionResult Create()
        {
            Member member = new Member
            {
                MemberStatus = MemberStatus.GoodStanding,
                MemberStartDate = DateTime.Today
            };
            PopulateAssignedIndustryData(member);
            PopulateAssignedMemberShipData(member);

            var breadcrumbs = new List<BreadcrumbItem>
                    {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Member", Url = "/Member/Index", IsActive = false },
                    new BreadcrumbItem { Title = "Create", Url = "/Member/Create", IsActive = true }
                    };

            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["MemberId"] = member.ID;
            return View();
        }

        // POST: Member/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Helper method added to your MemberController
        private int GetNewMemberCount()
        {
            int currentYear = DateTime.Now.Year;
            return _context.Members.Count(m => m.MemberStartDate.Year == currentYear);
        }

        // POST: Member/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ID,MemberName,MemberSize,MemberStatus,MemberAccountsPayableEmail,MemberStartDate,MemberEndDate,MemberLastContactDate,MemberNotes")] Member member,
            IFormFile? thePicture,
            string[] selectedMembership,
            string[] selectedIndustry)
        {
            // Update membership types and industries
            UpdateMemberMembershipTypes(selectedMembership, member);
            UpdateIndustry(selectedIndustry, member);

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle picture upload
                    await AddPicture(member, thePicture);

                    // Add the new member to the context and save changes
                    _context.Add(member);
                    await _context.SaveChangesAsync();

                    // Update the new member count for the current year
                    TempData["NewMemberCount"] = GetNewMemberCount();

                    // Set success message
                    TempData["SuccessMessage"] = "Member created successfully!";
                    return RedirectToAction(nameof(Details), new { id = member.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateException dex)
                {
                    string message = dex.GetBaseException().Message;
                    if (message.Contains("UNIQUE") && message.Contains("MemberName"))
                    {
                        ModelState.AddModelError("MemberName", "Unable to save changes. Remember, " +
                            "you cannot have duplicate Member Name .");
                    }
                    else if (message.Contains("UNIQUE") && message.Contains("MemberAccountsPayableEmail"))
                    {
                        ModelState.AddModelError("MemberAccountsPayableEmail", "Unable to save changes. Remember, " +
                            "you cannot have duplicate MemberAccountsPayableEmail .");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
                catch (Exception)
                {
                    // Set error message in case of failure
                    TempData["ErrorMessage"] = "An error occurred while creating the member.";
                }
            }
            else
            {
                // If model validation fails, set an error message
                TempData["ErrorMessage"] = "Please check the input data and try again.";
            }

            // Populate the assigned data for the view
            PopulateAssignedMemberShipData(member);
            PopulateAssignedIndustryData(member);

            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Members", Url = "/Member/Index", IsActive = false },
                new BreadcrumbItem { Title = member.MemberName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["MemberId"] = member.ID;

            // Return to the Create view in case of failure or validation errors
            return View(member);
        }

        // GET: Member/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var member = await _context.Members
               .Include(p => p.MemberPhoto)
                .Include(m => m.MemberIndustries).ThenInclude(mi => mi.Industry)
               .Include(d => d.MemberMembershipTypes).ThenInclude(d => d.MembershipType)
               .AsNoTracking()
               .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            PopulateAssignedMemberShipData(member);
            PopulateAssignedIndustryData(member);

            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Members", Url = "/Member/Index", IsActive = false },
                new BreadcrumbItem { Title = member.MemberName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["MemberId"] = member.ID;

            return View(member);
        }

        // POST: Member/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string? chkRemoveImage, IFormFile? thePicture,
    string[] selectedMembership, string[] selectedIndustry)
        {
            var memberToUpdate = await _context.Members
                .Include(p => p.MemberPhoto)
                .Include(m => m.MemberIndustries).ThenInclude(mi => mi.Industry)
                .Include(d => d.MemberMembershipTypes).ThenInclude(d => d.MembershipType)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (memberToUpdate == null)
            {
                return NotFound();
            }

            // Update membership types and industries separately
            UpdateMemberMembershipTypes(selectedMembership, memberToUpdate);
            UpdateIndustry(selectedIndustry, memberToUpdate);

            // Check the model state before updating the member
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["ErrorMessage"] = string.Join(" ", errorMessages);
                return View(memberToUpdate);
            }

            // Try updating the member with the values posted
            if (await TryUpdateModelAsync<Member>(memberToUpdate, "",
                m => m.MemberName, m => m.MemberSize, m => m.MemberStatus, m => m.MemberAccountsPayableEmail,
                m => m.MemberStartDate, m => m.MemberEndDate, m => m.MemberLastContactDate, m => m.MemberNotes))
            {
                try
                {
                    // Handle image deletion or upload
                    if (chkRemoveImage != null)
                    {
                        memberToUpdate.MemberThumbnail = _context.MemberThumbnails.Where(p => p.MemberID == memberToUpdate.ID).FirstOrDefault();
                        memberToUpdate.MemberPhoto = null;
                        memberToUpdate.MemberThumbnail = null;
                    }
                    else
                    {
                        await AddPicture(memberToUpdate, thePicture);
                    }

                    // Update member details in the database
                    _context.Update(memberToUpdate);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Member details updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = memberToUpdate.ID });
                }
                catch (DbUpdateException dex)
                {
                    string message = dex.GetBaseException().Message;
                    if (message.Contains("UNIQUE") && message.Contains("MemberName"))
                    {
                        ModelState.AddModelError("MemberName", "Unable to save changes. Remember, " +
                            "you cannot have duplicate Member Name .");
                    }
                    else if (message.Contains("UNIQUE") && message.Contains("MemberAccountsPayableEmail"))
                    {
                        ModelState.AddModelError("MemberAccountsPayableEmail", "Unable to save changes. Remember, " +
                            "you cannot have duplicate MemberAccountsPayableEmail .");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception message for debugging
                    Console.WriteLine(ex.Message);

                    TempData["ErrorMessage"] = "An error occurred while updating the member details.";
                }
            }

            // Populate the assigned data for the view
            PopulateAssignedMemberShipData(memberToUpdate);
            PopulateAssignedIndustryData(memberToUpdate);

            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Members", Url = "/Member/Index", IsActive = false },
                new BreadcrumbItem { Title = memberToUpdate.MemberName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["MemberId"] = memberToUpdate.ID;

            return View(memberToUpdate);
        }


        // GET: Member/CancelMember/5
        public async Task<IActionResult> Cancel(int? id)
        {

            if (id == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction("Index");
            }
            var member = await _context.Members
                .Include(p => p.MemberPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Members", Url = "/Member/Index", IsActive = false },
                new BreadcrumbItem { Title = member.MemberName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["MemberId"] = member.ID;

            return View(member);

        }

        // GET: Member/RemoveContact/5
        public async Task<IActionResult> RemoveContact(int contactID, int memberID)
        {
            var memberContact = await _context.MemberContacts
                .Include(mc => mc.Member)
                .Include(mc => mc.Contact)
                .FirstOrDefaultAsync(mc => mc.ContactID == contactID && mc.MemberID == memberID);

            if (memberContact == null)
            {
                TempData["ErrorMessage"] = "Contact relationship not found!";
                return RedirectToAction("Details", new { id = memberID });
            }

            return View(memberContact); // Show confirmation view
        }

        // POST: Member/RemoveContact/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveContactConfirmed(int contactID, int memberID)
        {
            var memberContact = await _context.MemberContacts
                .FirstOrDefaultAsync(mc => mc.ContactID == contactID && mc.MemberID == memberID);

            if (memberContact != null)
            {
                _context.MemberContacts.Remove(memberContact);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contact removed from the member successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Contact relationship not found!";
            }

            return RedirectToAction("Details", new { id = memberID });
        }

        public async Task<IActionResult> GetMemberContacts(int id)
        {
            var memberContacts = await _context.MemberContacts
                .Include(mc => mc.Contact)
                .Where(mc => mc.MemberID == id)
                .ToListAsync();

            return PartialView("_ListOfContacts", memberContacts);
        }


        // GET: Member/ActivateMember/5
        public async Task<IActionResult> Activate(int? id)
        {

            if (id == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction("Index");
            }
            var member = await _context.Members
                .Include(p => p.MemberPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }
            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Members", Url = "/Member/Index", IsActive = false },
                new BreadcrumbItem { Title = member.MemberName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["MemberId"] = member.ID;
            return View(member);

        }

        // POST: Member/CancelMember/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed([Bind("ID,Date,Status,Reason,Notes")] StatusHistory input)
        {
            var member = await _context.Members
                 .Include(m => m.MemberPhoto)
                 .FirstOrDefaultAsync(m => m.ID == input.ID);

            if (ModelState.IsValid)
            {
                if (member != null)
                {
                    member.MemberStatus = MemberStatus.Cancelled;  // Change status to Cancelled
                }                
                
                var cancellation = new StatusHistory
                {
                    MemberID = member.ID,
                    Date = input.Date,
                    Status = "Cancelled",
                    Reason = input.Reason,
                    Notes = input.Notes
                };
                _context.StatusHistories.Add(cancellation);

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Member '{member.MemberName}' cancelled successfully!";
                    return RedirectToAction("Index", "StatusHistory");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    TempData["ErrorMessage"] = "An error occurred while cancelling the member.";
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
                new BreadcrumbItem { Title = "Members", Url = "/Member/Index", IsActive = false },
                new BreadcrumbItem { Title = member.MemberName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["MemberId"] = member.ID;

            return View(member);
        }

        // POST: Member/ActivateMember/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed([Bind("ID,Date,Status,Reason,Notes")] StatusHistory input)
        {
            var member = await _context.Members
                 .Include(m => m.MemberPhoto)
                 .FirstOrDefaultAsync(m => m.ID == input.ID);

            if (ModelState.IsValid)
            {
                if (member != null)
                {
                    member.MemberStatus = MemberStatus.GoodStanding;  // Change status to Good Standing (Active)
                }
                
                var activation = new StatusHistory
                {
                    MemberID = member.ID,
                    Date = input.Date,
                    Status = "Good Standing",
                    Reason = input.Reason,
                    Notes = input.Notes
                };
                _context.StatusHistories.Add(activation);
                
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Member '{member.MemberName}' activated successfully!";
                    return RedirectToAction("Index", "StatusHistory");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    TempData["ErrorMessage"] = "An error occurred while activating the member.";
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
                new BreadcrumbItem { Title = "Members", Url = "/Member/Index", IsActive = false },
                new BreadcrumbItem { Title = member.MemberName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["MemberId"] = member.ID;

            return View(member);
        }
                
        private void PopulateAssignedMemberShipData(Member member)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.MembershipTypes.Select(mt => new { mt.ID, mt.MembershipTypeName }).ToList();
            var currentOptionsHS = new HashSet<int>(member.MemberMembershipTypes.Select(b => b.MembershipTypeID));
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
                        DisplayText = s.MembershipTypeName
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.MembershipTypeName
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");

            var breadcrumbs = new List<BreadcrumbItem>
             {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Members", Url = "/Member/Index", IsActive = false },
                new BreadcrumbItem { Title = member.MemberName, Url = "#", IsActive = true }
             };

            ViewData["Breadcrumbs"] = breadcrumbs;

            ViewData["MemberId"] = member.ID;
        }

        private void UpdateMemberMembershipTypes(string[] selectedOptions, Member memberToUpdate)
        {
            // Only initialize if null, don't clear existing data
            if (memberToUpdate.MemberMembershipTypes == null)
            {
                memberToUpdate.MemberMembershipTypes = new List<MemberMembershipType>();
            }

            if (selectedOptions == null || selectedOptions.Length == 0)
            {
                return; // Don't clear existing data
            }

            // Rest of the method remains the same
            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(memberToUpdate.MemberMembershipTypes.Select(b => b.MembershipTypeID));
            foreach (var s in _context.MembershipTypes)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString()))
                {
                    if (!currentOptionsHS.Contains(s.ID))
                    {
                        memberToUpdate.MemberMembershipTypes.Add(new MemberMembershipType
                        {
                            MembershipTypeID = s.ID,
                            MemberID = memberToUpdate.ID
                        });
                    }
                }
                else
                {
                    if (currentOptionsHS.Contains(s.ID))
                    {
                        MemberMembershipType? specToRemove = memberToUpdate.MemberMembershipTypes.FirstOrDefault(d => d.MembershipTypeID == s.ID);
                        if (specToRemove != null)
                        {
                            _context.Remove(specToRemove);
                        }
                    }
                }
            }
        }

        private void PopulateAssignedIndustryData(Member member)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.Industries;
            var currentOptionsHS = new HashSet<int>(member.MemberIndustries.Select(b => b.IndustryID));
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
                        DisplayText = s.Summary

                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.Summary
                    });
                }
            }

            ViewData["selOptsIndus"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOptsIndus"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }
        private void UpdateIndustry(string[] selectedOptions, Member memberToUpdate)
        {
            // Only initialize if null, don't clear existing data
            if (memberToUpdate.MemberIndustries == null)
            {
                memberToUpdate.MemberIndustries = new List<MemberIndustry>();
            }

            if (selectedOptions == null || selectedOptions.Length == 0)
            {
                return; // Don't clear existing data
            }

            // Rest of the method remains the same
            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(memberToUpdate.MemberIndustries.Select(b => b.IndustryID));
            foreach (var s in _context.Industries)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString()))
                {
                    if (!currentOptionsHS.Contains(s.ID))
                    {
                        memberToUpdate.MemberIndustries.Add(new MemberIndustry
                        {
                            IndustryID = s.ID,
                            MemberID = memberToUpdate.ID
                        });
                    }
                }
                else
                {
                    if (currentOptionsHS.Contains(s.ID))
                    {
                        MemberIndustry? specToRemove = memberToUpdate.MemberIndustries.FirstOrDefault(d => d.IndustryID == s.ID);
                        if (specToRemove != null)
                        {
                            _context.Remove(specToRemove);
                        }
                    }
                }
            }
        }
        private async Task AddPicture(Member member, IFormFile thePicture)
        {
            if (thePicture != null && thePicture.Length > 0 && thePicture.ContentType.StartsWith("image/"))
            {
                using var memoryStream = new MemoryStream();
                await thePicture.CopyToAsync(memoryStream);
                var pictureArray = memoryStream.ToArray();

                if (member.MemberPhoto != null)
                {
                    member.MemberPhoto.Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600);
                    member.MemberPhoto.MimeType = "image/webp";

                    member.MemberThumbnail = await _context.MemberThumbnails
                        .FirstOrDefaultAsync(m => m.MemberID == member.ID);
                    if (member.MemberThumbnail != null)
                    {
                        member.MemberThumbnail.Content = ResizeImage.ShrinkImageWebp(pictureArray, 75, 90);
                        member.MemberThumbnail.MimeType = "image/webp";
                    }
                }
                else
                {
                    member.MemberPhoto = new MemberPhoto
                    {
                        Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600),
                        MimeType = "image/webp"
                    };
                    member.MemberThumbnail = new MemberThumbnail
                    {
                        Content = ResizeImage.ShrinkImageWebp(pictureArray, 75, 90),
                        MimeType = "image/webp"
                    };
                }
            }
        }

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
                if (workSheet.Cells[1, 1].Text.Trim() != "Member Name" ||
                    workSheet.Cells[1, 2].Text.Trim() != "Size" ||
                    workSheet.Cells[1, 3].Text.Trim() != "Status"||
                    workSheet.Cells[1, 4].Text.Trim() != "Payable email" ||
                    workSheet.Cells[1, 5].Text.Trim() != "Start date")
                {
                    feedBack = "Error: Incorrect file format. Ensure headers are 'Member Name', 'Size', 'Status', 'Payable email', 'Start date'.";
                    TempData["Feedback"] = feedBack;
                    return RedirectToAction(nameof(Index));
                }

                int successCount = 0, errorCount = 0;
                var existingMemberName = new HashSet<string>(_context.Members.Select(i => i.MemberName));
                var existingMemberAccountsPayableEmail = new HashSet<string>(_context.Members.Select(i => i.MemberAccountsPayableEmail));
                var newMembers = new List<Member>();

                for (int row = start.Row + 1; row <= end.Row; row++)
                {
                    try
                    {
                        string name = workSheet.Cells[row, 1].Text?.Trim();
                        string sizeText = workSheet.Cells[row, 2].Text?.Trim();
                        string statusText = workSheet.Cells[row, 3].Text?.Trim();
                        string payableEmail = workSheet.Cells[row, 4].Text?.Trim();
                        string startDateText = workSheet.Cells[row, 5].Text?.Trim();

                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(sizeText) || string.IsNullOrEmpty(statusText) || string.IsNullOrEmpty(payableEmail) || string.IsNullOrEmpty(startDateText))
                        {
                            errorCount++;
                            feedBack += $"Error: Row {row} has missing values.<br />";
                            continue;
                        }

                        if (existingMemberName.Contains(name))
                        {
                            errorCount++;
                            feedBack += $"Error: Member Name {name} already exists. Skipping.<br />";
                            continue;
                        }
                        if (existingMemberAccountsPayableEmail.Contains(payableEmail))
                        {
                            errorCount++;
                            feedBack += $"Error: Member account's payable email {payableEmail} already exists. Skipping.<br />";
                            continue;
                        }
                        if (!int.TryParse(sizeText, out int memberSize))
                        {
                            errorCount++;
                            feedBack += $"Error: Row {row} has invalid size value. Must be a number.<br />";
                            continue;
                        }
                        if (!DateTime.TryParse(startDateText, out DateTime memberStartDate))
                        {
                            errorCount++;
                            feedBack += $"Error: Row {row} has invalid date format. Use yyyy-MM-dd format.<br />";
                            continue;
                        }
                        if (!Enum.TryParse<MemberStatus>(statusText, true, out MemberStatus memberStatus))
                        {
                            errorCount++;
                            feedBack += $"Error: Row {row} has invalid status value. Valid values are: {string.Join(", ", Enum.GetNames(typeof(MemberStatus)))}.<br />";
                            continue;
                        }

                        newMembers.Add(new Member
                        {
                            MemberName = name,
                            MemberSize = memberSize,
                            MemberStatus = memberStatus,
                            MemberStartDate = memberStartDate,
                            MemberAccountsPayableEmail = payableEmail
                        });

                        existingMemberName.Add(name);
                        existingMemberAccountsPayableEmail.Add(payableEmail);// Add to the set to prevent future duplicates
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        feedBack += $"Error: Row {row} caused an exception - {ex.Message}.<br />";
                    }
                }

                // Bulk insert to improve performance
                if (newMembers.Count > 0)
                {
                    _context.Members.AddRange(newMembers);  
                    await _context.SaveChangesAsync();
                    successCount = newMembers.Count;
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

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.ID == id);
        }
    }
}
