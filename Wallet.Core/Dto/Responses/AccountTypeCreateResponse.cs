using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto.Responses
{
    public class AccountTypeCreateResponse : CreateResponseBase
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string CategoryCode { get; set; }
        public decimal MinimumBalance { get; set; }
        public bool AllowOverdraw { get; set; }
    }
}
