using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanMyTrip.Web.Models
{
    public class Venue : IEquatable<Venue>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Category PrimaryCategory { get; set; }
        public GeoLocation GeoLocation { get; set; }
        public string MarkerIcon { get; set; }

        public bool Equals(Venue other)
        {
            return (other != null & other.Id == Id);
        }
    }
}