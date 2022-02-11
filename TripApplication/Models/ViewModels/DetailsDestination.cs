using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripApplication.Models.ViewModels
{
    public class DetailsDestination
    {
        public DestinationDto SelectedDestination { get; set; }
        public IEnumerable<TripDto> RelatedTrips { get; set; }
    }
}