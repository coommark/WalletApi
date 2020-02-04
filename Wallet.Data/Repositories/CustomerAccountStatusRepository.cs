using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wallet.Core.DomainEntities;
using Wallet.Data.Abstract;
using Wallet.Data.Shared;

namespace Wallet.Data.Repositories
{
    public class CustomerAccountStatusRepository : Repository<CustomerAccountStatus>, ICustomerAccountStatusRepository
    {
        private ApplicationDbContext _context;

        public CustomerAccountStatusRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async void UpdadeCurrent(int accountId, int modBy)
        {
            var result = _context.CustomerAccountStatuses.SingleOrDefault(x => x.CustomerAccountId == accountId && x.IsCurrentStatus);
            if(result != null)
            {
                result.IsCurrentStatus = false;
                result.Modified = DateTime.UtcNow;
                result.ModifiedBy = modBy;
            }
            await _context.SaveChangesAsync();
        }
    }
}
