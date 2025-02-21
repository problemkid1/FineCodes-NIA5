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
        public async Task<IActionResult> Index(string? SearchString, int? MemberSize, MemberStatus? MemberStatus, MembershipTypeName? MembershipTypeName)
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;


            var members = _context.Members
                 .Include(p => p.MemberThumbnail)
                .Include(m => m.Address)
                .Include(m => m.MemberIndustries).ThenInclude(mi => mi.Industry)
                .Include(m => m.MemberContacts).ThenInclude(mc => mc.Contact)
                .Include(m => m.MemberMembershipTypes).ThenInclude(mmt => mmt.MembershipType)
                .AsNoTracking();

            // Filter by Name
            if (!string.IsNullOrEmpty(SearchString))
            {
                members = members.Where(m => m.MemberName.Contains(SearchString));
                numberFilters++;
            }

            // Filter by Size
            if (MemberSize.HasValue)
            {
                members = members.Where(m => m.MemberSize == MemberSize.Value);
                numberFilters++;
            }

            // Filter by Status
            if (MemberStatus.HasValue)
            {
                members = members.Where(m => m.MemberStatus == MemberStatus.Value);
                numberFilters++;
            }

            // Filter by MembershipTypeName
            if (MembershipTypeName.HasValue)
            {
                members = members.Where(m => m.MemberMembershipTypes
                                .Any(mmt => mmt.MembershipType.MembershipTypeName == MembershipTypeName.Value));
                numberFilters++;
            }
            //Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-danger";
                //Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
                //Keep the Bootstrap collapse open
                @ViewData["ShowFilter"] = " show";
            }

            return View(await members.ToListAsync());
        }


        // GET: Member/Details/5
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

            return View(member);
        }

        // GET: Member/Create
        public IActionResult Create()
        {
            Member member = new Member();
            PopulateAssignedMemberShipData(member);
            return View();
        }

        // POST: Member/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MemberName,MemberSize,MemberStatus,MemberAccountsPayableEmail,MemberStartDate,MemberEndDate,MemberNotes")] Member member, IFormFile? thePicture, string[] selectedOptions)
        {
                UpdateMemberMembershipTypes(selectedOptions, member);
                if (ModelState.IsValid)
                {
                    try
                    {
                        await AddPicture(member, thePicture);
                        // Add the new member to the context and save changes
                        _context.Add(member);
                        await _context.SaveChangesAsync();

                        // Set success message in TempData
                        TempData["SuccessMessage"] = "Member created successfully!";
                        return RedirectToAction(nameof(Details), new { id = member.ID });
                    }
                    
                    catch (RetryLimitExceededException /* dex */)
                    {
                        ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
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
            PopulateAssignedMemberShipData(member);
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
               .Include(d => d.MemberMembershipTypes).ThenInclude(d => d.MembershipType)
               .AsNoTracking()
               .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            PopulateAssignedMemberShipData(member);

            return View(member);
        }

        // POST: Member/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string? chkRemoveImage, IFormFile? thePicture, string[] selectedOptions)
        {
            var memberToUpdate = await _context.Members
                .Include(p => p.MemberPhoto)
                .Include(d => d.MemberMembershipTypes).ThenInclude(d => d.MembershipType)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (memberToUpdate == null)
            {
                return NotFound();
            }

            UpdateMemberMembershipTypes(selectedOptions, memberToUpdate);

            // Check the model state before updating the member
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["ErrorMessage"] = string.Join(" ", errorMessages);
                return View(memberToUpdate); // Return to the edit view if validation fails
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

                    // Update member details in the database without checking RowVersion
                    _context.Update(memberToUpdate);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Member details updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = memberToUpdate.ID });
                }
                catch (Exception ex)
                {
                    // Log the exception message for debugging
                    Console.WriteLine(ex.Message);

                    TempData["ErrorMessage"] = "An error occurred while updating the member details.";
                }
            }
            PopulateAssignedMemberShipData(memberToUpdate);
            return View(memberToUpdate); // Return to the edit view if the update fails
        }


        // GET: Member/CancelMember/5
        public async Task<IActionResult> CancelMember(int? id)
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
        [HttpPost, ActionName("CancelMember")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var member = await _context.Members
                .Include(m => m.MemberPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (member != null)
            {
<<<<<<< Updated upstream
                member.MemberStatus = MemberStatus.Cancelled;  // Change status to Cancelled
=======
                if (member != null)
                {
                    member.MemberStatus = MemberStatus.Cancelled;  // Change status to Cancelled
                }

                //Using a cancellation table

                // Check if a cancellation record already exists
                //bool cancellationExists = await _context.Cancellations.AnyAsync(c => c.MemberID == member.ID);

                //if (!cancellationExists)
                //{
                //    var cancellation = new Cancellation
                //    {
                //        MemberID = member.ID,
                //        CancellationDate = input.CancellationDate,
                //        CancellationReason = input.CancellationReason,
                //        CancellationNotes = input.CancellationNotes
                //    };

                //    _context.Cancellations.Add(cancellation);
                //}

                //Using a Status History table

                // Do not check if record already exists. Add to Status History anyways, so that the status change for that member is recorded
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
>>>>>>> Stashed changes
            }

            // Check if a cancellation record already exists
            bool cancellationExists = await _context.Cancellations.AnyAsync(c => c.MemberID == member.ID);

            if (!cancellationExists)
            {
                var cancellation = new Cancellation
                {
                    MemberID = member.ID,
                    CancellationDate = DateTime.Now,
                    CancellationReason = "Cancelled by user.",
                    CancellationNotes = "Member moved to cancellations."
                };

                _context.Cancellations.Add(cancellation);
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Member '{member.MemberName}' cancelled successfully!";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "An error occurred while cancelling the member.";
            }

            return RedirectToAction(nameof(Index));
        }

<<<<<<< Updated upstream
       
=======
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

                //SOLVE BUG...DELETE CANCELLED RECORD WHEN ACTIVATING IT IF USING CANCELLATION TABLE

                //Using a cancellation table

                // Check if a cancellation record already exists

                //var cancellation = await _context.Cancellations
                // .FirstOrDefaultAsync(m => m.ID == id);

                //bool cancellationExists = await _context.Cancellations.AnyAsync(c => c.ID == id);

                //if (cancellationExists)
                //{
                //    _context.Cancellations.Remove(cancellation);
                //}

                //Using a Status History table
                //Remember: change table name to Status History and add status collumn
                //Add to Status History anyways, so that the status changing is recorded
                //{
                var cancellation = new Cancellation
                {
                    MemberID = member.ID,
                    CancellationDate = input.CancellationDate,
                    CancellationReason = input.CancellationReason,
                    CancellationNotes = input.CancellationNotes
                };

                _context.Cancellations.Add(cancellation);
                //}
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


>>>>>>> Stashed changes
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
            if (selectedOptions == null)
            {
                memberToUpdate.MemberMembershipTypes = new List<MemberMembershipType>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(memberToUpdate.MemberMembershipTypes.Select(b => b.MembershipTypeID));
            foreach (var s in _context.MembershipTypes)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString()))//it is selected
                {
                    if (!currentOptionsHS.Contains(s.ID))//but not currently in the Member's collection - Add it!
                    {
                        memberToUpdate.MemberMembershipTypes.Add(new MemberMembershipType
                        {
                            MembershipTypeID = s.ID,
                            MemberID = memberToUpdate.ID
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(s.ID))//but is currently in the Member's collection - Remove it!
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
