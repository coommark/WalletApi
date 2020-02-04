using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto.ViewModels
{
    public class MessageThreadViewModel : ViewModelBase
    {
        public int ApplicationUserId { get; set; }
        public ApplicationUserViewModel ApplicationUser { get; set; }

        public List<MessageViewModel> Messages { get; set; }
    }
}
