using System;
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

namespace TripApplication.Controllers
{
    public class DestinationDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all destinations in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all destinations in the database.
        /// </returns>
        /// <example>
        /// GET: api/DestinationData/ListDestinations
        /// </example>
        [HttpGet]
        [ResponseType(typeof(DestinationDto))]
        public IHttpActionResult ListDestinations()
        {
            List<Destination> Destinations = db.Destinations.ToList();
            List<DestinationDto> DestinationDtos = new List<DestinationDto>();

            Destinations.ForEach(a => DestinationDtos.Add(new DestinationDto()
            {
                DestinationID = a.DestinationID,
                DestinationName = a.DestinationName,
                DestinationCountry = a.DestinationCountry,
                DestinationLatitude = a.DestinationLatitude,
                DestinationLongitude = a.DestinationLongitude
            }));

            return Ok(DestinationDtos);
        }

        /// <summary>
        /// Returns all Destinations in the system associated with a particular trip.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Destinations in the database associated with a particular trip
        /// </returns>
        /// <param name="id">Trip Primary Key</param>
        /// <example>
        /// GET: api/DestinationData/ListDestinationsForTrip/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(DestinationDto))]
        public IHttpActionResult ListDestinationsForTrip(int id)
        {
            List<Destination> Destinations = db.Destinations.Where(
                k => k.Trips.Any(
                    a => a.TripID == id)
                ).ToList();
            List<DestinationDto> DestinationDtos = new List<DestinationDto>();

            Destinations.ForEach(k => DestinationDtos.Add(new DestinationDto()
            {
                DestinationID = k.DestinationID,
                DestinationName = k.DestinationName,
                DestinationCountry = k.DestinationCountry,
                DestinationLatitude = k.DestinationLatitude,
                DestinationLongitude = k.DestinationLongitude
            }));

            return Ok(DestinationDtos);
        }

        /// <summary>
        /// Returns all Destinations in the system associated with any trip.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Destinations in the database associated with any trip
        /// </returns>
        /// <example>
        /// GET: api/DestinationData/ListDestinationsInTrips/
        /// </example>
        [HttpGet]
        [ResponseType(typeof(DestinationDto))]
        public IHttpActionResult ListDestinationsInTrips()
        {
            List<Destination> Destinations = db.Destinations.Where(
                k => k.Trips.Any()
                ).ToList();
            List<DestinationDto> DestinationDtos = new List<DestinationDto>();

            Destinations.ForEach(k => DestinationDtos.Add(new DestinationDto()
            {
                DestinationID = k.DestinationID,
                DestinationName = k.DestinationName,
                DestinationCountry = k.DestinationCountry,
                DestinationLatitude = k.DestinationLatitude,
                DestinationLongitude = k.DestinationLongitude
            }));

            return Ok(DestinationDtos);
        }

        /// <summary>
        /// Returns Destinations in the system not related to a particular trip.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Destinations in the database not related to a particular trip
        /// </returns>
        /// <param name="id">Trip Primary Key</param>
        /// <example>
        /// GET: api/DestinationData/ListDestinationsNotRelatedToTrip/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(DestinationDto))]
        public IHttpActionResult ListDestinationsNotRelatedToTrip(int id)
        {
            List<Destination> Destinations = db.Destinations.Where(
                k => !k.Trips.Any(
                    a => a.TripID == id)
                ).ToList();
            List<DestinationDto> DestinationDtos = new List<DestinationDto>();

            Destinations.ForEach(k => DestinationDtos.Add(new DestinationDto()
            {
                DestinationID = k.DestinationID,
                DestinationName = k.DestinationName,
                DestinationCountry = k.DestinationCountry,
                DestinationLatitude = k.DestinationLatitude,
                DestinationLongitude = k.DestinationLongitude
            }));

            return Ok(DestinationDtos);
        }

        /// <summary>
        /// Returns a destination in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A destination in the system matching up to the destination ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the destination</param>
        /// <example>
        /// GET: api/DestinationData/FindDestination/5
        /// </example>
        [ResponseType(typeof(Destination))]
        [HttpGet]
        public IHttpActionResult FindDestination(int id)
        {
            Destination Destination = db.Destinations.Find(id);
            DestinationDto DestinationDto = new DestinationDto()
            {
                DestinationID = Destination.DestinationID,
                DestinationName = Destination.DestinationName,
                DestinationCountry = Destination.DestinationCountry,
                DestinationLatitude = Destination.DestinationLatitude,
                DestinationLongitude = Destination.DestinationLongitude
            };
            if (Destination == null)
            {
                return NotFound();
            }

            return Ok(DestinationDto);
        }

        /// <summary>
        /// Updates a particular destination in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Destination ID primary key</param>
        /// <param name="destination">JSON FORM DATA of a destination</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/DestinationData/UpdateDestination/5
        /// FORM DATA: Destination JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateDestination(int id, Destination destination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != destination.DestinationID)
            {
                return BadRequest();
            }

            db.Entry(destination).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DestinationExists(id))
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
        /// Adds a destination to the system
        /// </summary>
        /// <param name="destination">JSON FORM DATA of a destination</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Destination ID, Destination Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/DestinationData/AddDestination
        /// FORM DATA: Destination JSON Object
        /// </example>
        [ResponseType(typeof(Destination))]
        [HttpPost]
        public IHttpActionResult AddDestination(Destination destination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Destinations.Add(destination);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = destination.DestinationID }, destination);
        }

        /// <summary>
        /// Deletes a destination from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the destination</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/DestinationData/DeleteDestination/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Destination))]
        [HttpPost]
        public IHttpActionResult DeleteDestination(int id)
        {
            Destination destination = db.Destinations.Find(id);
            if (destination == null)
            {
                return NotFound();
            }

            db.Destinations.Remove(destination);
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

        private bool DestinationExists(int id)
        {
            return db.Destinations.Count(e => e.DestinationID == id) > 0;
        }
    }
}