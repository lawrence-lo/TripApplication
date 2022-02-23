using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using TripApplication.Models;
using TripApplication.Models.ViewModels;
using System.Web.Script.Serialization;
//using System.Diagnostics;

namespace TripApplication.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static HomeController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44399/api/");
        }
        public ActionResult Index()
        {
            DetailsHome ViewModel = new DetailsHome();

            //objective: communicate with our ListDestinationsRelatedToTrips data api to retrieve all destinations that are linked to any trip
            //curl https://localhost:44399/api/DestinationData/ListDestinationsRelatedToTrips/

            string url = "destinationdata/listdestinationsrelatedtotrips/";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<DestinationDto> Destinations = response.Content.ReadAsAsync<IEnumerable<DestinationDto>>().Result;

            //add DestinationName to an array
            string[] DestinationNames = Destinations.Select(d => d.DestinationName).ToArray();
            //count distinct only
            int DestinationCount = DestinationNames.Distinct().Count();
            ViewModel.DestinationCount = DestinationCount;

            //add DestinationCountry to an array
            string[] DestinationCountries = Destinations.Select(d => d.DestinationCountry).ToArray();
            //count distinct only
            int CountryCount = DestinationCountries.Distinct().Count();
            ViewModel.CountryCount = CountryCount;

            return View(ViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}