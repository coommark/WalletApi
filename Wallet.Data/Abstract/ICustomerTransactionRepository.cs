using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.DomainEntities;
using Wallet.Data.Shared;

namespace Wallet.Data.Abstract
{
    public interface ICustomerTransactionRepository : IRepository<CustomerTransaction>
    {
        decimal  AccountBalance(int accountId);
        decimal DailyTransactionTotal(int accountId);
        string GetCustomerName(int accountId);
        Task<int> CountForAccount(int accountId);
    }
}
