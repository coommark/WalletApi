using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.DomainEntities;

namespace Wallet.Core.Dto.ViewModels
{
    public class AccountTypeViewModel : ViewModelBase
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string CategoryCode { get; set; }
        public decimal MinimumBalance { get; set; }
        public bool AllowOverdraw { get; set; }

        public List<CustomerAccount> CustomerAccounts { get; set; }
    }
}
