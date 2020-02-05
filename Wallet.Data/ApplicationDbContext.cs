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
        public DbSet<CustomerAccountStatus> CustomerAccountStatuses { get; set; }
        public DbSet<CustomerTransactionBatch> CustomerTransactionBatches { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageReply> MessageReplies { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountType>().ToTable("AccountType");                
            modelBuilder.Entity<CustomerAccount>().ToTable("CustomerAccount");
            modelBuilder.Entity<CustomerTransaction>().ToTable("CustomerTransaction");
            modelBuilder.Entity<CustomerAccountStatus>().ToTable("CustomerAccountStatus");
            modelBuilder.Entity<CustomerTransactionBatch>().ToTable("CustomerTransactionBatch");
            modelBuilder.Entity<Message>().ToTable("Message");
            modelBuilder.Entity<MessageReply>().ToTable("MessageReply");
            base.OnModelCreating(modelBuilder);
        }
    }

}
