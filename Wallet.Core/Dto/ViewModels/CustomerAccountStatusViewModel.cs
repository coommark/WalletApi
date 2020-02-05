using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.DomainEntities;

namespace Wallet.Core.Dto.ViewModels
{
    public class CustomerAccountStatusViewModel : ViewModelBase
    {
        public string Status { get; set; }
        public bool IsCurrentStatus { get; set; }
        public string Comment { get; set; }
    }
}
