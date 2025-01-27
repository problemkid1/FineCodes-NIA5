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
using CRMProject.Data.CRMMigrations;

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Members
                .Include(m=>m.MemberThumbnail)
                .ToListAsync());
        }

        // GET: Member/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Addresses)
                .Include(m => m.MemberIndustries).ThenInclude(mi => mi.Industry)
                .Include(m => m.MemberContacts).ThenInclude(mi => mi.Contact).ThenInclude(mi => mi.ContactEmails)
                .Include(m=>m.MemberPhoto)
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
        //////
        ///Inside the Bind below, MemberPhoto is included. I don't know any other way to include it.///
        //////
        public async Task<IActionResult> Create([Bind("ID,MemberName,MemberSize,MemberStatus,MemberAccountsPayableEmail,MemberStartDate,MemberEndDate,MemberNotes,MemberPhoto")] Member member, IFormFile? thePicture)
        {
            if (ModelState.IsValid)
            {
                await AddPicture(member, thePicture);
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
                .Include(m => m.MemberPhoto)
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
        //////
        ///Inside the Bind below, MemberPhoto is included. I don't know any other way to include it.///
        //////
        public async Task<IActionResult> Edit(int id, [Bind("ID,MemberName,MemberSize,MemberStatus,MemberAccountsPayableEmail,MemberStartDate,MemberEndDate,MemberNotes,MemberPhoto")] Member member, string? chkRemoveImage, IFormFile? thePicture)
        {
            if (id != member.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //For the image
                    if (chkRemoveImage != null)
                    {
                        //If we are just deleting the two versions of the photo, we need to make sure the Change Tracker knows
                        //about them both so go get the Thumbnail since we did not include it.
                        member.MemberThumbnail = _context.MemberThumbnails.Where(m => m.MemberID == member.ID).FirstOrDefault();
                        //Then, setting them to null will cause them to be deleted from the database.
                        member.MemberPhoto = null;
                        member.MemberThumbnail = null;
                    }
                    else
                    {
                        await AddPicture(member, thePicture);
                    }

                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.ID))
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
            return View(member);
        }

        // GET: Member/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MemberPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task AddPicture(Member member, IFormFile thePicture)
        {
            //Get the picture and save it with the Patient (2 sizes)
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
                            member.MemberThumbnail = _context.MemberThumbnails.Where(p => p.MemberID == member.ID).FirstOrDefault();
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
