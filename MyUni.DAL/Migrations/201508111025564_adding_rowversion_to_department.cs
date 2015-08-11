namespace MyUni.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adding_rowversion_to_department : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Department", "RowVersion", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Department", "RowVersion");
        }
    }
}
