using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Wallet.Core.Shared;

namespace Wallet.Core.DomainEntities
{
    public class MessageReply : BaseEntity
    {
        [Required]
        public string Body { get; set; }

        public int MessageId { get; set; }
        public Message Message { get; set; }

        public int ApplicationUserId { get; set; }
    }
}
