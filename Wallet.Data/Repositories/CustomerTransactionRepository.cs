using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public decimal DailyTransactionTotal(int accountId)
        {
            return _context.CustomerTransactions.Where(x => x.CustomerAccountId == accountId && x.Created >= DateTime.Today && x.Created < DateTime.Today.AddDays(1)).Sum(x => x.Credit);
        }
    }
}
