using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Wallet.Core.Shared;

namespace Wallet.Core.DomainEntities
{
    public class AccountType : AuditedBaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string Type { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(256)")]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string CategoryCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumBalance { get; set; } = 0.0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DailyTransactionLimit { get; set; } = 0.0m;

        public bool AllowOverdraw { get; set; }

        public List<CustomerAccount> CustomerAccounts { get; set; }
    }
}
