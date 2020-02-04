using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public class AccountTypeRepository : Repository<AccountType>, IAccountTypeRepository
    {
        private ApplicationDbContext _context;

        public AccountTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public bool CheckCategoryCodeExist(string code)
        {
            return _context.AccountTypes.Any(x => x.CategoryCode == code);
        }

        public bool CheckTypeExist(string type)
        {
            return _context.AccountTypes.Any(x => x.Type == type);
        }        

        public bool IsAllowDebit(int id)
        {
            return _context.AccountTypes.SingleOrDefault(x => x.Id == id).AllowOverdraw;
        }
    }
}
