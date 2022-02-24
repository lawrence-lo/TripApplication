using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TripApplication.Models;
using System.Diagnostics;

namespace TripApplication.Controllers
{
    public class TripDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all trips in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all trips in the database
        /// </returns>
        /// <example>
        /// GET: api/TripData/ListTrips
        /// </example>
        [HttpGet]
        [ResponseType(typeof(TripDto))]
        public IHttpActionResult ListTrips()
        {
            List<Trip> Trips = db.Trips.OrderByDescending(s => s.TripFromDate).ToList();
            List<TripDto> TripDtos = new List<TripDto>();

            Trips.ForEach(a => TripDtos.Add(new TripDto()
            {
                TripID = a.TripID,
                TripName = a.TripName,
                TripFromDate = a.TripFromDate,
                TripToDate = a.TripToDate,
                TripRemarks = a.TripRemarks
            }));

            return Ok(TripDtos);
        }

        /// <summary>
        /// Gathers information about all trips related to a particular destination
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all trips in the database, including their associated destination matched with a particular destination ID
        /// </returns>
        /// <param name="id">Destination ID.</param>
        /// <example>
        /// GET: api/TripData/ListTripsForDestination/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(TripDto))]
        public IHttpActionResult ListTripsForDestination(int id)
        {
            //all trips that have destinations which match with our ID
            List<Trip> Trips = db.Trips.Where(
                a => a.Destinations.Any(
                    k => k.DestinationID == id
                )).OrderByDescending(s => s.TripFromDate).ToList();
            List<TripDto> TripDtos = new List<TripDto>();

            Trips.ForEach(a => TripDtos.Add(new TripDto()
            {
                TripID = a.TripID,
                TripName = a.TripName,
                TripFromDate = a.TripFromDate,
                TripToDate = a.TripToDate,
                TripRemarks = a.TripRemarks
            }));

            return Ok(TripDtos);
        }

        /// <summary>
        /// Associates a particular destination with a particular trip
        /// </summary>
        /// <param name="tripid">The trip ID primary key</param>
        /// <param name="destinationid">The destination ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/TripData/AssociateTripWithDestination/9/1
        /// </example>
        [HttpPost]
        [Route("api/TripData/AssociateTripWithDestination/{tripid}/{destinationid}")]
        public IHttpActionResult AssociateTripWithDestination(int tripid, int destinationid)
        {
            Trip SelectedTrip = db.Trips.Include(a => a.Destinations).Where(a => a.TripID == tripid).FirstOrDefault();
            Destination SelectedDestination = db.Destinations.Find(destinationid);

            if (SelectedTrip == null || SelectedDestination == null)
            {
                return NotFound();
            }

            SelectedTrip.Destinations.Add(SelectedDestination);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Removes an association between a particular destination and a particular trip
        /// </summary>
        /// <param name="tripid">The trip ID primary key</param>
        /// <param name="destinationid">The destination ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/TripData/UnassociateTripWithDestination/9/1
        /// </example>
        [HttpPost]
        [Route("api/TripData/UnassociateTripWithDestination/{tripid}/{destinationid}")]
        public IHttpActionResult UnAssociateTripWithDestination(int tripid, int destinationid)
        {
            Trip SelectedTrip = db.Trips.Include(a => a.Destinations).Where(a => a.TripID == tripid).FirstOrDefault();
            Destination SelectedDestination = db.Destinations.Find(destinationid);

            if (SelectedTrip == null || SelectedDestination == null)
            {
                return NotFound();
            }

            SelectedTrip.Destinations.Remove(SelectedDestination);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Returns a trip in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A trip in the system matching up to the trip ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the trip</param>
        /// <example>
        /// GET: api/TripData/FindTrip/5
        /// </example>
        [ResponseType(typeof(TripDto))]
        [HttpGet]
        public IHttpActionResult FindTrip(int id)
        {
            Trip Trip = db.Trips.Find(id);
            TripDto TripDto = new TripDto()
            {
                TripID = Trip.TripID,
                TripName = Trip.TripName,
                TripFromDate = Trip.TripFromDate,
                TripToDate = Trip.TripToDate,
                TripRemarks = Trip.TripRemarks,
                TripHasPic = Trip.TripHasPic,
                PicExtension = Trip.PicExtension
            };
            if (Trip == null)
            {
                return NotFound();
            }

            return Ok(TripDto);
        }

        /// <summary>
        /// Updates a particular trip in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Trip ID primary key</param>
        /// <param name="trip">JSON FORM DATA of an trip</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/TripData/UpdateTrip/5
        /// FORM DATA: Trip JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateTrip(int id, Trip trip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != trip.TripID)
            {

                return BadRequest();
            }

            db.Entry(trip).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        
        /// <summary>
        /// Receives trip picture data, uploads it to the webserver and updates the trip's HasPic option
        /// </summary>
        /// <param name="id">the trip id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F trippic=@file.jpg "https://localhost:xx/api/tripdata/uploadtrippic/2"
        /// POST: api/tripData/UpdatetripPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UploadTripPic(int id)
        {

            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var tripPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (tripPic.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(tripPic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/trips/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Trips/"), fn);

                                //save the file
                                tripPic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the animal haspic and picextension fields in the database
                                Trip Selectedtrip = db.Trips.Find(id);
                                Selectedtrip.TripHasPic = haspic;
                                Selectedtrip.PicExtension = extension;
                                db.Entry(Selectedtrip).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("trip Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }

                }

                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();

            }

        }

        /// <summary>
        /// Adds a trip to the system
        /// </summary>
        /// <param name="trip">JSON FORM DATA of an trip</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Trip ID, Trip Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/TripData/AddTrip
        /// FORM DATA: Animal JSON Object
        /// </example>
        [ResponseType(typeof(Trip))]
        [HttpPost]
        public IHttpActionResult AddTrip(Trip trip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Trips.Add(trip);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = trip.TripID }, trip);
        }

        /// <summary>
        /// Deletes a trip from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the trip</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/TripData/DeleteTrip/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Trip))]
        [HttpPost]
        public IHttpActionResult DeleteTrip(int id)
        {
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return NotFound();
            }

            db.Trips.Remove(trip);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TripExists(int id)
        {
            return db.Trips.Count(e => e.TripID == id) > 0;
        }
    }
}