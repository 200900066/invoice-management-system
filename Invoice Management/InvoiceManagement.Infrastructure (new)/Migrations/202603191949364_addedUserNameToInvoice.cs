namespace InvoiceManagement.Infrastructure__new_.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedUserNameToInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "CreatedByUserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "CreatedByUserName");
        }
    }
}
