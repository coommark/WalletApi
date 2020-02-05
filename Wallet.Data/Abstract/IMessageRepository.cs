using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.DomainEntities;
using Wallet.Data.Shared;

namespace Wallet.Data.Abstract
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<int> CountForUser(int userId);
    }
}
