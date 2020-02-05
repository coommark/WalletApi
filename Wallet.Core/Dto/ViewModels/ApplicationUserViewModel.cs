using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Core.Dto.ViewModels
{
    public class ApplicationUserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public string ProfileImage { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
