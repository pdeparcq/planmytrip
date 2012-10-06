using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using PlanMyTrip.Web.Contract.Data;
using System.Diagnostics;

namespace PlanMyTrip.Web.Services.External
{
    public class FoursquareService : IAuthenticator
    {
        private RestClient _fourSquareClient;

        public FoursquareService()
        {
            _fourSquareClient = new RestClient("https://api.foursquare.com");
            _fourSquareClient.Authenticator = this;
            _fourSquareClient.AddDefaultParameter("v","20121006",ParameterType.GetOrPost);
        }

        public IEnumerable<Category> GetCategories()
        {
            var request = new RestRequest("v2/venues/categories", Method.GET);
            var response = _fourSquareClient.Execute<VenueCategories>(request);
            return response.Data.response.categories;
        }

        public IEnumerable<Venue> SearchVenues(IDictionary<string,string> parameters)
        {
            var request = new RestRequest("v2/venues/search", Method.GET);
            foreach (var kvp in parameters)
            {
                request.AddParameter(kvp.Key, kvp.Value);
            }
            var response = _fourSquareClient.Execute<VenueSearchResult>(request);
            while (response.Data.meta.code != 200)
            {
                Debug.Print(string.Format("Foursquare error {0} {1}: {2}", response.Data.meta.code, response.Data.meta.errorType, response.Data.meta.errorDetail));
                response = _fourSquareClient.Execute<VenueSearchResult>(request);
            }
            return response.Data.response.venues;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddParameter("client_id", "3EPMASFPHUHWGOVDJ33FOO3MGFKXULX55XHHRJA3DXTI2ONR");
            request.AddParameter("client_secret", "DI2TMCZZ32HGUILURIEG2ZLO2EYJNPR31IW0RURQ2NUG3YVK");
        }
    }
}