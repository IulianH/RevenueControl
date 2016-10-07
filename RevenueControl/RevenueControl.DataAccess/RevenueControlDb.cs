using System.Data.Entity;
using RevenueControl.DomainObjects.Entities;

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
    }
}