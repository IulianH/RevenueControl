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
    }
}
