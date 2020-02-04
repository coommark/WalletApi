using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.DomainEntities;
using Wallet.Data.Shared;

namespace Wallet.Data.Abstract
{
    public interface ICustomerTransactionRepository : IRepository<CustomerTransaction>
    {
        decimal  AccountBalance(int accountId);
        decimal DailyTransactionTotal(int accountId);        
    }
}
