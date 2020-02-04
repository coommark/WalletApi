using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Wallet.Core.DomainEntities;
using Wallet.Data.Shared;

namespace Wallet.Data.Abstract
{
    public interface ICustomerAccountRepository : IRepository<CustomerAccount>
    {
        bool CheckCategoryAccountExist(int typeId, int customerId);
        decimal DailyTransactionLimit(int accountId);
        bool IsSameCustomerAccounts(int accountOneId, int accountTwoId);
        CustomerAccount AdminCurrenAccount(int adminId);
        CustomerAccount GetAccountByAccountNumber(string accountNumber);
        IEnumerable<CustomerAccount> AllIncludingWithBalance(params Expression<Func<CustomerAccount, object>>[] includeProperties);
        IEnumerable<CustomerAccount> GetCustomerAccounts(int id);
    }

}
