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
        public async Task<IActionResult> Index(string? SearchString, int? MemberSize, MemberStatus? MemberStatus)
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
            return View();
        }

        // POST: Member/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MemberName,MemberSize,MemberStatus,MemberAccountsPayableEmail,MemberStartDate,MemberEndDate,MemberNotes")] Member member, IFormFile? thePicture)
        {
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
               .AsNoTracking()
               .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Member/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string? chkRemoveImage, IFormFile? thePicture)
        {
            var memberToUpdate = await _context.Members
                .Include(p => p.MemberPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (memberToUpdate == null)
            {
                return NotFound();
            }

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

            return View(memberToUpdate); // Return to the edit view if the update fails
        }





        // GET: Member/Archive/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
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

        // POST: Member/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            var member = await _context.Members
                .Include(p => p.MemberPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();

                // Set success message in TempData
                TempData["SuccessMessage"] = "Member archived successfully!";
            }
            else
            {
                // If member not found, set an error message
                TempData["ErrorMessage"] = "Member not found!";
            }

            // Redirect to the Index or other appropriate page
            return RedirectToAction(nameof(Index));
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
