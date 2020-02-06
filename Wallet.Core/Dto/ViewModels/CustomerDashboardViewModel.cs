using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public IEnumerable<CustomerAccountViewModel> AllAccounts { get; set; }
        public IEnumerable<CustomerTransactionViewModel> RecentTransactions { get; set; }
    }
}
