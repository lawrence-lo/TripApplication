using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TripApplication.Models
{
    public class Destination
    {
        [Key]
        public int DestinationID { get; set; }
        public string DestinationName { get; set; }
        public string DestinationCountry { get; set; }
        public string DestinationLatitude { get; set; }
        public string DestinationLongitude { get; set; }

        //A destination can be in many trips
        public ICollection<Trip> Trips { get; set; }

    }

    public class DestinationDto
    {
        public int DestinationID { get; set; }
        public string DestinationName { get; set; }
        public string DestinationCountry { get; set; }
        public string DestinationLatitude { get; set; }
        public string DestinationLongitude { get; set; }
    }
}