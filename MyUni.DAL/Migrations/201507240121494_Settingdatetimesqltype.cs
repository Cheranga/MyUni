namespace MyUni.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Settingdatetimesqltype : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Department", "StartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Instructor", "HireDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Student", "EnrolledDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Student", "EnrolledDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Instructor", "HireDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Department", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
