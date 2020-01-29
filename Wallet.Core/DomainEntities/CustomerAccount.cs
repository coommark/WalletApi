using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Wallet.Core.Membership;
using Wallet.Core.Shared;

namespace Wallet.Core.DomainEntities
{
    public class CustomerAccount : AuditedBaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string AccountNumber { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int AccountTypeId { get; set; }
        public AccountType AccountType { get; set; }        

        [Column(TypeName = "decimal(18,2)")]
        public decimal DailyTransactionLimit { get; set; } = 0.0m;

        public List<CustomerTransactionBatch> CustomerTransactionBatches { get; set; }
        public List<CustomerAccountStatus> CustomerAccountStatuses { get; set; }
    }
}
