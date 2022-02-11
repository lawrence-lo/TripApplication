namespace TripApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class destinationstrips : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Destinations",
                c => new
                    {
                        DestinationID = c.Int(nullable: false, identity: true),
                        DestinationName = c.String(),
                        DestinationCountry = c.String(),
                        DestinationLatitude = c.String(),
                        DestinationLongitude = c.String(),
                    })
                .PrimaryKey(t => t.DestinationID);
            
            CreateTable(
                "dbo.DestinationTrips",
                c => new
                    {
                        Destination_DestinationID = c.Int(nullable: false),
                        Trip_TripID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Destination_DestinationID, t.Trip_TripID })
                .ForeignKey("dbo.Destinations", t => t.Destination_DestinationID, cascadeDelete: true)
                .ForeignKey("dbo.Trips", t => t.Trip_TripID, cascadeDelete: true)
                .Index(t => t.Destination_DestinationID)
                .Index(t => t.Trip_TripID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DestinationTrips", "Trip_TripID", "dbo.Trips");
            DropForeignKey("dbo.DestinationTrips", "Destination_DestinationID", "dbo.Destinations");
            DropIndex("dbo.DestinationTrips", new[] { "Trip_TripID" });
            DropIndex("dbo.DestinationTrips", new[] { "Destination_DestinationID" });
            DropTable("dbo.DestinationTrips");
            DropTable("dbo.Destinations");
        }
    }
}
