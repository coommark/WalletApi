using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Shared
{
    public abstract class AuditedBaseEntity : BaseEntity
    {
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }

        public DateTime Modified { get; set; }
    }
}
