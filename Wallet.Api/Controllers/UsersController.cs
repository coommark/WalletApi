using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.Extensions;
using Wallet.Core.DomainEntities;
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
        int page = 1;
        int pageSize = 4;


        public UsersController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("search/{term}", Name = "SearchCustomers")]
        public IActionResult Get(string term)
        {

            var result = _repository.FindBy(x => x.FirstName.StartsWith(term) || x.LastName.StartsWith(term));

            if (result != null)
            {
                IEnumerable<ApplicationUserViewModel> vm = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<ApplicationUserViewModel>>(result);
                return new OkObjectResult(vm);
            }
            else
                return NotFound();
        }

        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult Get(int? id)
        {
            if (id == null)
                return NotFound();

            ApplicationUser result = _repository.GetSingle(x => x.Id == id);

            if (result != null)
            {
                ApplicationUserViewModel vm = Mapper.Map<ApplicationUser, ApplicationUserViewModel>(result);

                return new OkObjectResult(vm);
            }
            else
                return NotFound();
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Get()
        {
            var pagination = Request.Headers["Pagination"];
            string sortExpression = string.Empty;

            if (!string.IsNullOrEmpty(pagination))
            {
                string[] vals = pagination.ToString().Split(',');
                int.TryParse(vals[0], out page);
                int.TryParse(vals[1], out pageSize);
            }

            int currentPage = page;
            int currentPageSize = pageSize;
            var totalCount = await _repository.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            IEnumerable<ApplicationUser> result = _repository
                .AllIncluding()
                .OrderBy(c => c.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            Response.AddPagination(page, pageSize, totalCount, totalPages);
            IEnumerable<ApplicationUserViewModel> vm = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<ApplicationUserViewModel>>(result);
            return new OkObjectResult(vm);
        }
    }
}