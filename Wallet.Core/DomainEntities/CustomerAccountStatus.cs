using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Wallet.Core.Shared;

namespace Wallet.Core.DomainEntities
{
    public class CustomerAccountStatus : AuditedBaseEntity
    {
        public int CustomerAccountId { get; set; }
        public CustomerAccount CustomerAccount { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string Status { get; set; }

        public bool IsCurrentStatus { get; set; }

        [Required]
        public string Comment { get; set; }

    }
}
