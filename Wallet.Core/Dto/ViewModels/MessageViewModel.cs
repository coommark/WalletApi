using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto.ViewModels
{
    public class MessageViewModel : ViewModelBase
    {
        public string Type { get; set; }
        public string Body { get; set; }
        public ApplicationUserViewModel ApplicationUser { get; set; }
    }
}
