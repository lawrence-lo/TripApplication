using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TripApplication.Models
{
    public class Trip
    {
        [Key]
        public int TripID { get; set; }
        public string TripName { get; set; }
        public DateTime TripFromDate { get; set; }
        public DateTime TripToDate { get; set; }
        public string TripRemarks { get; set; }

        //A trip can have many destinations
        public ICollection<Destination> Destinations { get; set; }
    }

    public class TripDto
    {
        public int TripID { get; set; }
        public string TripName { get; set; }
        public DateTime TripFromDate { get; set; }
        public DateTime TripToDate { get; set; }
        public string TripRemarks { get; set; }
    }
}