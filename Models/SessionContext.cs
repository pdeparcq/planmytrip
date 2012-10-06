using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanMyTrip.Web.Models
{
    public class SessionContext
    {
        public List<Category> MainCategories { get; set; }
        public VenueSearchRequest SearchRequest { get; set; }
        public List<Venue> SuggestedVenues { get; set; }
        public List<Venue> PlannedTrip { get; set; }
    }
}