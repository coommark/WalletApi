using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto.ViewModels
{
    public class CustomerAccountViewModel : ViewModelBase
    {
        public string AccountNumber { get; set; }
        public ApplicationUserViewModel ApplicationUser { get; set; }
        public AccountTypeChildViewModel AccountType { get; set; }
    }
}
