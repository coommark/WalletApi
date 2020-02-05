using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Wallet.Core.Shared;

namespace Wallet.Core.DomainEntities
{
    public class CustomerTransaction : BaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string TransactionType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Credit { get; set; } = 0.0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Debit { get; set; } = 0.0m;

        [Required]
        [Column(TypeName = "nvarchar(256)")]
        public string Description { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string AuditDescription { get; set; }

        public int CustomerAccountId { get; set; }
        public CustomerAccount CustomerAccount { get; set; }

        public int CustomerTransactionBatchId { get; set; }

    }
}
