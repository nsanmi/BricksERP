using HRM.DAL.IService;
using HRM.DAL.Models;
using HRM.DAL.Service;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnePortal.Helper;
using OnePortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnePortal.Controllers
{
    public class BookingApiController : ApiController
    {
        IReservationService _reservationService;
        IGuestService _guestService;
        IRoomService _roomService;
        ILookupService _lookupService;
        IEmployeeService _employeeService;

      

        public BookingApiController(IGuestService guestService, ILookupService lookupService, IEmployeeService employeeService, IRoomService roomService, IReservationService reservationService)
        {
            _guestService = guestService;
            _roomService = roomService;
            _reservationService = reservationService;
            _lookupService = lookupService;
            _employeeService = employeeService;
        }




        //[HttpGet]
       
        //public Guest  GetGuest(Guest guest)
        //{
        //    var guests = _guestService.GetGuests();

        //    //if (!ModelState.IsValid)
        //    //    return BadRequest(ModelState);


        //    foreach (var gues in guests)
        //    {
        //        booksDto.Add(new BookDto
        //        {
        //            Id = book.Id,
        //            Title = book.Title,
        //            Isbn = book.Isbn,
        //            DatePublished = book.DatePublished
        //        });
        //    }

        //    return Ok(booksDto);
        //}


        // GET: api/BookingApi
        public IEnumerable<ws_lookup> Get()
        {
            //var guests = _guestService.GetGuests();
            //return guests;
            var look = _lookupService.GetAllLookup();
            return look;
        }

        // GET: api/BookingApi/5
        [Route("api/Booking")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var guest = _guestService.GetGuests();

            //return "value";
            return Ok(guest);
        }

        // GET: api/BookingApi/5
        //public string Get(int id)
        //{
        //    var name = "oluwasanmi dimeji";
        //    //return "value";
        //    return name;
        //}
        [Route("api/PostNewBooking")]
        [HttpPost]
        public IHttpActionResult PostNewBooking(Guest guest, Room room, RoomReservation roomreserve, RoomRate rate)
        {
            var user_id = User.Identity.GetUserId();

            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            /*
             * 
             * To Check if an Guest exists before creating
             * This check is done by email comparism
             */
            var existing_guest = _guestService.GetGuest(guest.Email);
            var deactivated_guest = _guestService.GetDeactivatedGuest(guest.Email);
            if (existing_guest != null && deactivated_guest != null)
            {
                var message = "The Guest has already been added or was deactivated, Please check and make sure that the email address is correct and has not been added before";
                return Ok(message);
            }
            //var user_id = guest.Employee_EmployeeNumber;
            var regemp = UserHelper.GetEmployeeEmail(user_id);
            try
            {
                var newguest = new Guest
            {
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                Active = 1,
                Deleted = 0,
                Address = guest.Address,
                Employee_EmployeeNumber = 1000,
                user_id = regemp


                };
            newguest.GuestID = _guestService.AddGuest(newguest);

                //var newreserve = new RoomReservation
                //{
                //    Room_RoomID = room.RoomID,
                //    StartDate = roomreserve.StartDate,
                //    EndDate = roomreserve.EndDate,
                //    CheckinTime = roomreserve.CheckinTime,
                //    CheckoutTime = roomreserve.CheckoutTime,
                //    Guest_id = guest.GuestID
                   

                //};




                var message = "Guest account creaded sucessfully";

            }
            catch (Exception e)
            {
                var Errormessage = "Guest account wasn't creaded sucessfully";
                return NotFound();
            }




            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");



           
            return Ok();
        }



        public IHttpActionResult PostNewGuest(Guest guest)
        {
            var existing_guest = _guestService.GetGuest(guest.Email);
            var deactivated_guest = _guestService.GetDeactivatedGuest(guest.Email);
            if (existing_guest != null && deactivated_guest != null)
            {
                var message = "The Guest has already been added or was deactivated, Please check and make sure that the email address is correct and has not been added before";
                return Ok(message);
            }
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            try
            {
                var newguest = new Guest
                {
                    FirstName = guest.FirstName,
                    LastName = guest.LastName,
                    Active = 1,
                    Deleted = 0,
                    Address = guest.Address
                   
                };

                newguest.GuestID = _guestService.AddGuest(newguest);

                var message = "Guest account creaded sucessfully";
                
            }
            catch (Exception e)
            {
                var Errormessage = "Guest account wasn't creaded sucessfully";
            }


            return Ok();
        }

        // POST: api/BookingApi
        public void Post(Guest guest, Room room, RoomReservation roomreserve, RoomRate rate)
        {
            var user_id = User.Identity.GetUserId();

            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            /*
             * 
             * To Check if an Guest exists before creating
             * This check is done by email comparism
             */
            var existing_guest = _guestService.GetGuest(guest.Email);
            var deactivated_guest = _guestService.GetDeactivatedGuest(guest.Email);
            if (existing_guest != null && deactivated_guest != null)
            {
                var message = "The Guest has already been added or was deactivated, Please check and make sure that the email address is correct and has not been added before";
                //return Ok(message);
            }
            //var user_id = guest.Employee_EmployeeNumber;
            var regemp = UserHelper.GetEmployeeEmail(user_id);
            try
            {
                var newguest = new Guest
                {
                    FirstName = guest.FirstName,
                    LastName = guest.LastName,
                    Active = 1,
                    Deleted = 0,
                    Address = guest.Address,
                    Employee_EmployeeNumber = 1000,
                    user_id = regemp


                };
                newguest.GuestID = _guestService.AddGuest(newguest);

                //var newreserve = new RoomReservation
                //{
                //    Room_RoomID = room.RoomID,
                //    StartDate = roomreserve.StartDate,
                //    EndDate = roomreserve.EndDate,
                //    CheckinTime = roomreserve.CheckinTime,
                //    CheckoutTime = roomreserve.CheckoutTime,
                //    Guest_id = guest.GuestID


                //};




                var message = "Guest account creaded sucessfully";

            }
            catch (Exception e)
            {
                var Errormessage = "Guest account wasn't creaded sucessfully";
               // return NotFound();
            }




            //if (!ModelState.IsValid)
            //   return BadRequest("Invalid data.");




            //return Ok();
        }


        // PUT: api/BookingApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/BookingApi/5
        public void Delete(int id)
        {
        }
    }
}
