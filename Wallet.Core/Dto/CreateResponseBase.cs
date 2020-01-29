using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto
{
    public abstract class CreateResponseBase
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public int CreatedBy { get; set; }
    }
}
