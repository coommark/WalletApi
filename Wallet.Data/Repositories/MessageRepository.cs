using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.DomainEntities;
using Wallet.Data.Abstract;
using Wallet.Data.Shared;

namespace Wallet.Data.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> CountForUser(int userId)
        {
            return await _context.Messages
                .Where(x => x.ApplicationUserId == userId)
                .CountAsync();
        }
        
    }
}
