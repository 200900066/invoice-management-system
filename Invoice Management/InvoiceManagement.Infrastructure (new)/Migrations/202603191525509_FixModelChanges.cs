namespace InvoiceManagement.Infrastructure__new_.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixModelChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InvoiceItems", "Product_Id", "dbo.Products");
            DropIndex("dbo.InvoiceItems", new[] { "Product_Id" });
            DropColumn("dbo.InvoiceItems", "ProductId");
            RenameColumn(table: "dbo.InvoiceItems", name: "Product_Id", newName: "ProductId");
            AlterColumn("dbo.InvoiceItems", "ProductId", c => c.Int(nullable: false));
            AlterColumn("dbo.InvoiceItems", "ProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.InvoiceItems", "ProductId");
            AddForeignKey("dbo.InvoiceItems", "ProductId", "dbo.Products", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceItems", "ProductId", "dbo.Products");
            DropIndex("dbo.InvoiceItems", new[] { "ProductId" });
            AlterColumn("dbo.InvoiceItems", "ProductId", c => c.Int());
            AlterColumn("dbo.InvoiceItems", "ProductId", c => c.Guid(nullable: false));
            RenameColumn(table: "dbo.InvoiceItems", name: "ProductId", newName: "Product_Id");
            AddColumn("dbo.InvoiceItems", "ProductId", c => c.Guid(nullable: false));
            CreateIndex("dbo.InvoiceItems", "Product_Id");
            AddForeignKey("dbo.InvoiceItems", "Product_Id", "dbo.Products", "Id");
        }
    }
}
