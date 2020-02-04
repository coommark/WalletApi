using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.DomainEntities;
using Wallet.Data.Abstract;
using Wallet.Data.Shared;

namespace Wallet.Data.Repositories
{
    public class MessageThreadRepository : Repository<MessageThread>, IMessageThreadRepository
    {
        private ApplicationDbContext _context;

        public MessageThreadRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
