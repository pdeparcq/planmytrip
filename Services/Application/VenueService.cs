using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanMyTrip.Web.Services.External;
using PlanMyTrip.Web.Models;
using Ninject;

namespace PlanMyTrip.Web.Services.Application
{
    public class VenueService
    {
        [Inject]
        public CategoryService CategoryService { get; set; }

        [Inject]
        public FoursquareService Service { get; set; }

        public IEnumerable<Venue> SearchVenues(VenueSearchRequest request)
        {
            if (request.Location == null)
                return new List<Venue>();
            
            var parameters = new Dictionary<string, string>();
            parameters.Add("ll", string.Format("{0:0.######},{1:0.######}", request.Location.Latitude, request.Location.Longitude));
            parameters.Add("limit", "25");
            parameters.Add("radius", request.Radius.ToString());
            parameters.Add("intent", "browse");

            //Add list of categories to search for
            var categoryList = string.Join(",",(from c in request.Categories select c.Id).ToArray());
            parameters.Add("categoryId", categoryList);

            return CreateVenues(Service.SearchVenues(parameters));
        }

        private IEnumerable<Venue> CreateVenues(IEnumerable<Contract.Data.Venue> venues)
        {
            var result = new List<Venue>();
            if (venues != null)
            {
                foreach (var v in venues)
                {
                    var venue = CreateVenue(v);
                    if (venue.PrimaryCategory != null)
                        result.Add(venue);
                }
            }
            return result;
        }

        private Venue CreateVenue(Contract.Data.Venue venue)
        {
            var v = new Venue();
            var categoryId = (from c in venue.categories where c.primary select c.id).FirstOrDefault();
            if (categoryId != null)
                v.PrimaryCategory = CategoryService.FindCategory(categoryId);
            v.Name = venue.name;
            v.Url = venue.url;
            v.GeoLocation = new GeoLocation { 
                Latitude = venue.location.lat, 
                Longitude = venue.location.lng };

            return v;
        }
    }
}