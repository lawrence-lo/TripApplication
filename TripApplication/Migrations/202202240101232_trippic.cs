namespace TripApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class trippic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trips", "TripHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Trips", "PicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trips", "PicExtension");
            DropColumn("dbo.Trips", "TripHasPic");
        }
    }
}
