using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.Shared;

namespace Wallet.Core.DomainEntities
{
    public class CustomerTransactionBatch : BaseEntity
    {
        public int CustomerAccountId { get; set; }
        public CustomerAccount CustomerAccount { get; set; }

        public List<CustomerTransaction> CustomerTransaction { get; set; }
    }
}
