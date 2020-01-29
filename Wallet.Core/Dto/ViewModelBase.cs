using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto
{
    public abstract class ViewModelBase
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
    }
}
