using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Wallet.Core.DomainEntities;
using Wallet.Data.Abstract;
using Wallet.Data.Shared;

namespace Wallet.Data.Repositories
{
    public class CustomerAccountRepository : Repository<CustomerAccount>, ICustomerAccountRepository
    {
        private ApplicationDbContext _context;

        public CustomerAccountRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public CustomerAccount AdminCurrenAccount(int adminId)
        {
            var accounts = _context.CustomerAccounts
                .Include(x => x.AccountType)
                .Include(x => x.ApplicationUser)
                .Where(x => x.ApplicationUserId == adminId);
            return accounts.SingleOrDefault(x => x.AccountType.Type == "Current Account");
        }

        public IEnumerable<CustomerAccount> AllIncludingWithBalance(params Expression<Func<CustomerAccount, object>>[] includeProperties)
        {
            IQueryable<CustomerAccount> query = _context.Set<CustomerAccount>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            var result = query.AsEnumerable();
            foreach(var item in result)
            {
                item.Balance = _context.CustomerTransactions.Where(x => x.CustomerAccountId == item.Id).Sum(x => x.Debit)
                - _context.CustomerTransactions.Where(x => x.CustomerAccountId == item.Id).Sum(x => x.Credit);
            }
            return result;
        }

        public bool CheckCategoryAccountExist(int typeId, int customerId)
        {
            return _context.CustomerAccounts.Any(x => x.ApplicationUserId == customerId && x.AccountTypeId == typeId);
        }

        public decimal DailyTransactionLimit(int accountId)
        {
            var account = _context.CustomerAccounts.SingleOrDefault(x => x.Id == accountId);
            decimal dailyCatLimit = _context.AccountTypes.SingleOrDefault(x => x.Id == account.AccountTypeId).DailyTransactionLimit;
            if(account.DailyTransactionLimit >= dailyCatLimit)
            {
                return account.DailyTransactionLimit;
            }
            else
            {
                return dailyCatLimit;
            }
            
        }

        public CustomerAccount GetAccountByAccountNumber(string accountNumber)
        {
            return _context.CustomerAccounts
                .Include(X => X.ApplicationUser)
                .SingleOrDefault(x => x.AccountNumber == accountNumber);
        }

        

        public IEnumerable<CustomerAccount> GetCustomerAccounts(int id)
        {
            var result = _context.CustomerAccounts
               .Include(x => x.AccountType)
               .Include(x => x.ApplicationUser)
               .Where(x => x.ApplicationUserId == id);
            foreach (var item in result)
            {
                item.Balance = _context.CustomerTransactions.Where(x => x.CustomerAccountId == item.Id).Sum(x => x.Debit)
                - _context.CustomerTransactions.Where(x => x.CustomerAccountId == item.Id).Sum(x => x.Credit);
            }
            return result;
        }

        public CustomerAccount GetSingleWithBalance(int accountId)
        {
            var account = _context.CustomerAccounts
                .Include(X => X.ApplicationUser)
                .SingleOrDefault(x => x.Id == accountId);
            account.Balance  = _context.CustomerTransactions.Where(x => x.CustomerAccountId == account.Id).Sum(x => x.Debit)
                - _context.CustomerTransactions.Where(x => x.CustomerAccountId == account.Id).Sum(x => x.Credit);

            return account;
        }

        public bool IsSameCustomerAccounts(int accountOneId, int accountTwoId)
        {
            if(_context.CustomerAccounts.SingleOrDefault(x => x.Id == accountOneId).ApplicationUserId == 
                _context.CustomerAccounts.SingleOrDefault(x => x.Id == accountTwoId).ApplicationUserId)
            {
                return true;
            }
            return false;
        }
    }
}
