using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.DomainEntities;
using Wallet.Data.Shared;

namespace Wallet.Data.Abstract
{
    public interface ICustomerAccountRepository : IRepository<CustomerAccount>
    {
        bool CheckCategoryAccountExist(int typeId, int customerId);
    }

}
