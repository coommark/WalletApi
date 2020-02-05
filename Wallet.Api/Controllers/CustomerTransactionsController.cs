using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class CustomerTransactionsController : Controller
    {
        private readonly ICustomerAccountRepository _accountRepository;
        private readonly IAccountTypeRepository _typeRepository;
        private readonly ICustomerAccountStatusRepository _statusRepository;
        private readonly ICustomerTransactionRepository _repository;
        private readonly ICustomerTransactionBatchRepository _batchRepository;
        int page = 1;
        int pageSize = 4;


        public CustomerTransactionsController(ICustomerAccountRepository accountRepository, IAccountTypeRepository typeRepository,
            ICustomerAccountStatusRepository statusRepository, ICustomerTransactionRepository repository, ICustomerTransactionBatchRepository batchRepository)
        {
            _repository = repository;
            _typeRepository = typeRepository;
            _statusRepository = statusRepository;
            _accountRepository = accountRepository;
            _batchRepository = batchRepository;
        }

        [HttpPost("CustomerTransfer")]
        public async Task<IActionResult> CustomerTransfer([FromBody]CustomerTransactionCreateRequest model)
        {
            if (model.SourceAccount == model.DestinationAccount)
            {
                ModelState.AddModelError("", "You cannot transfer from the same account into the same account.");
                return BadRequest(ModelState);
            }
            var sourceAccountFromDb = _accountRepository.GetAccountByAccountNumber(model.SourceAccount);
            var destinationAccountFromDb = _accountRepository.GetAccountByAccountNumber(model.DestinationAccount);            
            var sourceAccountCat = _typeRepository.GetSingle(x => x.Id == sourceAccountFromDb.AccountTypeId);
            var status = _statusRepository.GetSingle(x => x.Id == sourceAccountFromDb.CurrentStatusId);
            if(status.Status != "Active")
            {
                ModelState.AddModelError("", String.Format("Your account status is currently {0} . Transaction declined.", status.Status));
                return BadRequest(ModelState);
            }
            else if(sourceAccountFromDb != null && destinationAccountFromDb != null)
            {
                decimal dailyTransLimit = _accountRepository.DailyTransactionLimit(sourceAccountFromDb.Id);
                decimal dailyTransTotal = _repository.DailyTransactionTotal(sourceAccountFromDb.Id);
                decimal dailyTransBalance = dailyTransLimit - dailyTransTotal;
                decimal accountBalance = _repository.AccountBalance(sourceAccountFromDb.Id);
                if (Math.Abs(dailyTransLimit) < Math.Abs(model.Debit))
                {
                    ModelState.AddModelError("", String.Format("This transaction exceeds your daily limit balance of {0} . Transaction declined.", string.Format("{0:n}", dailyTransLimit)));
                    return BadRequest(ModelState);
                }
                if (Math.Abs(dailyTransBalance) >= Math.Abs(dailyTransLimit))
                {
                    ModelState.AddModelError("", String.Format("This transaction exceeds your daily limit balance of {0} . Transaction declined.", string.Format("{0:n}", dailyTransLimit)));
                    return BadRequest(ModelState);
                }
                if (model.Debit > accountBalance)
                {
                    ModelState.AddModelError("", String.Format("Your account balance {0} is not sufficient for this transaction. Transaction declined.", string.Format("{0:n}", accountBalance)));
                    return BadRequest(ModelState);
                }                
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                string type = "Inter-Customer Transfer";
                if(_accountRepository.IsSameCustomerAccounts(sourceAccountFromDb.Id, destinationAccountFromDb.Id))
                {
                    type = "Inter-Account Transafer";
                }
                var batch = new CustomerTransactionBatch
                {
                    ApplicationUserId = Convert.ToInt32(User.Identity.Name),
                    Type = type
                };
                await _batchRepository.Add(batch);
                await _batchRepository.CommitAsync();

                var sourceTransaction = new CustomerTransaction
                {
                    TransactionType = "Transfer",
                    Credit = model.Debit,
                    Description = "Funds transfer to " + destinationAccountFromDb.ApplicationUser.FullName,
                    CustomerAccountId = sourceAccountFromDb.Id,
                    CustomerTransactionBatchId = batch.Id,
                    AuditDescription = model.Description,
                };

                var destinationTransaction = new CustomerTransaction
                {
                    TransactionType = "Transfer",
                    Debit = model.Debit,
                    Description = "Funds transfer from " + sourceAccountFromDb.ApplicationUser.FullName,
                    CustomerAccountId = destinationAccountFromDb.Id,
                    CustomerTransactionBatchId = batch.Id,
                    AuditDescription = model.Description,
                };
                await _repository.Add(sourceTransaction);
                await _repository.Add(destinationTransaction);
                await _repository.CommitAsync();

                CustomerTransactionViewModel response = Mapper.Map<CustomerTransaction, CustomerTransactionViewModel>(sourceTransaction);
                return CreatedAtRoute("GetTransaction", new { controller = "CustomerTransactions", id = sourceTransaction.Id }, response);
            }            
        }

        [HttpPost("AdminTransfer")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminTransfer([FromBody]CustomerTransactionCreateRequest model)
        {
            var adminCurrentAccount = _accountRepository.AdminCurrenAccount(Convert.ToInt32(User.Identity.Name));
            if(adminCurrentAccount == null)
            {
                ModelState.AddModelError("", String.Format("You must create a currrent account for this Administrator before any transaction. Transaction declined"));
                return BadRequest(ModelState);
            }
            var destinationAccountFromDb = _accountRepository.GetSingle(x => x.ApplicationUserId == model.CustomerId, x => x.ApplicationUser);

            if ((adminCurrentAccount.ApplicationUserId == destinationAccountFromDb.ApplicationUserId)
                && adminCurrentAccount.AccountTypeId == destinationAccountFromDb.AccountTypeId)
            {
                ModelState.AddModelError("", String.Format("You cannot transfer from the same account into the same account"));
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {                
                var batch = new CustomerTransactionBatch
                {
                    ApplicationUserId = Convert.ToInt32(User.Identity.Name),
                    Type = "Admin Transfer"
                };
                await _batchRepository.Add(batch);
                await _batchRepository.CommitAsync();

                var sourceTransaction = new CustomerTransaction
                {
                    TransactionType = "Transfer",
                    Credit = model.Debit,
                    Description = "Funds transfer to " + destinationAccountFromDb.ApplicationUser.FullName,
                    AuditDescription = model.Description,
                    CustomerAccountId = adminCurrentAccount.Id,
                    CustomerTransactionBatchId = batch.Id,
                };

                var destinationTransaction = new CustomerTransaction
                {
                    TransactionType = "Transfer",
                    Debit = model.Debit,
                    Description = "Funds transfer from " + adminCurrentAccount.ApplicationUser.FullName,
                    AuditDescription = model.Description,
                    CustomerAccountId = destinationAccountFromDb.Id,
                    CustomerTransactionBatchId = batch.Id,
                };
                await _repository.Add(sourceTransaction);
                await _repository.Add(destinationTransaction);
                await _repository.CommitAsync();

                CustomerTransactionViewModel response = Mapper.Map<CustomerTransaction, CustomerTransactionViewModel>(sourceTransaction);
                return CreatedAtRoute("GetTransaction", new { controller = "CustomerTransactions", id = sourceTransaction.Id }, response);
            }
        }

        [HttpGet("{id}", Name = "GetTransaction")]
        public IActionResult Get(int? id)
        {
            if (id == null)
                return NotFound();

            CustomerTransaction result = _repository.GetSingle(x => x.Id == id);

            if (result != null)
            {
                CustomerTransactionViewModel vm = Mapper.Map<CustomerTransaction, CustomerTransactionViewModel>(result);

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

            IEnumerable<CustomerTransaction> result = _repository
                .AllIncluding(x => x.CustomerAccount)
                .OrderBy(c => c.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();
            
            Response.AddPagination(page, pageSize, totalCount, totalPages);
            IEnumerable<CustomerTransactionViewModel> vm = Mapper.Map<IEnumerable<CustomerTransaction>, IEnumerable<CustomerTransactionViewModel>>(result);
            foreach (var item in vm)
            {
                item.AccountName = _repository.GetCustomerName(item.CustomerAccount.Id);
            }
            return new OkObjectResult(vm);
        }

        [HttpGet("UserTransactions/{accountNumber}", Name = "UserTransactions")]
        public async Task<IActionResult> UserTransactions(string accountNumber)
        {            
            var customerAccount = _accountRepository.GetSingle(x => x.Id == Convert.ToInt32(User.Identity.Name));
            if(customerAccount.ApplicationUserId != Convert.ToInt32(User.Identity.Name))
            {
                return new UnauthorizedResult();
            }
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
            var totalCount = await _repository.CountForAccount(customerAccount.Id);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            IEnumerable<CustomerTransaction> result = _repository
                .AllIncluding(x => x.CustomerAccount)
                .Where(x => x.CustomerAccount.AccountNumber == accountNumber)
                .OrderBy(c => c.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            Response.AddPagination(page, pageSize, totalCount, totalPages);
            IEnumerable<CustomerTransactionViewModel> vm = Mapper.Map<IEnumerable<CustomerTransaction>, IEnumerable<CustomerTransactionViewModel>>(result);
            foreach (var item in vm)
            {
                item.AccountName = _repository.GetCustomerName(item.CustomerAccount.Id);
            }
            return new OkObjectResult(vm);
        }
    }
}