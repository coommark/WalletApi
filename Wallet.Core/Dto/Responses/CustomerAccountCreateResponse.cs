using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.Dto.ViewModels;

namespace Wallet.Core.Dto.Responses
{
    public class CustomerAccountCreateResponse : CreateResponseBase
    {
        public string AccountNumber { get; set; }
        public ApplicationUserViewModel ApplicationUser { get; set; }
        public AccountTypeChildViewModel AccountType { get; set; }
   }
}
