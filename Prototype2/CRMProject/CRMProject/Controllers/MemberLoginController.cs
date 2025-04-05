using CRMProject.CustomController;
using CRMProject.Data;
using CRMProject.Models;
using CRMProject.Utilities;
using CRMProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CRMProject.Controllers
{
    [Authorize(Roles = "Admin, Super")]
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

        public async Task<IActionResult> Index()
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Member Logins", Url = "/MemberLogin/Index", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

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
            }
            ;
            return View(memberslogin);
        }

        // GET: MemberLogin/Create
        public IActionResult Create()
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Member Logins", Url = "/MemberLogin/Index", IsActive = false },
                new BreadcrumbItem { Title = "Create", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            MemberAdminVm memberAdmin = new MemberAdminVm();


            var allRoles = _identityContext.Roles.ToList();
            var currentRoles = memberAdmin.UserRoles ?? new List<string>();
            var viewModel = new List<RoleVM>();

            foreach (var role in allRoles)
            {
                if (User.IsInRole("Admin") && role.Name == "User")
                {
                    viewModel.Add(new RoleVM
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        Assigned = currentRoles.Contains(role.Name)
                    });
                }
                else if (User.IsInRole("Super") && (role.Name == "User" || role.Name == "Admin"))
                {
                    viewModel.Add(new RoleVM
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        Assigned = currentRoles.Contains(role.Name)
                    });
                }
            }
            ViewBag.Roles = viewModel;

            return View(memberAdmin);
        }

        //POST: MemberLogin/Create
        //To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Phone,Email")] MemberLogin memberlogin, string selectedRole)
        {
            var breadcrumbs = new List<BreadcrumbItem>
    {
        new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
        new BreadcrumbItem { Title = "Member Logins", Url = "/MemberLogin/Index", IsActive = false },
        new BreadcrumbItem { Title = "Create", Url = "#", IsActive = true }
    };

            ViewData["Breadcrumbs"] = breadcrumbs;

            try
            {
                if (ModelState.IsValid)
                {
                    if (selectedRole == "Super")
                    {
                        ModelState.AddModelError("Role", "You cannot assign the Super role.");
                        return View(memberlogin);
                    }

                    if (User.IsInRole("Admin") && selectedRole != "User")
                    {
                        ModelState.AddModelError("Role", "Admins can only assign the User role.");
                        return View(memberlogin);
                    }

                    _context.Add(memberlogin);
                    await _context.SaveChangesAsync();

                    // Default role to "User"
                    //string[] selectedRoles = new string[] { "User" };

                    string[] selectedRoles = new string[] { selectedRole };
                    InsertIdentityUser(memberlogin.Email, selectedRoles);

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                {
                    ModelState.AddModelError("Email", "Unable to save changes. Remember, you cannot have duplicate Email addresses.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            MemberAdminVm memberAdminVm = new MemberAdminVm
            {
                Email = memberlogin.Email,
                Active = memberlogin.Active,
                ID = memberlogin.ID,
                FirstName = memberlogin.FirstName,
                LastName = memberlogin.LastName,
                Phone = memberlogin.Phone
            };
            PopulateAssignedRoleData(memberAdminVm);
            return View(memberAdminVm);
        }

        // GET: MemberLogin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberLogin = await _context.MemberLogins
                .Where(e => e.ID == id)
                .Select(e => new MemberAdminVm
                {
                    Email = e.Email,
                    Active = e.Active,
                    ID = e.ID,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Phone = e.Phone
                }).FirstOrDefaultAsync();

            if (memberLogin == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Member Logins", Url = "/MemberLogin/Index", IsActive = false },
                new BreadcrumbItem { Title = $"{memberLogin.FirstName} {memberLogin.LastName}", Url = $"/MemberLogin/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            //Get the user from the Identity system
            var user = await _userManager.FindByEmailAsync(memberLogin.Email);

            var currentUser = await _userManager.GetUserAsync(User);
            string currentEmail = currentUser?.Email;
            bool isEditingSelf = currentEmail == memberLogin.Email;

            if (user != null)
            {
                //Add the current roles
                var r = await _userManager.GetRolesAsync(user);
                memberLogin.UserRoles = (List<string>)r;

                // Restrict any edit operations on Super users
                if (memberLogin.UserRoles.Contains("Super"))
                {
                    TempData["ErrorMessage"] = "You are not authorized to edit a Super user.";
                    return RedirectToAction(nameof(Index));
                }

                var targetUser = await _userManager.FindByEmailAsync(memberLogin.Email);
                var targetRoles = targetUser != null ? await _userManager.GetRolesAsync(targetUser) : new List<string>();

                bool isTargetAdmin = targetRoles.Contains("Admin");
                bool isTargetSuper = targetRoles.Contains("Super");

                if (User.IsInRole("Admin"))
                {
                    // Allow Admin to edit themselves, but:
                    if (!isEditingSelf && (isTargetAdmin || isTargetSuper))
                    {
                        //Admin trying to edit another Admin or a Super
                        TempData["ErrorMessage"] = "You are not authorized to edit this user.";
                        return RedirectToAction(nameof(Index));
                    }

                    if (isEditingSelf && isTargetSuper)
                    {
                        // Extra safety check: should never happen, but just in case
                        TempData["ErrorMessage"] = "You are not authorized to edit a Super user.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            var allRoles = _identityContext.Roles.ToList();
            var currentRoles = memberLogin.UserRoles ?? new List<string>();
            var viewModel = new List<RoleVM>();

            foreach (var role in allRoles)
            {
                if (User.IsInRole("Admin") && role.Name == "User")
                {
                    viewModel.Add(new RoleVM
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        Assigned = currentRoles.Contains(role.Name)
                    });
                }
                else if (User.IsInRole("Super") && (role.Name == "User" || role.Name == "Admin"))
                {
                    viewModel.Add(new RoleVM
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        Assigned = currentRoles.Contains(role.Name)
                    });
                }
            }

            ViewBag.Roles = viewModel;

            return View(memberLogin);
        }

        // POST: MemberLogin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool Active, string selectedRole)
        {
            var memberLoginToUpdate = await _context.MemberLogins
                .FirstOrDefaultAsync(m => m.ID == id);
            if (memberLoginToUpdate == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Member Logins", Url = "/MemberLogin/Index", IsActive = false },
                new BreadcrumbItem { Title = $"{memberLoginToUpdate.FirstName} {memberLoginToUpdate.LastName}", Url = $"/MemberLogin/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Edit", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            bool ActiveStatus = memberLoginToUpdate.Active;
            string databaseEmail = memberLoginToUpdate.Email;

            var currentUser = await _userManager.GetUserAsync(User);
            string currentEmail = currentUser?.Email;
            bool isEditingSelf = currentEmail == memberLoginToUpdate.Email;

            var user = await _userManager.FindByEmailAsync(memberLoginToUpdate.Email);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Super"))
                {
                    TempData["ErrorMessage"] = "You are not authorized to edit a Super user.";
                    return RedirectToAction(nameof(Index));
                }
            }

            if (await TryUpdateModelAsync<MemberLogin>(memberLoginToUpdate, "",
                e => e.FirstName, e => e.LastName, e => e.Phone, e => e.Email,
                 e => e.Active))
            {
                try
                {
                    if (selectedRole == "Super")
                    {
                        ModelState.AddModelError("Role", "You cannot assign the Super role.");
                        return View(memberLoginToUpdate);
                    }

                    if (User.IsInRole("Admin") && selectedRole != "User")
                    {
                        ModelState.AddModelError("Role", "Admins can only assign the User role.");
                        return View(memberLoginToUpdate);
                    }

                    if (isEditingSelf)
                    {
                        // Prevent Admin from changing their own role or making themselves inactive
                        selectedRole = "Admin";
                        memberLoginToUpdate.Active = true;
                    }

                    await _context.SaveChangesAsync();

                    if (memberLoginToUpdate.Active == false && ActiveStatus == true)
                    {
                        await DeleteIdentityUser(memberLoginToUpdate.Email);
                    }
                    else if (memberLoginToUpdate.Active == true && ActiveStatus == false)
                    {
                        string[] roles = new string[] { selectedRole };
                        InsertIdentityUser(memberLoginToUpdate.Email, roles);
                    }
                    else if (memberLoginToUpdate.Active == true && ActiveStatus == true)
                    {
                        if (memberLoginToUpdate.Email != databaseEmail)
                        {
                            InsertIdentityUser(memberLoginToUpdate.Email, new string[] { selectedRole });
                            await DeleteIdentityUser(databaseEmail);
                        }
                        else
                        {
                            await UpdateUserRoles(new string[] { selectedRole }, memberLoginToUpdate.Email);
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(memberLoginToUpdate.ID))
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
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("Email", "Unable to save changes. Remember, you cannot have duplicate Email addresses.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }

            MemberAdminVm memberAdminVm = new MemberAdminVm
            {
                Email = memberLoginToUpdate.Email,
                Active = memberLoginToUpdate.Active,
                ID = memberLoginToUpdate.ID,
                FirstName = memberLoginToUpdate.FirstName,
                LastName = memberLoginToUpdate.LastName,
                Phone = memberLoginToUpdate.Phone
            };
            memberAdminVm.UserRoles.Add(selectedRole);
            PopulateAssignedRoleData(memberAdminVm);
            return View(memberAdminVm);
        }

        // GET: MemberLogin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberLogin = await _context.MemberLogins
                .FirstOrDefaultAsync(m => m.ID == id);

            if (memberLogin == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Member Logins", Url = "/MemberLogin/Index", IsActive = false },
                new BreadcrumbItem { Title = $"{memberLogin.FirstName} {memberLogin.LastName}", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(memberLogin);
        }

        // GET: MemberLogin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberLogin = await _context.MemberLogins
                .FirstOrDefaultAsync(m => m.ID == id);

            if (memberLogin == null)
            {
                return NotFound();
            }

            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                new BreadcrumbItem { Title = "Member Logins", Url = "/MemberLogin/Index", IsActive = false },
                new BreadcrumbItem { Title = $"{memberLogin.FirstName} {memberLogin.LastName}", Url = $"/MemberLogin/Details/{id}", IsActive = false },
                new BreadcrumbItem { Title = "Delete", Url = "#", IsActive = true }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View(memberLogin);
        }

        // POST: MemberLogin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberLogin = await _context.MemberLogins.FindAsync(id);
            if (memberLogin != null)
            {
                _context.MemberLogins.Remove(memberLogin);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateAssignedRoleData(MemberAdminVm memberAdmin)
        {
            var allRoles = _identityContext.Roles;
            var currentRoles = memberAdmin.UserRoles;
            var viewModel = new List<RoleVM>();
            foreach (var r in allRoles)
            {
                viewModel.Add(new RoleVM
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    Assigned = currentRoles.Contains(r.Name)
                });
            }
            memberAdmin.AvailableRoles = viewModel;
        }


        private async Task UpdateUserRoles(string[] selectedRoles, string Email)
        {
            var _user = await _userManager.FindByEmailAsync(Email);
            if (_user != null)
            {
                var UserRoles = (List<string>)await _userManager.GetRolesAsync(_user);

                if (selectedRoles == null)
                {
                    foreach (var r in UserRoles)
                    {
                        await _userManager.RemoveFromRoleAsync(_user, r);
                    }
                }
                else
                {
                    IList<IdentityRole> allRoles = _identityContext.Roles.ToList<IdentityRole>();

                    foreach (var r in allRoles)
                    {
                        if (selectedRoles.Contains(r.Name))
                        {
                            if (!UserRoles.Contains(r.Name))
                            {
                                await _userManager.AddToRoleAsync(_user, r.Name);
                            }
                        }
                        else
                        {
                            if (UserRoles.Contains(r.Name))
                            {
                                await _userManager.RemoveFromRoleAsync(_user, r.Name);
                            }
                        }
                    }
                }
            }
        }

        private void InsertIdentityUser(string Email, string[] selectedRoles)
        {
            if (_userManager.FindByEmailAsync(Email).Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = Email,
                    Email = Email,
                    EmailConfirmed = true
                };
                string password = MakePassword.Generate();
                password = "Pa55w@rd";
                IdentityResult result = _userManager.CreateAsync(user, password).Result;

                if (result.Succeeded)
                {
                    foreach (string role in selectedRoles)
                    {
                        _userManager.AddToRoleAsync(user, role).Wait();
                    }
                }
            }
            else
            {
                TempData["message"] = "The Login Account for " + Email + " was already in the system.";
            }
        }

        private async Task DeleteIdentityUser(string Email)
        {
            var userToDelete = await _identityContext.Users.Where(u => u.Email == Email).FirstOrDefaultAsync();
            if (userToDelete != null)
            {
                _identityContext.Users.Remove(userToDelete);
                await _identityContext.SaveChangesAsync();
            }
        }

        //private async Task InviteUserToResetPassword(MemberLogin memberLogin, string message)
        //{
        //    message ??= "Hello " + memberLogin.FirstName + "<br /><p>Please navigate to:<br />" +
        //                "<a href='https://theapp.azurewebsites.net/' title='https://theapp.azurewebsites.net/' target='_blank' rel='noopener'>" +
        //                "https://theapp.azurewebsites.net</a><br />" +
        //                " and create a new password for " + memberLogin.Email + " using Forgot Password.</p>";
        //    try
        //    {
        //        await _emailSender.SendOneAsync(employee.Summary, employee.Email,
        //        "Account Registration", message);
        //        TempData["message"] = "Invitation email sent to " + employee.Summary + " at " + employee.Email;
        //    }
        //    catch (Exception)
        //    {
        //        TempData["message"] = "Could not send Invitation email to " + employee.Summary + " at " + employee.Email;
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] ToggleStatusViewModel model)
        {
            if (model == null) return BadRequest();

            var member = await _context.MemberLogins.FindAsync(model.Id);
            if (member == null) return NotFound();

            member.Active = !member.Active;
            await _context.SaveChangesAsync();

            return Json(new { success = true, newStatus = member.Active });
        }

        public class ToggleStatusViewModel
        {
            public int Id { get; set; }
        }

        private bool EmployeeExists(int id)
        {
            return _context.MemberLogins.Any(e => e.ID == id);
        }
    }


}
