using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto.ViewModels
{
    public class MessageThreadViewModel : ViewModelBase
    {        
        public List<MessageViewModel> Messages { get; set; }
    }
}
