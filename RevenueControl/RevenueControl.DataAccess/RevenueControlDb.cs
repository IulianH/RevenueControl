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
        public DbSet<DataSource> DataSources { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
