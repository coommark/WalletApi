using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool CheckCategoryAccountExist(int typeId, int customerId)
        {
            return _context.CustomerAccounts.Any(x => x.ApplicationUserId == customerId && x.AccountTypeId == typeId);
        }
    }
}
