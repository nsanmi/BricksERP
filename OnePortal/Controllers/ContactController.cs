using HRM.DAL.IService;
using HRM.DAL.Models;
using OnePortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnePortal.Controllers
{
    public class ContactController : ApiController
    {
        IGuestService _guestService;
        IReservationService _reservationService;
        IRoomService _roomService;
        ILookupService _lookupService;
        IEmployeeService _employeeService;



        public ContactController(IGuestService guestService, ILookupService lookupService, IEmployeeService employeeService, IRoomService roomService, IReservationService reservationService)
        {
            _guestService = guestService;
            _roomService = roomService;
            _reservationService = reservationService;
            _lookupService = lookupService;
            _employeeService = employeeService;
        }



        // GET: api/Contact
        //[HttpGet]
        //public IEnumerable<ws_lookup> Get()
        //{
        //    //var guests = _guestService.GetGuests();
        //    //return guests;
        //    var look = _lookupService.GetAllLookup();
        //    return look;
        //}

        //GET: api/Contact
       [HttpGet]
        public IEnumerable<Contact> Get()
        {
            Contact[] contacts = new Contact[]
           {
            new Contact() { id = 1, FirstName = "stephen", LastName = "ademola"},
            new Contact() { id = 2, FirstName = "oluwasanmi", LastName = "Ogedengbe"},
            new Contact() { id = 3, FirstName = "Colins", LastName = "Uche"},
            new Contact() { id = 3, FirstName = "Chima", LastName = "femi"}
           };

            return contacts;
        }

        [Route("api/Contact/id:int")]
        [HttpGet]
        public string FindContact(int id)
        {
            return "value";
        }

        // GET: api/Contact/5
        //[HttpGet("people/all")]
        public string Get(int id)
        {
            return "value";
        }

        //public HttpResponseMessage <Contact> Get()
        //{
        //    return Ok();
        //}

        // POST: api/Contact
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Contact/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Contact/5
        public void Delete(int id)
        {
        }
    }
}
