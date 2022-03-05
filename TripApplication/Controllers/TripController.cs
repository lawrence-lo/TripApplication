using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
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
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44399/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";

            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
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
        [Authorize]
        public ActionResult Associate(int id, int DestinationID)
        {
            GetApplicationCookie();//get token credentials
            //call our api to associate trip with destination
            string url = "tripdata/associatetripwithdestination/" + id + "/" + DestinationID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        // Get: Trip/UnAssociate/{id}?DestinationID={destinationID}
        [HttpGet]
        [Authorize]
        public ActionResult UnAssociate(int id, int DestinationID)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Trip/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Trip trip)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
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
        [Authorize]
        public ActionResult Update(int id, Trip trip, HttpPostedFileBase TripPic)
        {
            GetApplicationCookie();//get token credentials
            string url = "tripdata/updatetrip/" + id;
            string jsonpayload = jss.Serialize(trip);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //update request is successful, and we have image data
            if (response.IsSuccessStatusCode && TripPic != null)
            {
                //Updating the trip picture as a separate request
                Debug.WriteLine("Calling Update Image method.");
                //Send over image data for player
                url = "TripData/UploadTripPic/" + id;
                //Debug.WriteLine("Received Trip Picture "+TripPic.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(TripPic.InputStream);
                requestcontent.Add(imagecontent, "TripPic", TripPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;

                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                //No image upload, but update still successful
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Trip/DeleteConfirm/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "tripdata/findtrip/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TripDto selectedtrip = response.Content.ReadAsAsync<TripDto>().Result;
            return View(selectedtrip);
        }

        // POST: Trip/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
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
