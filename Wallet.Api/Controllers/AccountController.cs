using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Wallet.Core;
using Wallet.Core.Dto.Requests;
using Wallet.Core.Dto.ViewModels;
using Wallet.Core.Membership;

namespace Wallet.Api.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private RoleManager<ApplicationRole> _roleManager;
        private JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<ApplicationRole> roleManager,
                                 IOptions<JwtIssuerOptions> jwtOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtOptions = jwtOptions.Value;

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]UserRegistrationRequest model)
        {
            if (!ModelState.IsValid)
            {
                string errorMsg = null;

                foreach (var test in ModelState.Values)
                {
                    foreach (var msg in test.Errors)
                    {
                        errorMsg = msg.ErrorMessage;
                    }
                }
                return BadRequest(errorMsg);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                RegisterDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var userAccount = await _userManager.FindByEmailAsync(model.Email);
                userAccount.EmailConfirmed = true;

                var findUserRole = await _roleManager.FindByNameAsync("Customer");
                var userRole = new IdentityRole("Customer");                

                if (!await _userManager.IsInRoleAsync(userAccount, userRole.Name))
                {
                    await _userManager.AddToRoleAsync(userAccount, userRole.Name);
                }
                return new OkResult();
            }

            AddErrors(result);

            return new BadRequestObjectResult(result.Errors);
        }

        



        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        #endregion
    }
}