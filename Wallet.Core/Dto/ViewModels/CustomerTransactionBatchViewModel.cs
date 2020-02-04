using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto.ViewModels
{
    public class CustomerTransactionBatchViewModel : ViewModelBase
    {
        public int CustomerAccountId { get; set; }
        public int ApplicationUserId { get; set; }
        public string Type { get; set; }

        public CustomerAccountViewModel CustomerAccount { get; set; }
        public ApplicationUserViewModel ApplicationUser { get; set; }
        public List<CustomerTransactionViewModel> CustomerTransaction { get; set; }
    }
}
