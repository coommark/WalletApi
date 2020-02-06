using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.Api.Mailer
{
    public interface IEMailer
    {        
        void SendTransactionNotification(string to, string name, string amount, string balance, string message);       
    }
}
