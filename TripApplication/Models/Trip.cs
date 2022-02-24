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

        //data needed for keeping track of trip images uploaded
        //images deposited into /Content/Images/Trips/{id}.{extension}
        public bool TripHasPic { get; set; }
        public string PicExtension { get; set; }

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

        //data needed for keeping track of trip images uploaded
        //images deposited into /Content/Images/Trips/{id}.{extension}
        public bool TripHasPic { get; set; }
        public string PicExtension { get; set; }
    }
}