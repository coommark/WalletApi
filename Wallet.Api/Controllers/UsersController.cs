using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Core.Dto.ViewModels;
using Wallet.Core.Membership;
using Wallet.Data.Abstract;

namespace Wallet.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _repository;
       


        public UsersController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{name}", Name = "SearchCustomers")]
        public IActionResult Get(string name)
        {

            var result = _repository.FindBy(x => x.FirstName.StartsWith(name));

            if (result != null)
            {
                IEnumerable<ApplicationUserViewModel> vm = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<ApplicationUserViewModel>>(result);
                return new OkObjectResult(vm);
            }
            else
                return NotFound();
        }
    }
}