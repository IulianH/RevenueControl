namespace RevenueControl.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DataSources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BankAccount = c.String(),
                        Name = c.String(),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransactionDate = c.DateTime(nullable: false),
                        TransactionDetails = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionType = c.Int(nullable: false),
                        OtherDetails = c.String(),
                        DataSourceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataSources", t => t.DataSourceId, cascadeDelete: true)
                .Index(t => t.DataSourceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DataSources", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Transactions", "DataSourceId", "dbo.DataSources");
            DropIndex("dbo.Transactions", new[] { "DataSourceId" });
            DropIndex("dbo.DataSources", new[] { "ClientId" });
            DropTable("dbo.Transactions");
            DropTable("dbo.DataSources");
            DropTable("dbo.Clients");
        }
    }
}
