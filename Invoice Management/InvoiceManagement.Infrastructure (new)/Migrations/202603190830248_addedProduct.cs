namespace InvoiceManagement.Infrastructure__new_.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InvoiceItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InvoiceId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Product_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .Index(t => t.InvoiceId)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        CostPerItem = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityInStock = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceItems", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.InvoiceItems", "InvoiceId", "dbo.Invoices");
            DropIndex("dbo.InvoiceItems", new[] { "Product_Id" });
            DropIndex("dbo.InvoiceItems", new[] { "InvoiceId" });
            DropTable("dbo.Products");
            DropTable("dbo.Invoices");
            DropTable("dbo.InvoiceItems");
        }
    }
}
