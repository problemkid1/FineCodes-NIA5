﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CRMProject.Data
{
    public static class ApplicationDbInitializer
    {
        public static async void Initialize(IServiceProvider serviceProvider,
            bool UseMigrations = true, bool SeedSampleData = true)
        {
            #region Prepare the Database
            if (UseMigrations)
            {
                using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
                {
                    try
                    {
                        //Create the database if it does not exist and apply the Migration
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.GetBaseException().Message);
                    }
                }
            }
            #endregion

            #region Seed Sample Data 
            if (SeedSampleData)
            {
                //Create Roles
                using (var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>())
                {
                    try
                    {
                        string[] roleNames = { "Super","Admin", "User" };

                        IdentityResult roleResult;
                        foreach (var roleName in roleNames)
                        {
                            var roleExist = await roleManager.RoleExistsAsync(roleName);
                            if (!roleExist)
                            {
                                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.GetBaseException().Message);
                    }
                }

                //Create Users
                using (var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>())
                {
                    try
                    {
                        string defaultPassword = "Pa55w@rd";

                        if (userManager.FindByEmailAsync("admin@outlook.com").Result == null)
                        {
                            IdentityUser user = new IdentityUser
                            {
                                UserName = "admin@outlook.com",
                                Email = "admin@outlook.com",
                                EmailConfirmed = true
                            };

                            IdentityResult result = userManager.CreateAsync(user, defaultPassword).Result;

                            if (result.Succeeded)
                            {
                                userManager.AddToRoleAsync(user, "Admin").Wait();
                            }
                        }
                        if (userManager.FindByEmailAsync("super@outlook.com").Result == null)
                        {
                            IdentityUser user = new IdentityUser
                            {
                                UserName = "super@outlook.com",
                                Email = "super@outlook.com",
                                EmailConfirmed = true
                            };

                            IdentityResult result = userManager.CreateAsync(user, defaultPassword).Result;

                            if (result.Succeeded)
                            {
                                userManager.AddToRoleAsync(user, "Super").Wait();
                            }
                        }
                        if (userManager.FindByEmailAsync("user@outlook.com").Result == null)
                        {
                            IdentityUser user = new IdentityUser
                            {
                                UserName = "user@outlook.com",
                                Email = "user@outlook.com",
                                EmailConfirmed = true
                            };

                            IdentityResult result = userManager.CreateAsync(user, defaultPassword).Result;
                            if (result.Succeeded)
                            {
                                userManager.AddToRoleAsync(user, "User").Wait();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.GetBaseException().Message);
                    }
                }
            }
            #endregion
        }
    }
}
