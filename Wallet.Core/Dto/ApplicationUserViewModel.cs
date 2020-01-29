using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto
{
    public class ApplicationUserViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public string ProfileImage { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
