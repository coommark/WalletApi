using Microsoft.AspNetCore.Identity;
using System;

namespace Wallet.Core.Membership 
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime LastLogin { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
