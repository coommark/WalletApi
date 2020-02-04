using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.Membership;
using Wallet.Data.Shared;

namespace Wallet.Data.Abstract
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
    }
}
