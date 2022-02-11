﻿using System;
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
    public class DestinationController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static DestinationController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44399/api/");
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
        public ActionResult New()
        {
            return View();
        }

        // POST: Destination/Create
        [HttpPost]
        public ActionResult Create(Destination destination)
        {
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
        public ActionResult Update(int id, Destination destination)
        {
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
        public ActionResult DeleteConfirm(int id)
        {
            string url = "destinationdata/finddestination/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DestinationDto selecteddestination = response.Content.ReadAsAsync<DestinationDto>().Result;
            return View(selecteddestination);
        }

        // POST: Destination/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
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
