using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.Membership;
using Wallet.Core.Shared;

namespace Wallet.Core.DomainEntities
{
    public class MessageThread : BaseEntity
    {
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public List<Message> Messages { get; set; }
    }
}
