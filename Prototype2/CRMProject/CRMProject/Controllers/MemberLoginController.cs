using CRMProject.CustomController;
using CRMProject.Data;
using CRMProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRMProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MemberLoginController : CognizantController
    {
        private readonly CRMContext _context;
        private readonly ApplicationDbContext _identityContext;
        private readonly UserManager<IdentityUser> _userManager;

        public MemberLoginController(CRMContext context,
           ApplicationDbContext identityContext,
           UserManager<IdentityUser> userManager)
        {
            _context = context;
            _identityContext = identityContext;
            _userManager = userManager;
        }

        public async Task <IActionResult> Index()
        {
            var memberslogin = await _context.MemberLogins
                .Include(e => e.Subscriptions)
 
             .Select(e => new MemberAdminVm
             {
                 Email = e.Email,
                 Active = e.Active,
                 ID = e.ID,
                 FirstName = e.FirstName,
                 LastName = e.LastName,
                 Phone = e.Phone
             }).ToListAsync();
            foreach (var e in memberslogin)
            {
                var user = await _userManager.FindByEmailAsync(e.Email);
                if (user != null)
                {
                    e.UserRoles = (List<string>)await _userManager.GetRolesAsync(user);
                }
            };
            return View(memberslogin);
        }
    }
}
