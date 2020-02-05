using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.Membership;

namespace Wallet.Data
{
    public class Seed
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;

        public Seed(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedData()
        {
            var existAdminRole = await _roleManager.FindByNameAsync("Administrator");
            var existCustRole = await _roleManager.FindByNameAsync("Customer");
            var adminRole = new ApplicationRole("Administrator");
            var custRole = new ApplicationRole("Customer");

            if(existAdminRole == null)
            {
                await _roleManager.CreateAsync(adminRole);
            }

            if (existCustRole == null)
            {
                await _roleManager.CreateAsync(custRole);
            }

            var existAdminAccount = await _userManager.FindByNameAsync("admin@admin.com");
            if (existAdminAccount == null)
            {
                var admin = new ApplicationUser()
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    FirstName = "Mark",
                    LastName = "Melton",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    RegisterDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(admin, "Password123$");
                var account = await _userManager.FindByEmailAsync(admin.Email);
                account.EmailConfirmed = true;

                try
                {
                    if (result.Succeeded)
                    {
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            var adminAccount = await _userManager.FindByNameAsync("admin@admin.com");
            if (!await _userManager.IsInRoleAsync(adminAccount, adminRole.Name))
            {
                await _userManager.AddToRoleAsync(adminAccount, adminRole.Name);
            }

            await _context.SaveChangesAsync();
        }
    }
}
