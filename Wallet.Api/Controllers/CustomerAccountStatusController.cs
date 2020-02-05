using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wallet.Core.DomainEntities;
using Wallet.Core.Dto.Requests;
using Wallet.Core.Dto.ViewModels;
using Wallet.Data.Abstract;

namespace Wallet.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CustomerAccountStatusController : Controller
    {
        private readonly ICustomerAccountStatusRepository _repository;
        private readonly ICustomerAccountRepository _accounRepository;

        public CustomerAccountStatusController(ICustomerAccountStatusRepository repository,
             ICustomerAccountRepository accounRepository)
        {
            _repository = repository;
            _accounRepository = accounRepository;
        }

        [HttpGet("{id}", Name = "GetAccountStatus")]
        [Authorize(Roles = "Administrator")]
        public  IActionResult Get(int? id)
        {
            if (id == null)
                return NotFound();

            CustomerAccountStatus result =  _repository.GetSingle(x => x.Id == id);

            if (result != null)
            {
                CustomerAccountStatusCreateRequest vm = Mapper.Map<CustomerAccountStatus, CustomerAccountStatusCreateRequest>(result);

                return new OkObjectResult(vm);
            }
            else
                return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody]CustomerAccountStatusCreateRequest model)
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _repository.UpdadeCurrent(model.CustomerAccountId, Convert.ToInt32(User.Identity.Name));
            CustomerAccountStatus toadd = new CustomerAccountStatus
            {
                CustomerAccountId = model.CustomerAccountId,
                Status = model.Status,
                IsCurrentStatus = true,
                Comment = model.Comment
            };

            await _repository.Add(toadd);
            await _repository.CommitAsync();

            var customerAccount = _accounRepository.GetSingle(x => x.Id == model.CustomerAccountId);
            customerAccount.CurrentStatusId = toadd.Id;
             _accounRepository.Update(customerAccount);
            await _accounRepository.CommitAsync();

            CustomerAccountStatusViewModel response = Mapper.Map<CustomerAccountStatus, CustomerAccountStatusViewModel>(toadd);

            return CreatedAtRoute("GetAccountStatus", new { controller = "CustomerAccountStatus", id = toadd.Id }, response);
        }
    }
}