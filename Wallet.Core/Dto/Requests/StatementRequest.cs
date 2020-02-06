using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto.Requests
{
    public class StatementRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AccountNumber { get; set; }
    }
}
