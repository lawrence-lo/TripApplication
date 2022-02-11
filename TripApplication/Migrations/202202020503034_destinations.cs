namespace TripApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class destinations : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DestinationTrips", newName: "TripDestinations");
            DropPrimaryKey("dbo.TripDestinations");
            AddPrimaryKey("dbo.TripDestinations", new[] { "Trip_TripID", "Destination_DestinationID" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TripDestinations");
            AddPrimaryKey("dbo.TripDestinations", new[] { "Destination_DestinationID", "Trip_TripID" });
            RenameTable(name: "dbo.TripDestinations", newName: "DestinationTrips");
        }
    }
}
