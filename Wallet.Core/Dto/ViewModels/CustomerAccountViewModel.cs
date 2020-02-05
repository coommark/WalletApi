using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto.ViewModels
{
    public class CustomerAccountViewModel : ViewModelBase
    {
        public string AccountNumber { get; set; }
        public decimal DailyTransactionLimit { get; set; }
        public decimal Balance { get; set; } = 0.0m;
        public int CurrentStatusId { get; set; }
        public ApplicationUserViewModel ApplicationUser { get; set; }
        public AccountTypeChildViewModel AccountType { get; set; }
        public CustomerAccountStatusViewModel AccountStatus { get; set; }
        public List<CustomerTransactionBatchViewModel> CustomerTransactionBatches { get; set; }
        public List<CustomerAccountStatusViewModel> CustomerAccountStatuses { get; set; }
    }
}
