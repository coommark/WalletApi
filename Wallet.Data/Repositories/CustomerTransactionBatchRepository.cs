using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.DomainEntities;
using Wallet.Data.Abstract;
using Wallet.Data.Shared;

namespace Wallet.Data.Repositories
{
    public class CustomerTransactionBatchRepository : Repository<CustomerTransactionBatch>, ICustomerTransactionBatchRepository
    {
        private ApplicationDbContext _context;

        public CustomerTransactionBatchRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
