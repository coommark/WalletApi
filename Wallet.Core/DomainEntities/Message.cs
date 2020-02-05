using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Wallet.Core.Dto.ViewModels;
using Wallet.Core.Membership;
using Wallet.Core.Shared;

namespace Wallet.Core.DomainEntities
{
    public class Message : BaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string Type { get; set; }

        [Required]
        public string Body { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public List<MessageReply> MessageReplies { get; set; }

    }
}
