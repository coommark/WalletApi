using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Wallet.Core.Membership;
using Wallet.Core.Shared;

namespace Wallet.Core.DomainEntities
{
    public class CustomerTransactionBatch : BaseEntity
    {
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string Type { get; set; }
    }
}
