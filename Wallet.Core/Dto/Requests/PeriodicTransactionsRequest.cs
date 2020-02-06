using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto.Requests
{
    public class PeriodicTransactionsRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
       
    }
}
