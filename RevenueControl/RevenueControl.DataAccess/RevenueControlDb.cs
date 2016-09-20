using RevenueControl.DomainObjects.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DataAccess
{
    public class RevenueControlDb : DbContext
    {
        public RevenueControlDb() : base("name=DefaultConnection")
        {
            Database.SetInitializer<RevenueControlDb>(null);
        }
        public DbSet<Client> Clients { get; set; }

        public DbSet<DataSource> DataSources { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<TransactionTag> TransactionTags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            

            modelBuilder.Entity<Client>()
                .HasKey<string>(c => c.Name)
                .Property(p => p.Name).HasMaxLength(30);
            modelBuilder.Entity<Transaction>().Ignore(t => t.Tags);

            /*modelBuilder.Entity<DataSource>()
                .HasKey<int>(ds => ds.Id)
                .Property(p => p.BankAccount).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<DataSource>().Property(p => p.ClientName).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<DataSource>().Property(p => p.Culture).HasMaxLength(15).IsRequired();
            modelBuilder.Entity<DataSource>().Property(p => p.Name).HasMaxLength(50);


            modelBuilder.Entity<DataSource>().HasRequired<Client>(c => )


            modelBuilder.Entity<Transaction>().HasKey<int>(t => t.Id);*/

           
        }
    }
}
