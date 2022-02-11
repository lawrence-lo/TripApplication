namespace TripApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class trips : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trips",
                c => new
                    {
                        TripID = c.Int(nullable: false, identity: true),
                        TripName = c.String(),
                        TripFromDate = c.DateTime(nullable: false),
                        TripToDate = c.DateTime(nullable: false),
                        TripRemarks = c.String(),
                    })
                .PrimaryKey(t => t.TripID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Trips");
        }
    }
}
