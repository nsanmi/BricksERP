using Hotel.Dal.IService;
using Hotel.Dal.Models;
using HRM.DAL.IService;
using HRM.DAL.Util;
using OnePortal.Helper;
using OnePortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnePortal.Controllers
{
    public class HotelGuestBookingController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();

        IGuestBookingService _guestBookingService;
        ILookupService _lookupService;
        IEmployeeService _employeeService;
        IAmenityService _amenityService;
        IAddonService _addonService;
        IBookingService _bookingService;
        
        

        public HotelGuestBookingController(IGuestBookingService guestbookingService, IAmenityService amenityService, IAddonService addonService, IBookingService bookingService, ILookupService lookupService, IEmployeeService employeeService)
        {
            _guestBookingService = guestbookingService;
            _lookupService = lookupService;
            _employeeService = employeeService;
           _amenityService = amenityService;
            _addonService = addonService;
            _bookingService = bookingService;
            
           
        }


        // GET: HotelBooking
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChooseDate(ReservationVModel reserve)
        {

            /*var guestId; = User.Identity.GetUserId();*/
           
                
                var guestBooking = new Hotel_reservation
                {
                    reservation_date = DateTime.Now,
                    reservation_checking_date = reserve.reservation_checking_date,
                    reservation_checkout_date = reserve.reservation_checkout_date,
                   
                };

            ViewBag.DataState = guestBooking;
            //guestBooking.Id = _guestBookingService.AddBooking(guestBooking);

            
            return View();
        }

        [HttpGet]
        public ActionResult ChooseDate()
        {
            return View();
        }



        public ActionResult Rooms()
        {

            return View();
        }

        public ActionResult ChooseRoom(GetRoom search)
        {
            db.Configuration.ProxyCreationEnabled = false;
            //select all available rooms between dates selected
            //var rooms_available = db.Hotel_Room.Where(n => n.room_typeid == search.room_type || n.Hotel_reservation.Any(m => m.reservation_checking_date <= search.check_in_date || m.reservation_checkout_date >= search.check_out_date));
            //var rooms_availabl = db.Hotel_Room.Where(n =>  n.Hotel_reservation.Any(m => m.reservation_checking_date <= search.checkin || m.reservation_checkout_date >= search.checkout));
            var rooms_available2 = db.Hotel_Room.Where(n => !n.Hotel_reservation.Any(m => m.reservation_checking_date >= search.checkin && m.reservation_checkout_date <= search.checkout));
            var rooms_available3 = db.Hotel_Room.Where(n => n.Hotel_reservation.Any());

            //var tt = rooms_available.ToList();
            var data = rooms_available2;
            var data2 = rooms_available2.ToList();
            ViewBag.realdata = data2;
            ViewBag.realdata2 = Json(data2);

            ViewBag.CheckinDate = search.checkin;
            ViewBag.CheckoutDate = search.checkout;


            //return Json(rooms_available.ToList());
            //return Json(data2);


            return View();
        }


        //[HttpGet]
        public ActionResult Reservation(int Id, DateTime checkindate, DateTime checkoutdate, GetRoom search)
        {

            ViewBag.Room = _bookingService.GetRoom(Id);
            ViewBag.Room_id = Id;
            ViewBag.checkinDate = checkindate;
            ViewBag.checkoutDate = checkoutdate;


            //guestBooking.Id = _guestBookingService.AddBooking(guestBooking);


            return View();
        }

        //[HttpPost]
        //public ActionResult Reservation(ReservationVModel reserve)
        //{
        //    //ViewBag.product = _bookingService.GetRoom(reserve.Id);
        //    //return View();
        //    return RedirectToAction("reservation");
        //}


      
        public ActionResult ConfirmReservation(ReservationVModel reserve)
        {
            //var access = Utils.GenerateSessionToken(3, 3, 3) + "!";
           
            ViewBag.reservation = reserve; 

            return View();
        }

        public ActionResult ConfirmBooking(ReservationVModel reserve)
        {
            //var access = Utils.GenerateSessionToken(3, 3, 3) + "!";

            /*var guestId; = User.Identity.GetUserId();*/
            string userId = "B001";
            try
            {
                var guest = new Hotel_guest
                {
                    First_name = reserve.Firstname,
                    Last_name = reserve.Lastname,
                    Address = reserve.Address,
                    Email = reserve.Email,
                    Password = reserve.Password,
                    Phone_number = reserve.Phone,
                    status = 0,
                    deleted = 0,

                };

                guest.Id = _guestBookingService.AddGuest(guest);

                var guestId = guest.Id;


                var guestBooking = new Hotel_reservation
                {
                    reservation_date = DateTime.Now,
                    reservation_checking_date = reserve.reservation_checking_date,
                    reservation_checkout_date = reserve.reservation_checkout_date,
                    guest_id = guestId,
                    Room_id = reserve.Room_id,

                    reservation_typeId = 0,
                    reservation_comment = reserve.reservation_comment,
                    reservation_status = 0,
                    payment_status = reserve.PaymentStatus,
                    session = "online booking",

                    deleted = 0,

                };

                guestBooking.Id = _guestBookingService.AddBooking(guestBooking);

                //guestBooking.Id = _guestBookingService.AddBooking(guestBooking);



                //send mail to booking admin

                //Get the url of the booking detail
                string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                            (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                Domain += "/HotelBooking/update?guestId=" + guestBooking.Id;
                var email_body = string.Format("A boking of <strong>{0}</strong> for a room has been raised on our website <strong>{1}</strong>.<br/>", guestBooking.reservation_checking_date,
                    guestBooking.reservation_checkout_date);
                email_body += "the description is <br/>";
                email_body += string.Format("<i>{0}</i>.<br/>", guestBooking.reservation_comment);
                email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

                var notification_email = new Email
                {
                    body = email_body,
                    subject = "Booking - Action required - BricksERP"
                };
                notification_email.IsHtml = true;
                notification_email.body = email_body;
#if DEBUG
                //notification_email.to = new List<string>{ UserHelper.GetEmployeeEmail(user_id) };

                notification_email.to = _bookingService.GetBookingAdmin();
#else
                //notification_email.to = _complainService.GetComplainAdmin();
#endif

                NotificationUtil.SendEmailComplain(notification_email, UserHelper.GetEmployeeEmail(userId));

                TempData["message"] =
                    "Your Booking has been saved and email sent to the appropriate persons. <br> A code for updating your booking has been sent to your email and phone. you are required to keep this code safe for any future update on your booking status  Thank You";



                ViewBag.a = "Here is the code for updating your bookng status" + " " /*+ access*/;

            }
            catch (Exception e)
            {
                TempData["message"] = "There was an error completing your booking, Please try again";
                Utils.LogError(e);
            }

            return View();
        }

        public ActionResult RoomSingle()
        {

            return View();
        }

        public ActionResult RoomGallery()
        {

            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }

        public ActionResult CompleteReservation(ConfirmationVModel confirm)
        {
            //var access = Utils.GenerateSessionToken(3, 3, 3) + "!";

            /*var guestId; = User.Identity.GetUserId();*/
            //string userId = "B001";
            //try
            //{
                var guest = new Hotel_guest
                {
                    First_name = confirm.Firstname,
                    Last_name = confirm.Lastname,
                    Address = confirm.Address,
                    Email = confirm.Email,
                    Password = confirm.Password,
                    Phone_number = confirm.Phone,
                    status = 0,
                    deleted = 0,

                };

                guest.Id = _guestBookingService.AddGuest(guest);

                var guestId = guest.Id;


                var guestBooking = new Hotel_reservation
                {
                    reservation_date = DateTime.Now,
                    reservation_checking_date = confirm.reservation_checking_date,
                    reservation_checkout_date = confirm.reservation_checkout_date,
                    guest_id = guestId,
                    Room_id = confirm.Room_id,

                    reservation_typeId = 2,
                    reservation_comment = confirm.reservation_comment,
                    reservation_status = 0,
                    payment_status = confirm.PaymentStatus,
                    total = confirm.Rate,
                    session = "online booking",

                    deleted = 0,

                };

                guestBooking.Id = _guestBookingService.AddBooking(guestBooking);

                //guestBooking.Id = _guestBookingService.AddBooking(guestBooking);



                //send mail to booking admin

                //Get the url of the booking detail
                string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                            (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                Domain += "/HotelBooking/update?guestId=" + guestBooking.Id;
                var email_body = string.Format("A boking of <strong>{0}</strong> for a room has been raised on our website <strong>{1}</strong>.<br/>", guestBooking.reservation_checking_date,
                    guestBooking.reservation_checkout_date);
                email_body += "the description is <br/>";
                email_body += string.Format("<i>{0}</i>.<br/>", guestBooking.reservation_comment);
                email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

                var notification_email = new Email
                {
                    body = email_body,
                    subject = "Booking - Action required - BricksERP"
                };
                notification_email.IsHtml = true;
                notification_email.body = email_body;
#if DEBUG
                //notification_email.to = new List<string>{ UserHelper.GetEmployeeEmail(user_id) };

                notification_email.to = _bookingService.GetBookingAdmin();
#else
                //notification_email.to = _complainService.GetComplainAdmin();
#endif
                var userId = "B2";
                //NotificationUtil.SendEmailComplain(notification_email, UserHelper.GetEmployeeEmail(userId));

                TempData["message"] =
                    "Your Booking has been saved and email sent to the appropriate persons. <br> A code for updating your booking has been sent to your email and phone. you are required to keep this code safe for any future update on your booking status  Thank You";



                ViewBag.a = "Here is the code for updating your bookng status" + " " /*+ access*/;

            //}
            //catch (Exception e)
            //{
            //    TempData["message"] = "There was an error completing your booking, Please try again";
            //    Utils.LogError(e);
            //}

            return View();
        }
    }
}