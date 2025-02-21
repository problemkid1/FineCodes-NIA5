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
        public async Task<IActionResult> Index(string? SearchString, int? MemberSize, MemberStatus? MemberStatus, MembershipTypeName? MembershipTypeName, DateTime StartDate, DateTime EndDate)
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
            if (MembershipTypeName.HasValue)
            {
                members = members.Where(m => m.MemberMembershipTypes
                                .Any(mmt => mmt.MembershipType.MembershipTypeName == MembershipTypeName.Value));
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

            return View(await members.ToListAsync());
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

            ViewData["MemberId"] = member.ID;

            return View(member);
        }


        // GET: Member/Create
        public IActionResult Create()
        {
            Member member = new Member();
            PopulateAssignedIndustryData(member);
            PopulateAssignedMemberShipData(member);
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
            [Bind("ID,MemberName,MemberSize,MemberStatus,MemberAccountsPayableEmail,MemberStartDate,MemberEndDate,MemberNotes")] Member member,
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

            // Return to the Create view in case of failure or validation errors
            return View(member);
        }









    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> Create(
    //[Bind("ID,MemberName,MemberSize,MemberStatus,MemberAccountsPayableEmail,MemberStartDate,MemberEndDate,MemberNotes")] Member member,
    //IFormFile? thePicture,
    //string[] selectedMembership,
    //string[] selectedIndustry)
    //    {
    //        // Update membership types and industries
    //        UpdateMemberMembershipTypes(selectedMembership, member);
    //        UpdateIndustry(selectedIndustry, member);

    //        if (ModelState.IsValid)
    //        {
    //            try
    //            {
    //                // Handle picture upload
    //                await AddPicture(member, thePicture);

    //                // Add the new member to the context and save changes
    //                _context.Add(member);
    //                await _context.SaveChangesAsync();

    //                // Set success message
    //                TempData["SuccessMessage"] = "Member created successfully!";
    //                return RedirectToAction(nameof(Details), new { id = member.ID });
    //            }
    //            catch (RetryLimitExceededException /* dex */)
    //            {
    //                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
    //            }
    //            catch (Exception)
    //            {
    //                // Set error message in case of failure
    //                TempData["ErrorMessage"] = "An error occurred while creating the member.";
    //            }
    //        }
    //        else
    //        {
    //            // If model validation fails, set an error message
    //            TempData["ErrorMessage"] = "Please check the input data and try again.";
    //        }

    //        // Populate the assigned data for the view
    //        PopulateAssignedMemberShipData(member);
    //        PopulateAssignedIndustryData(member);

    //        // Return to the Create view in case of failure or validation errors
    //        return View(member);
    //    }

    //    private int GetNewMemberCount()
    //    {
    //        int currentYear = DateTime.Now.Year;
    //        return _context.Members.Count(m => m.MemberStartDate.Year == currentYear);
    //    }


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

            return View(member);

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

            return View(member);

        }

        // POST: Member/CancelMember/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed([Bind("ID,CancellationDate,CancellationReason,CancellationNotes")] Cancellation input)
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
                
                var cancellation = new Cancellation
                {
                    MemberID = member.ID,
                    CancellationDate = input.CancellationDate,
                    CancellationReason = input.CancellationReason,
                    CancellationNotes = input.CancellationNotes
                };
                _context.Cancellations.Add(cancellation);

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Member '{member.MemberName}' cancelled successfully!";
                    return RedirectToAction("Index", "Cancellation");
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
            return View(member);
        }

        // POST: Member/ActivateMember/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed([Bind("ID,CancellationDate,MemberStatus,CancellationReason,CancellationNotes")] Cancellation input)
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
                
                var cancellation = new Cancellation
                {
                    MemberID = member.ID,
                    CancellationDate = input.CancellationDate,
                    CancellationReason = input.CancellationReason,
                    CancellationNotes = input.CancellationNotes
                };
                _context.Cancellations.Add(cancellation);
                
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Member '{member.MemberName}' activated successfully!";
                    return RedirectToAction("Index", "Cancellation");
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
            return View(member);
        }


        private void PopulateAssignedMemberShipData(Member member)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = Enum.GetValues(typeof(MembershipTypeName))
                         .Cast<MembershipTypeName>();
            var currentOptionsHS = new HashSet<int>(
        member.MemberMembershipTypes
              .Select(b => (int)b.MembershipType.MembershipTypeName)
              );
            //Instead of one list with a boolean, we will make two lists
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach (var s in allOptions)
            {
                var displayText = s.GetDisplayName();
                if (currentOptionsHS.Contains((int)s))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = (int)s, // Store Enum as string
                        DisplayText = displayText
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = (int)s,
                        DisplayText = displayText
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
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
            //Get the picture and save it with the Member (2 sizes)
            if (thePicture != null)
            {
                string mimeType = thePicture.ContentType;
                long fileLength = thePicture.Length;
                if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                {
                    if (mimeType.Contains("image"))
                    {
                        using var memoryStream = new MemoryStream();
                        await thePicture.CopyToAsync(memoryStream);
                        var pictureArray = memoryStream.ToArray();//Gives us the Byte[]

                        //Check if we are replacing or creating new
                        if (member.MemberPhoto != null)
                        {
                            //We already have pictures so just replace the Byte[]
                            member.MemberPhoto.Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600);

                            //Get the Thumbnail so we can update it.  Remember we didn't include it
                            member.MemberThumbnail = _context.MemberThumbnails.Where(m => m.MemberID == member.ID).FirstOrDefault();
                            if (member.MemberThumbnail != null)
                            {
                                member.MemberThumbnail.Content = ResizeImage.ShrinkImageWebp(pictureArray, 75, 90);
                            }
                        }
                        else //No pictures saved so start new
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
            }
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.ID == id);
        }
    }
}
