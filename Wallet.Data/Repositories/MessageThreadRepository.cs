using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.DomainEntities;
using Wallet.Data.Abstract;
using Wallet.Data.Shared;

namespace Wallet.Data.Repositories
{
    public class MessageReplyRepository : Repository<MessageReply>, IMessageReplyRepository
    {
        private ApplicationDbContext _context;

        public MessageReplyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
