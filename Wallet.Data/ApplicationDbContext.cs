using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.DomainEntities;
using Wallet.Core.Membership;

namespace Wallet.Data
{    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<CustomerTransaction> CustomerTransactions { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountType>().ToTable("AccountType");                
            modelBuilder.Entity<CustomerAccount>().ToTable("CustomerAccount");
            modelBuilder.Entity<CustomerTransaction>().ToTable("CustomerTransaction");
            base.OnModelCreating(modelBuilder);
        }
    }

}
