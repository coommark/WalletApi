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
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var userAccount = await _userManager.FindByEmailAsync(model.Email);
                // Delete when the user must activate their account via email.
                userAccount.EmailConfirmed = true;

                // Create user role                
                var findUserRole = await _roleManager.FindByNameAsync("Customer");
                var userRole = new IdentityRole("Customer");                

                // Add userAccount to a user role
                if (!await _userManager.IsInRoleAsync(userAccount, userRole.Name))
                {
                    await _userManager.AddToRoleAsync(userAccount, userRole.Name);
                }

                return new OkResult();
            }

            // If result is not successful, add error message(s)
            AddErrors(result);

            return new BadRequestObjectResult(result.Errors);
        }

        //[HttpPost]
        //[Route("login")]
        //public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        //    if (!result.Succeeded)
        //    {
        //        ModelState.AddModelError(string.Empty, "Wrong username or password");
        //        return BadRequest(ModelState);
        //    }

        //    var user = await _userManager.FindByNameAsync(model.Email);

        //    return new OkResult();
        //}



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