using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.Extensions;
using Wallet.Core.DomainEntities;
using Wallet.Core.Dto.Requests;
using Wallet.Core.Dto.ViewModels;
using Wallet.Data.Abstract;

namespace Wallet.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AccountTypesController : Controller
    {
        private readonly IAccountTypeRepository _repository;
        int page = 1;
        int pageSize = 4;

        public AccountTypesController(IAccountTypeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var pagination = Request.Headers["Pagination"];

            if (!string.IsNullOrEmpty(pagination))
            {
                string[] vals = pagination.ToString().Split(',');
                int.TryParse(vals[0], out page);
                int.TryParse(vals[1], out pageSize);
            }

            int currentPage = page;
            int currentPageSize = pageSize;
            var total = await _repository.Count();
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            IEnumerable<AccountType> result = _repository
                .AllIncluding(c => c.CustomerAccounts)
                .OrderBy(c => c.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            Response.AddPagination(page, pageSize, total, totalPages);
            IEnumerable<AccountTypeViewModel> vm = Mapper.Map<IEnumerable<AccountType>, IEnumerable<AccountTypeViewModel>>(result);
            return new OkObjectResult(vm);
        }

        [HttpGet("{id}", Name = "GetAccountType")]
        public IActionResult Get(int? id)
        {
            if (id == null)
                return NotFound();

            AccountType result = _repository.GetSingle(x => x.Id == id);

            if (result != null)
            {
                AccountTypeViewModel vm = Mapper.Map<AccountType, AccountTypeViewModel>(result);

                return new OkObjectResult(vm);
            }
            else
                return NotFound();
        }

        [HttpGet("search/{term}", Name = "SearchAccountType")]
        public IActionResult Search(string term)
        {
            var result = _repository.FindBy(x => x.Type.StartsWith(term));

            if (result != null)
            {
                IEnumerable<AccountTypeViewModel> vm = Mapper.Map<IEnumerable<AccountType>, IEnumerable<AccountTypeViewModel>>(result);
                return new OkObjectResult(vm);
            }
            else
                return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody]AccountTypeCreateRequest model)
        {
            if(_repository.CheckTypeExist(model.Type)) {                
                    ModelState.AddModelError("Type", "This account type already exists. Create a unique account type.");
            };
            if (_repository.CheckCategoryCodeExist(model.CategoryCode))
            {
                ModelState.AddModelError("CategoryCode", "This category code already exists. Create a unique code.");
            };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            AccountType accountType = new AccountType
            {
                Type = model.Type,
                Description = model.Description,
                CategoryCode = model.CategoryCode,
                CreatedBy = Convert.ToInt32(User.Identity.Name)
            };

            await _repository.Add(accountType);
            await _repository.CommitAsync();

            AccountTypeViewModel response = Mapper.Map<AccountType, AccountTypeViewModel>(accountType);

            return CreatedAtRoute("GetAccountType", new { controller = "AccountTypes", id = accountType.Id }, response);
        }
    }
}