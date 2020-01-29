using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.Extensions;
using Wallet.Core.DomainEntities;
using Wallet.Core.Dto.Requests;
using Wallet.Core.Dto.Responses;
using Wallet.Core.Dto.ViewModels;
using Wallet.Data.Abstract;
using Wallet.Data.Helpers;

namespace Wallet.Api.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    public class CustomerAccountsController : Controller
    {
        private readonly ICustomerAccountRepository _repository;
        private readonly IAccountTypeRepository _typeRepository;
        int page = 1;
        int pageSize = 4;
        
       
        public CustomerAccountsController(ICustomerAccountRepository repository, IAccountTypeRepository typeRepository)
        {
            _repository = repository;
            _typeRepository = typeRepository;
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
            var totalCourses = await _repository.Count();
            var totalPages = (int)Math.Ceiling((double)totalCourses / pageSize);

            IEnumerable<CustomerAccount> result = _repository
                .AllIncluding(x => x.ApplicationUser, m => m.AccountType)
                .OrderBy(c => c.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            Response.AddPagination(page, pageSize, totalCourses, totalPages);
            IEnumerable<CustomerAccountViewModel> vm = Mapper.Map<IEnumerable<CustomerAccount>, IEnumerable<CustomerAccountViewModel>>(result);
            return new OkObjectResult(vm);
        }


        [HttpGet("{id}", Name = "GetCustomerAccount")]
        public IActionResult Get(int? id)
        {
            if (id == null)
                return NotFound();

            CustomerAccount result = _repository.GetSingle(x => x.Id == id);

            if (result != null)
            {
                CustomerAccountViewModel vm = Mapper.Map<CustomerAccount, CustomerAccountViewModel>(result);

                return new OkObjectResult(vm);
            }
            else
                return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody]CustomerAccountCreateRequest model)
        {
            if (_repository.CheckCategoryAccountExist(model.AccountTypeId, model.ApplicationUserId))
            {
                ModelState.AddModelError("ApplicationUserId", "The customer already has an account in this account category.");
            };
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CustomerAccount tocreate = new CustomerAccount
            {
                ApplicationUserId = model.ApplicationUserId,
                AccountTypeId = model.AccountTypeId,
                CreatedBy = Convert.ToInt32(User.Identity.Name)
            };
            var accountType = _typeRepository.GetSingle(x => x.Id == model.AccountTypeId);
            tocreate.AccountNumber = NumberSeriesHelper.CreateAccountNumber(model.ApplicationUserId, accountType.CategoryCode);
            await _repository.Add(tocreate);
            await _repository.CommitAsync();

            CustomerAccountCreateResponse response = Mapper.Map<CustomerAccount, CustomerAccountCreateResponse>(tocreate);

            return CreatedAtRoute("GetCustomerAccount", new { controller = "CustomerAccounts", id = tocreate.Id }, response);
        }
    }
}