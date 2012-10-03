using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanMyTrip.Web.Models
{
    public class VenueSearchRequest
    {
        public int Radius { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public GeoLocation Location { get; set; }
    }
}