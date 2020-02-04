using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var sourceAccountFromDb = _accountRepository.GetAccountByAccountNumber(model.SourceAccount);
            var destinationAccountFromDb = _accountRepository.GetSingle(x => x.Id == model.CustomerId, x => x.ApplicationUser);
            var sourceAccountCat = _typeRepository.GetSingle(x => x.Id == sourceAccountFromDb.AccountTypeId);
            var status = _statusRepository.GetSingle(x => x.Id == sourceAccountFromDb.CurrentStatusId);
            if(status.Status != "Active")
            {
                ModelState.AddModelError("", String.Format("Your account status is currently {0} . Transaction declined.", status.Status));
            }
            else if(sourceAccountFromDb != null && destinationAccountFromDb != null)
            {
                decimal dailyTransLimit = _accountRepository.DailyTransactionLimit(sourceAccountFromDb.Id);
                decimal dailyTransTotal = _repository.DailyTransactionTotal(sourceAccountFromDb.Id);
                decimal dailyTransBalance = dailyTransLimit - dailyTransTotal;
                decimal accountBalance = _repository.AccountBalance(sourceAccountFromDb.Id);
                if(dailyTransBalance > dailyTransLimit)
                {
                    ModelState.AddModelError("", String.Format("This transaction exceeds your daily limit balance of {0} . Transaction declined.", string.Format("{0:n}", dailyTransBalance)));
                }
                if (model.Debit > accountBalance)
                {
                    ModelState.AddModelError("", String.Format("Your account balance {0} is not sufficient for this transaction. Transaction declined.", string.Format("{0:n}", accountBalance)));
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
                };

                var destinationTransaction = new CustomerTransaction
                {
                    TransactionType = "Transfer",
                    Debit = model.Debit,
                    Description = "Funds transfer from " + sourceAccountFromDb.ApplicationUser.FullName,
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

        [HttpPost("AdminTransfer")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminTransfer([FromBody]CustomerTransactionCreateRequest model)
        {
            var adminCurrentAccount = _accountRepository.AdminCurrenAccount(Convert.ToInt32(User.Identity.Name));
            var status = _statusRepository.GetSingle(x => x.Id == adminCurrentAccount.CurrentStatusId);
            var destinationAccountFromDb = _accountRepository.GetSingle(x => x.Id == model.CustomerId, x => x.ApplicationUser);
            if (status.Status != "Active")
            {
                ModelState.AddModelError("", String.Format("Your account status is currently {0} . Transaction declined.", status.Status));
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
                    CustomerAccountId = Convert.ToInt32(User.Identity.Name),
                    CustomerTransactionBatchId = batch.Id,
                };

                var destinationTransaction = new CustomerTransaction
                {
                    TransactionType = "Transfer",
                    Debit = model.Debit,
                    Description = "Funds transfer from " + adminCurrentAccount.ApplicationUser.FullName,
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
        public IActionResult Get()
        {
            return View();
        }

        //[HttpPost("CustomerTransfer")]
        //public async Task<IActionResult> CustomerTransfer([FromBody]List<CustomerTransactionCreateRequest> model)
        //{
        //    var sourceAccount = model.SingleOrDefault(x => x.Flow == "Souce");
        //    var destinationAccount = model.SingleOrDefault(x => x.Flow == "Destination");
        //    var sourceAccountFromDb = _accountRepository.GetSingle(x => x.Id == sourceAccount.CustomerId, x => x.ApplicationUser);
        //    var destinationAccountFromDb = _accountRepository.GetSingle(x => x.Id == destinationAccount.CustomerId, x => x.ApplicationUser);
        //    var sourceAccountCat = _typeRepository.GetSingle(x => x.Id == sourceAccountFromDb.AccountTypeId);
        //    var status = _statusRepository.GetSingle(x => x.Id == sourceAccountFromDb.CurrentStatusId);
        //    if (status.Status != "Active")
        //    {
        //        ModelState.AddModelError("", String.Format("Your account status is currently {0} . Transaction declined.", status.Status));
        //    }
        //    else if (sourceAccount != null && destinationAccount != null)
        //    {
        //        decimal dailyTransLimit = _accountRepository.DailyTransactionLimit(sourceAccount.CustomerId);
        //        decimal dailyTransTotal = _repository.DailyTransactionTotal(sourceAccount.CustomerId);
        //        decimal dailyTransBalance = dailyTransLimit - dailyTransLimit;
        //        decimal accountBalance = _repository.AccountBalance(sourceAccount.CustomerId);
        //        if (sourceAccountCat.AllowOverdraw && dailyTransBalance > dailyTransLimit)
        //        {
        //            ModelState.AddModelError("", String.Format("This transaction exceeds your daily limit balance of {0} . Transaction declined.", dailyTransBalance));
        //        }
        //        else if (!sourceAccountCat.AllowOverdraw && dailyTransBalance > dailyTransLimit && sourceAccount.Debit <= accountBalance)
        //        {
        //            ModelState.AddModelError("", String.Format("Your account balance {0} is not sufficient for this transaction. Transaction declined.", accountBalance));
        //        }
        //    }
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);
        //    else
        //    {
        //        string type = "Inter-Customer Transfer";
        //        if (_accountRepository.IsSameCustomerAccounts(sourceAccount.CustomerId, destinationAccount.CustomerId))
        //        {
        //            type = "Inter-Account Transafer";
        //        }
        //        var batch = new CustomerTransactionBatch
        //        {
        //            ApplicationUserId = Convert.ToInt32(User.Identity.Name),
        //            Type = type
        //        };
        //        await _batchRepository.Add(batch);
        //        await _batchRepository.CommitAsync();

        //        var sourceTransaction = new CustomerTransaction
        //        {
        //            TransactionType = "Transfer",
        //            Credit = sourceAccount.Credit,
        //            Description = "Funds transfer to " + destinationAccountFromDb.ApplicationUser.FullName,
        //            CustomerAccountId = sourceAccountFromDb.Id,
        //            CustomerTransactionBatchId = batch.Id,
        //        };

        //        var destinationTransaction = new CustomerTransaction
        //        {
        //            TransactionType = "Transfer",
        //            Debit = destinationAccount.Debit,
        //            Description = "Funds transfer from " + sourceAccountFromDb.ApplicationUser.FullName,
        //            CustomerAccountId = destinationAccountFromDb.Id,
        //            CustomerTransactionBatchId = batch.Id,
        //        };
        //        await _repository.Add(sourceTransaction);
        //        await _repository.Add(destinationTransaction);
        //        await _repository.CommitAsync();

        //        CustomerTransactionViewModel response = Mapper.Map<CustomerTransaction, CustomerTransactionViewModel>(sourceTransaction);
        //        return CreatedAtRoute("GetTransaction", new { controller = "CustomerTransactions", id = sourceTransaction.Id }, response);
        //    }
        //}
    }
}