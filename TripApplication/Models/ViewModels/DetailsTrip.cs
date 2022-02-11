using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripApplication.Models.ViewModels
{
    public class DetailsTrip
    {
        public TripDto SelectedTrip { get; set; }
        public IEnumerable<DestinationDto> RelatedDestinations { get; set; }
        public IEnumerable<DestinationDto> UnrelatedDestinations { get; set; }
    }
}