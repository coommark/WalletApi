using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Core.DomainEntities;
using Wallet.Core.Dto.ViewModels;
using Wallet.Data.Abstract;

namespace Wallet.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class DashboardsController : Controller
    {
        private readonly ICustomerAccountRepository _accountRepository;
        private readonly ICustomerTransactionRepository _transrepository;
        private readonly IUserRepository _usersRepository;
        private readonly IMessageRepository _messagesRepository;

        public DashboardsController(ICustomerAccountRepository accountRepository,
             ICustomerTransactionRepository transrepository, IUserRepository usersRepository,
             IMessageRepository messagesRepository
            )
        {
            _transrepository = transrepository;
            _accountRepository = accountRepository;
            _usersRepository = usersRepository;
            _messagesRepository = messagesRepository;
        }

        [HttpGet("AdminDashboard")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Admin()
        {
            
            AdminDashboardViewModel model = new AdminDashboardViewModel();
            model.AllTransactionsCount = await _transrepository.Count();
            model.AllAccountsCount = await _accountRepository.Count();
            model.AllAUsersCount = await _usersRepository.Count();
            model.AllMessagesCount = await _messagesRepository.Count();
            IEnumerable<CustomerTransaction> result = _transrepository
                       .AllIncluding(x => x.CustomerAccount)
                       .OrderByDescending(c => c.Id)
                       .Take(10)
                       .ToList();
            
            model.RecentTransactions = Mapper.Map<IEnumerable<CustomerTransaction>, IEnumerable<CustomerTransactionViewModel>>(result);
            foreach (var item in model.RecentTransactions)
            {
                item.AccountName = _transrepository.GetCustomerName(item.CustomerAccount.Id);
            }
            return new OkObjectResult(model);
        }


        [HttpGet("CustomerDashboard")]
        public IActionResult Customer()
        {
            CustomerDashboardViewModel model = new CustomerDashboardViewModel();
            var accounts = _accountRepository.GetCustomerAccounts(Convert.ToInt32(User.Identity.Name)).ToList();
            List<CustomerTransaction> transactions = new List<CustomerTransaction>();
            if (accounts.Count() > 0)
            {
                foreach (var account in accounts)
                {
                    IEnumerable<CustomerTransaction> result = _transrepository
                        .AllIncluding(x => x.CustomerAccount)
                        .Where(x => x.CustomerAccountId == account.Id)
                        .OrderByDescending(c => c.Id)
                        .Take(5)
                        .ToList();
                    transactions.AddRange(result);
                }
                model.AllAccounts = Mapper.Map<IEnumerable<CustomerAccount>, IEnumerable<CustomerAccountViewModel>>(accounts);
                model.RecentTransactions = Mapper.Map<IEnumerable<CustomerTransaction>, IEnumerable<CustomerTransactionViewModel>>(transactions);
            }

            return new OkObjectResult(model);
        }
    }
}