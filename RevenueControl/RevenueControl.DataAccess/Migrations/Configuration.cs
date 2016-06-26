namespace RevenueControl.DataAccess.Migrations
{
    using DomainObjects.Entities;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<RevenueControl.DataAccess.RevenueControlDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "RevenueControl.DataAccess.RevenueControlDb";
        }

        protected override void Seed(RevenueControlDb context)
        {
            context.Clients.AddOrUpdate(c => c.Name,
            new Client
            {
                Name = "DefaultClient",
                DataSources = new List<DataSource> {
                    new DataSource
                    {
                        BankAccount = "RO15INGB0000999901728793",
                        Name = "ING"
                    }
                }
            });
        }
    }
}
