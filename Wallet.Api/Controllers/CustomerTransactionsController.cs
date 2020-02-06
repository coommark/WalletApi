using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.Extensions;
using Wallet.Api.Mailer;
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
        private readonly IEMailer _mailer;
        int page = 1;
        int pageSize = 4;


        public CustomerTransactionsController(ICustomerAccountRepository accountRepository, IAccountTypeRepository typeRepository,
            ICustomerAccountStatusRepository statusRepository, ICustomerTransactionRepository repository, ICustomerTransactionBatchRepository batchRepository,
            IEMailer mailer)
        {
            _repository = repository;
            _typeRepository = typeRepository;
            _statusRepository = statusRepository;
            _accountRepository = accountRepository;
            _batchRepository = batchRepository;
            _mailer = mailer;
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
            var destinationStatus = _statusRepository.GetSingle(x => x.Id == destinationAccountFromDb.CurrentStatusId);
            decimal sourceBalance = _repository.AccountBalance(sourceAccountFromDb.Id);
            decimal destinationBalance = _repository.AccountBalance(destinationAccountFromDb.Id);
            if (destinationStatus.Status == "Closed")
            {
                ModelState.AddModelError("", String.Format("The destination account {0} is {1}. Transaction declined.", destinationAccountFromDb.AccountNumber, destinationStatus.Status));
                return BadRequest(ModelState);
            }
            if (status.Status != "Active")
            {
                ModelState.AddModelError("", String.Format("Your account status is currently {0} . Transaction declined.", status.Status));
                return BadRequest(ModelState);
            }
            else if(sourceAccountFromDb != null && destinationAccountFromDb != null)
            {
                decimal dailyTransLimit = _accountRepository.DailyTransactionLimit(sourceAccountFromDb.Id);
                decimal dailyTransTotal = _repository.DailyTransactionTotal(sourceAccountFromDb.Id);
                decimal dailyTransBalance = dailyTransLimit - dailyTransTotal;
                
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
                if (model.Debit > sourceBalance)
                {
                    ModelState.AddModelError("", String.Format("Your account balance {0} is not sufficient for this transaction. Transaction declined.", string.Format("{0:n}", sourceBalance)));
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

                //Send Email to Receipient
                _mailer.SendTransactionNotification(
                    destinationAccountFromDb.ApplicationUser.Email,
                    destinationAccountFromDb.ApplicationUser.FullName,
                    string.Format("{0:n}", model.Debit),
                    string.Format("{0:n}", model.Debit),
                    "Credit Alert on your account " + destinationAccountFromDb.AccountType.Type + " " + destinationAccountFromDb.AccountNumber + " from " + sourceAccountFromDb.ApplicationUser.FullName);

                //Send to Source
                _mailer.SendTransactionNotification(
                   sourceAccountFromDb.ApplicationUser.Email,
                   sourceAccountFromDb.ApplicationUser.FullName,
                   string.Format("{0:n}", model.Debit),
                   string.Format("{0:n}", (sourceBalance - model.Debit)),
                   "Debit Alert on your account " + sourceAccountFromDb.AccountType.Type + " " + sourceAccountFromDb.AccountNumber + " to " + destinationAccountFromDb.ApplicationUser.FullName);

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
                decimal sourceBalance = _repository.AccountBalance(adminCurrentAccount.Id);
                decimal destinationBalance = _repository.AccountBalance(destinationAccountFromDb.Id);
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
                    Description = "Funds transfer from E-Wallet Admin",
                    AuditDescription = model.Description,
                    CustomerAccountId = destinationAccountFromDb.Id,
                    CustomerTransactionBatchId = batch.Id,
                };
                await _repository.Add(sourceTransaction);
                await _repository.Add(destinationTransaction);
                await _repository.CommitAsync();

                //Send Email to Receipient
                _mailer.SendTransactionNotification(
                    destinationAccountFromDb.ApplicationUser.Email,
                    destinationAccountFromDb.ApplicationUser.FullName,
                    string.Format("{0:n}", model.Debit),
                    string.Format("{0:n}", destinationBalance + model.Debit),
                    "Credit Alert on your account " + destinationAccountFromDb.AccountType.Type + " " + destinationAccountFromDb.AccountNumber + " from E-Wallet Admin.");

                //Send to Source
                _mailer.SendTransactionNotification(
                   adminCurrentAccount.ApplicationUser.Email,
                   adminCurrentAccount.ApplicationUser.FullName,
                   string.Format("{0:n}", model.Debit),
                   string.Format("{0:n}", (sourceBalance - model.Debit)),
                   "Debit Alert on your account " + adminCurrentAccount.AccountType.Type + " " + adminCurrentAccount.AccountNumber + " to " + destinationAccountFromDb.ApplicationUser.FullName);


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

        [HttpPost("AccountStatement", Name = "AccountStatement")]
        public IActionResult AccountStatement([FromBody]StatementRequest model)
        {
            var customerAccount = _accountRepository.GetSingle(x => x.Id == Convert.ToInt32(User.Identity.Name));
            if (customerAccount.ApplicationUserId != Convert.ToInt32(User.Identity.Name))
            {
                return new UnauthorizedResult();
            }

            IEnumerable<CustomerTransaction> result = _repository
                .AllIncluding(x => x.CustomerAccount)
                .Where(x => x.CustomerAccount.AccountNumber == model.AccountNumber && x.Created >= model.StartDate && x.Created <= model.EndDate)
                .OrderBy(c => c.Id)
                .ToList();

            IEnumerable<CustomerTransactionViewModel> vm = Mapper.Map<IEnumerable<CustomerTransaction>, IEnumerable<CustomerTransactionViewModel>>(result);
            foreach (var item in vm)
            {
                item.AccountName = _repository.GetCustomerName(item.CustomerAccount.Id);
            }
            return new OkObjectResult(vm);
        }


        [HttpPost("TransactionsReport", Name = "TransactionsReport")]
        [Authorize(Roles = "Administrator")]
        public IActionResult TransactionsReport([FromBody]PeriodicTransactionsRequest model)
        {
            var customerAccount = _accountRepository.GetSingle(x => x.Id == Convert.ToInt32(User.Identity.Name));
            if (customerAccount.ApplicationUserId != Convert.ToInt32(User.Identity.Name))
            {
                return new UnauthorizedResult();
            }
            List<CustomerTransaction> result = new List<CustomerTransaction>();
            if(model.Type == "Debits")
            {
                result = _repository
                .AllIncluding(x => x.CustomerAccount)
                .Where(x => x.Debit > 0 && x.Created >= model.StartDate && x.Created <= model.EndDate)
                .OrderBy(c => c.Id)
                .ToList();
            }
            else if (model.Type == "Credits")
            {
                result = _repository
                .AllIncluding(x => x.CustomerAccount)
                .Where(x => x.Credit > 0 && x.Created >= model.StartDate && x.Created <= model.EndDate)
                .OrderBy(c => c.Id)
                .ToList();
            }
            else
            {
                result = _repository
                .AllIncluding(x => x.CustomerAccount)
                .Where(x => x.Created >= model.StartDate && x.Created <= model.EndDate)
                .OrderBy(c => c.Id)
                .ToList();
            }

            IEnumerable<CustomerTransactionViewModel> vm = Mapper.Map<IEnumerable<CustomerTransaction>, IEnumerable<CustomerTransactionViewModel>>(result);
            foreach (var item in vm)
            {
                item.AccountName = _repository.GetCustomerName(item.CustomerAccount.Id);
            }
            return new OkObjectResult(vm);
        }
    }
}