using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
//using System.Diagnostics;
using TripApplication.Models;
using TripApplication.Models.ViewModels;
using System.Web.Script.Serialization;

namespace TripApplication.Controllers
{
    public class TripController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static TripController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44399/api/");
        }

        // GET: Trip/List
        public ActionResult List()
        {
            //objective: communicate with our trip data api to retrieve a list of trips
            //curl https://localhost:44399/api/tripdata/listtrips

            string url = "tripdata/listtrips";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<TripDto> trips = response.Content.ReadAsAsync<IEnumerable<TripDto>>().Result;

            return View(trips);
        }

        // GET: Trip/Details/5
        public ActionResult Details(int id)
        {
            DetailsTrip ViewModel = new DetailsTrip();

            //objective: communicate with our trip data api to retrieve one trip
            //curl https://localhost:44399/api/tripdata/findtrip/{id}

            string url = "tripdata/findtrip/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            TripDto SelectedTrip = response.Content.ReadAsAsync<TripDto>().Result;

            ViewModel.SelectedTrip = SelectedTrip;

            //show associated destinations with this trip
            url = "destinationdata/listdestinationsfortrip/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<DestinationDto> RelatedDestinations = response.Content.ReadAsAsync<IEnumerable<DestinationDto>>().Result;

            ViewModel.RelatedDestinations = RelatedDestinations;

            url = "destinationdata/listdestinationsnotrelatedtotrip/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<DestinationDto> UnrelatedDestinations = response.Content.ReadAsAsync<IEnumerable<DestinationDto>>().Result;

            ViewModel.UnrelatedDestinations = UnrelatedDestinations;

            return View(ViewModel);
        }

        // POST: Trip/Associate/{tripid}
        [HttpPost]
        public ActionResult Associate(int id, int DestinationID)
        {
            //call our api to associate trip with destination
            string url = "tripdata/associatetripwithdestination/" + id + "/" + DestinationID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        // Get: Trip/UnAssociate/{id}?DestinationID={destinationID}
        [HttpGet]
        public ActionResult UnAssociate(int id, int DestinationID)
        {
            //call our api to associate trip with destination
            string url = "tripdata/unassociatetripwithdestination/" + id + "/" + DestinationID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        // Get: Trip/Error
        public ActionResult Error()
        {

            return View();
        }

        // GET: Trip/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Trip/Create
        [HttpPost]
        public ActionResult Create(Trip trip)
        {
            //objective: add a new trip into our system using the API
            //curl -H "Content-Type:application/json" -d @trip.json https://localhost:44399/api/tripdata/addtrip
            string url = "tripdata/addtrip";

            string jsonpayload = jss.Serialize(trip);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Trip/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateTrip ViewModel = new UpdateTrip();

            //the existing trip information
            string url = "tripdata/findtrip/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TripDto SelectedTrip = response.Content.ReadAsAsync<TripDto>().Result;
            ViewModel.SelectedTrip = SelectedTrip;

            return View(ViewModel);
        }

        // POST: Trip/Update/5
        [HttpPost]
        public ActionResult Update(int id, Trip trip)
        {

            string url = "tripdata/updatetrip/" + id;
            string jsonpayload = jss.Serialize(trip);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Trip/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "tripdata/findtrip/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TripDto selectedtrip = response.Content.ReadAsAsync<TripDto>().Result;
            return View(selectedtrip);
        }

        // POST: Trip/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "tripdata/deletetrip/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
