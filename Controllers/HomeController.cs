using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlanMyTrip.Web.Services.Application;
using PlanMyTrip.Web.Models;
using Ninject;

namespace PlanMyTrip.Web.Controllers
{
    public class HomeController : Controller
    {
        private const string SessionContextKey = "SessionContext";

        [Inject]
        public CategoryService CategoryService { get; set; }

        [Inject]
        public VenueService VenueService { get; set; }


        private SessionContext Context
        {
            get
            {
                if (Session[SessionContextKey] == null)
                    Session[SessionContextKey] = new SessionContext
                    {
                        MainCategories = new List<Category>(CategoryService.GetMainCategories()),
                        SearchRequest = new VenueSearchRequest
                        {
                            Radius = 10000,
                            Categories = CategoryService.GetMainCategories()
                        },
                        PlannedTrip = new List<Venue>()
                    };
                return Session[SessionContextKey] as SessionContext;
            }
        }

        private IEnumerable<Models.Venue> FindSuggestedVenues()
        {
            var selectedCategories = Context.SearchRequest.Categories;
            var venues = VenueService.SearchVenues(
                Context.SearchRequest);
            venues = venues.Where(x => selectedCategories.Contains(x.PrimaryCategory.RootCategory) && !Context.PlannedTrip.Contains(x));
            int i = 0;
            foreach (var venue in venues)
            {
                venue.MarkerIcon = string.Format("http://www.google.com/mapfiles/marker{0}.png", (char)('A' + i++));
            }
            return venues;
        }

        private void ClearSuggestedVenues()
        {
            Context.SuggestedVenues = null;
        }

        private IEnumerable<Venue> GetSuggestedVenues()
        {
            if (Context.SuggestedVenues == null)
                Context.SuggestedVenues = new List<Venue>(FindSuggestedVenues());
            return Context.SuggestedVenues;
        }

        public IEnumerable<Venue> GetPlannedTrip()
        {
            int i=1;
            foreach (var venue in Context.PlannedTrip)
            {
                venue.MarkerIcon = Url.Content(string.Format("Content/images/markerIcons/largeTDBlueIcons/marker{0}.png", i++));
            }
            return Context.PlannedTrip;
        }

        public ActionResult Index()
        {
            Session.Clear();
            return View();
        }

        [HttpGet]
        [OutputCache(VaryByHeader="Accept", Duration=0, NoStore=true)]
        public ActionResult SuggestedVenues()
        {
            if(Request.AcceptTypes.Contains("application/json"))
                return Json(GetSuggestedVenues(), JsonRequestBehavior.AllowGet);
            else
                return PartialView("_SuggestedVenues", GetSuggestedVenues());
        }

        [HttpGet]
        [OutputCache(VaryByHeader = "Accept", Duration = 0, NoStore = true)]
        public ActionResult PlannedTrip()
        {
            if (Request.AcceptTypes.Contains("application/json"))
                return Json(GetPlannedTrip(), JsonRequestBehavior.AllowGet);
            else
                return PartialView("_PlannedTrip", GetPlannedTrip());
        }

        [HttpGet]
        [OutputCache(VaryByHeader = "Accept", Duration = 0, NoStore = true)]
        public ActionResult MainCategories()
        {
            if (Request.AcceptTypes.Contains("application/json"))
                return Json(CategoryService.GetMainCategories(), JsonRequestBehavior.AllowGet);
            else
                return PartialView("_MainCategories", CategoryService.GetMainCategories());
        }

        [HttpPost]
        public void ToggleMainCategory(string categoryId)
        {
            var categories = Context.MainCategories;
            var category = (from c in categories where c.Id == categoryId select c).SingleOrDefault();
            category.IsSelected = !category.IsSelected;
            Context.SearchRequest.Categories = categories.Where(x => x.IsSelected);
            ClearSuggestedVenues();
        }

        [HttpPost]
        public void AddSuggestedVenueToPlan(string venueId)
        {
            if (Context.SuggestedVenues == null)
                return;
            var venue = Context.SuggestedVenues.Where(x => x.Id == venueId).SingleOrDefault();
            if (venue != null)
            {
                Context.PlannedTrip.Add(venue);
                Context.SuggestedVenues.Remove(venue);
            }
        }

        [HttpPost]
        public void RemoveVenueFromPlan(string venueId)
        {
            var venue = Context.PlannedTrip.Where(x => x.Id == venueId).SingleOrDefault();
            if (venue != null)
            {
                Context.PlannedTrip.Remove(venue);
                ClearSuggestedVenues();
            }
        }

        [HttpPost]
        public void SetSearchRadius(int radius)
        {
            Context.SearchRequest.Radius = radius;
            ClearSuggestedVenues();
        }

        [HttpPost]
        public void SetSearchLocation(double latitude, double longitude)
        {
            Context.SearchRequest.Location = new GeoLocation { Latitude = latitude, Longitude = longitude };
            ClearSuggestedVenues();
        }
    }
}
