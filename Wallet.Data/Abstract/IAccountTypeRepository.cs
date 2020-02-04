using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.DomainEntities;
using Wallet.Data.Shared;

namespace Wallet.Data.Abstract
{
    public interface IAccountTypeRepository : IRepository<AccountType>
    {
        bool CheckTypeExist(string type);
        bool CheckCategoryCodeExist(string code);
        bool IsAllowDebit(int id);
    }
}
