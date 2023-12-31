﻿using System;
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

        public List<CustomerAccountViewModel> CustomerAccounts { get; set; }
    }
}
