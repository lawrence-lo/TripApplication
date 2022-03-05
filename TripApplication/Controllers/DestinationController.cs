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
    public class DestinationController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static DestinationController()
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

        // GET: Destination/List
        public ActionResult List()
        {
            //objective: communicate with our destination data api to retrieve a list of destinations
            //curl https://localhost:44399/api/destinationdata/listdestinations

            string url = "destinationdata/listdestinations";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<DestinationDto> destinations = response.Content.ReadAsAsync<IEnumerable<DestinationDto>>().Result;

            return View(destinations);
        }

        // GET: Destination/Details/5
        public ActionResult Details(int id)
        {
            DetailsDestination ViewModel = new DetailsDestination();

            //objective: communicate with our destination data api to retrieve one destination
            //curl https://localhost:44399/api/destinationdata/finddestination/{id}

            string url = "destinationdata/finddestination/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DestinationDto SelectedDestination = response.Content.ReadAsAsync<DestinationDto>().Result;

            ViewModel.SelectedDestination = SelectedDestination;

            //show associated trips with this destination
            url = "tripdata/listtripsfordestination/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<TripDto> RelatedTrips = response.Content.ReadAsAsync<IEnumerable<TripDto>>().Result;

            ViewModel.RelatedTrips = RelatedTrips;

            return View(ViewModel);
        }

        // GET: Destination/Error
        public ActionResult Error()
        {
            return View();
        }

        // GET: Destination/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Destination/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Destination destination)
        {
            GetApplicationCookie();//get token credentials
            //objective: add a new destination into our system using the API
            //curl -H "Content-Type:application/json" -d @destination.json https://localhost:44399/api/destinationdata/adddestination
            string url = "destinationdata/adddestination";

            string jsonpayload = jss.Serialize(destination);

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

        // GET: Destination/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            UpdateDestination ViewModel = new UpdateDestination();

            //the existing destination information
            string url = "destinationdata/finddestination/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DestinationDto SelectedDestination = response.Content.ReadAsAsync<DestinationDto>().Result;
            ViewModel.SelectedDestination = SelectedDestination;

            return View(ViewModel);
        }

        // POST: Destination/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Destination destination)
        {
            GetApplicationCookie();//get token credentials
            string url = "destinationdata/updatedestination/" + id;
            string jsonpayload = jss.Serialize(destination);
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

        // GET: Destination/DeleteConfirm/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "destinationdata/finddestination/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DestinationDto selecteddestination = response.Content.ReadAsAsync<DestinationDto>().Result;
            return View(selecteddestination);
        }

        // POST: Destination/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            GetApplicationCookie();//get token credentials
            string url = "destinationdata/deletedestination/" + id;
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
