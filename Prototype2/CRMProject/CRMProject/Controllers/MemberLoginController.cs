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
            PopulateAssignedRoleData(memberAdmin);

            // Remove the Admin role from the list of roles
            ViewBag.Roles = ((List<RoleVM>)ViewBag.Roles).Where(r => r.RoleName != "Admin").ToList();

            return View(memberAdmin);
        }

        //POST: MemberLogin/Create
        //To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Phone," +
            "Email")] MemberLogin memberlogin/*, string[] selectedRoles*/)
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
                    _context.Add(memberlogin);
                    await _context.SaveChangesAsync();

                    // Default role to "User"
                    string[] selectedRoles = new string[] { "User" };
                    InsertIdentityUser(memberlogin.Email, selectedRoles);

                    //Send Email to new Employee - commented out till email configured
                    //await InviteUserToResetPassword(employee, null);

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
            //We are here because something went wrong and need to redisplay
            MemberAdminVm memberAdminVm = new MemberAdminVm
            {
                Email = memberlogin.Email,
                Active = memberlogin.Active,
                ID = memberlogin.ID,
                FirstName = memberlogin.FirstName,
                LastName = memberlogin.LastName,
                Phone = memberlogin.Phone
            };
            //foreach (var role in selectedRoles)
            //{
            //    memberAdminVm.UserRoles.Add(role);
            //}
            PopulateAssignedRoleData(memberAdminVm);

            // Remove the Admin role from the list of roles
            ViewBag.Roles = ((List<RoleVM>)ViewBag.Roles).Where(r => r.RoleName != "Admin").ToList();

            return View(memberAdminVm);
        }

        // GET: Employees/Edit/5
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
            if (user != null)
            {
                //Add the current roles
                var r = await _userManager.GetRolesAsync(user);
                memberLogin.UserRoles = (List<string>)r;

                //// Check if user is the only Admin
                //if (memberLogin.UserRoles.Contains("Admin"))
                //{
                //    var allAdmins = await _userManager.GetUsersInRoleAsync("Admin");
                //    if (allAdmins.Count == 1 && allAdmins[0].Email == memberLogin.Email)
                //    {
                //        ViewData["IsSoleAdmin"] = true;
                //    }
                //}
            }
            PopulateAssignedRoleData(memberLogin);

            return View(memberLogin);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool Active, string[] selectedRoles)
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

            //Note the current Email and Active Status
            bool ActiveStatus = memberLoginToUpdate.Active;
            string databaseEmail = memberLoginToUpdate.Email;


            if (await TryUpdateModelAsync<MemberLogin>(memberLoginToUpdate, "",
                e => e.FirstName, e => e.LastName, e => e.Phone, e => e.Email,
                 e => e.Active))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    //Save successful so go on to related changes

                    //Check for changes in the Active state
                    if (memberLoginToUpdate.Active == false && ActiveStatus == true)
                    {
                        //Deactivating them so delete the IdentityUser
                        //This deletes the user's login from the security system
                        await DeleteIdentityUser(memberLoginToUpdate.Email);

                    }
                    else if (memberLoginToUpdate.Active == true && ActiveStatus == false)
                    {
                        //You reactivating the user, create them and
                        //give them the selected roles
                        //InsertIdentityUser(memberLoginToUpdate.Email, selectedRoles);

                        // Reactivating the user, create them and
                        // give them the "User" role
                        string[] roles = new string[] { "User" };
                        InsertIdentityUser(memberLoginToUpdate.Email, roles);
                    }
                    else if (memberLoginToUpdate.Active == true && ActiveStatus == true)
                    {
                        //No change to Active status so check for a change in Email
                        //If you Changed the email, Delete the old login and create a new one
                        //with the selected roles
                        if (memberLoginToUpdate.Email != databaseEmail)
                        {
                            //Add the new login with the selected roles
                            InsertIdentityUser(memberLoginToUpdate.Email, selectedRoles);

                            //This deletes the user's old login from the security system
                            await DeleteIdentityUser(databaseEmail);
                        }
                        else
                        {
                            //Finially, Still Active and no change to Email so just Update
                            await UpdateUserRoles(selectedRoles, memberLoginToUpdate.Email);
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
            //We are here because something went wrong and need to redisplay
            MemberAdminVm memberAdminVm = new MemberAdminVm
            {
                Email = memberLoginToUpdate.Email,
                Active = memberLoginToUpdate.Active,
                ID = memberLoginToUpdate.ID,
                FirstName = memberLoginToUpdate.FirstName,
                LastName = memberLoginToUpdate.LastName,
                Phone = memberLoginToUpdate.Phone
            };
            foreach (var role in selectedRoles)
            {
                memberAdminVm.UserRoles.Add(role);
            }
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
        {//Prepare checkboxes for all Roles
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
            ViewBag.Roles = viewModel;
        }


        private async Task UpdateUserRoles(string[] selectedRoles, string Email)
        {
            var _user = await _userManager.FindByEmailAsync(Email);//IdentityUser
            if (_user != null)
            {
                var UserRoles = (List<string>)await _userManager.GetRolesAsync(_user);//Current roles user is in

                if (selectedRoles == null)
                {
                    //No roles selected so just remove any currently assigned
                    foreach (var r in UserRoles)
                    {
                        await _userManager.RemoveFromRoleAsync(_user, r);
                    }
                }
                else
                {
                    //At least one role checked so loop through all the roles
                    //and add or remove as required

                    //We need to do this next line because foreach loops don't always work well
                    //for data returned by EF when working async.  Pulling it into an IList<>
                    //first means we can safely loop over the colleciton making async calls and avoid
                    //the error 'New transaction is not allowed because there are other threads running in the session'
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
            //Create the IdentityUser in the IdentitySystem
            //Note: this is similar to what we did in ApplicationSeedData
            if (_userManager.FindByEmailAsync(Email).Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = Email,
                    Email = Email,
                    EmailConfirmed = true //since we are creating it!
                };
                //Create a random password with a default 8 characters
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
