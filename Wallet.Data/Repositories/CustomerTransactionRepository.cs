using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.DomainEntities;
using Wallet.Data.Abstract;
using Wallet.Data.Shared;

namespace Wallet.Data.Repositories
{
    public class CustomerTransactionRepository : Repository<CustomerTransaction>, ICustomerTransactionRepository
    {
        private ApplicationDbContext _context;

        public CustomerTransactionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public decimal AccountBalance(int accountId)
        {
            decimal balance = _context.CustomerTransactions.Where(x => x.CustomerAccountId == accountId).Sum(x => x.Debit)
                - _context.CustomerTransactions.Where(x => x.CustomerAccountId == accountId).Sum(x => x.Credit);

            return balance;
        }

       

        public async Task<int> CountForAccount(int accountId)
        {
            return await _context.CustomerTransactions
                .Where(x => x.CustomerAccountId == accountId)
                .CountAsync();
        }

        public decimal DailyTransactionTotal(int accountId)
        {
            return _context.CustomerTransactions.Where(x => x.CustomerAccountId == accountId && x.Created >= DateTime.Today && x.Created < DateTime.Today.AddDays(1)).Sum(x => x.Credit);
        }

        public string GetCustomerName(int accountId)
        {
            var account = _context.CustomerAccounts
                .Include(x => x.ApplicationUser)
                .SingleOrDefault(x => x.Id == accountId);
            return account.ApplicationUser.FullName;
        }
    }
}
