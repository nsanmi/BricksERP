using Hotel.Dal.IService;
using Hotel.Dal.Models;
using HRM.DAL.IService;
using Microsoft.AspNet.Identity;
using OnePortal.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnePortal.Controllers
{
    [Authorize]
    public class HotelBookingController : Controller
    {
        private HotelDBEntities HotelDB;

        IGuestBookingService _guestBookingService;
        ILookupService _lookupService;
        IEmployeeService _employeeService;
        IAmenityService _amenityService;
        IAddonService _addonService;
        IBookingService _bookingService;



        public HotelBookingController(IGuestBookingService guestbookingService, IAmenityService amenityService, IAddonService addonService, IBookingService bookingService, ILookupService lookupService, IEmployeeService employeeService)
        {
            _guestBookingService = guestbookingService;
            _lookupService = lookupService;
            _employeeService = employeeService;
            _amenityService = amenityService;
            _addonService = addonService;
            _bookingService = bookingService;
        }


        public ActionResult Test()
        { 

            return View();
        }


        public ActionResult RoomIndex()
        {
            HotelDB = new HotelDBEntities();
            RoomVModel objOfRoomViewModel = new RoomVModel();

            objOfRoomViewModel.ListOfRoomType = (from obj in HotelDB.Hotel_Room_Type
            select new SelectListItem()
            { Text = obj.RoomType,
                Value = obj.RoomTypeId.ToString()
                  }).ToList();


            return View(objOfRoomViewModel);
        }


        public ActionResult Reservation()
        {
            HotelDB = new HotelDBEntities();
            ReservationVModel objOfReservationViewModel = new ReservationVModel();

            objOfReservationViewModel.ListOfGuest = (from obj in HotelDB.Hotel_guest
                                                 select new SelectListItem()
                                                 {
                                                     Text = obj.First_name,
                                                     Value = obj.Id.ToString()
                                                 }).ToList();


            objOfReservationViewModel.ListOfPaymentType = (from obj in HotelDB.Hotel_PaymentOption
                                                     select new SelectListItem()
                                                     {
                                                         Text = obj.Options,
                                                         Value = obj.Id.ToString()
                                                     }).ToList();

            objOfReservationViewModel.ListOfRoom = (from obj in HotelDB.Hotel_Room
                                                           select new SelectListItem()
                                                           {
                                                               Text = obj.Room_number,
                                                               Value = obj.Id.ToString()
                                                           }).ToList();

            objOfReservationViewModel.ListOfReservationType = (from obj in HotelDB.Hotel_ReservationType
                                                    select new SelectListItem()
                                                    {
                                                        Text = obj.Type_name,
                                                        Value = obj.Id.ToString()
                                                    }).ToList();



            return View();
        }

        // GET: Hotel
        public ActionResult Index(int? page, string search = null)
        {
            HotelDB = new HotelDBEntities();
            ReservationVModel objOfReservationViewModel = new ReservationVModel();

            objOfReservationViewModel.ListOfGuest = (from obj in HotelDB.Hotel_guest
                                                     select new SelectListItem()
                                                     {
                                                         Text = obj.First_name,
                                                         Value = obj.Id.ToString()
                                                     }).ToList();


            objOfReservationViewModel.ListOfPaymentType = (from obj in HotelDB.Hotel_PaymentOption
                                                           select new SelectListItem()
                                                           {
                                                               Text = obj.Options,
                                                               Value = obj.Id.ToString()
                                                           }).ToList();

            objOfReservationViewModel.ListOfRoom = (from obj in HotelDB.Hotel_Room
                                                    select new SelectListItem()
                                                    {
                                                        Text = obj.Room_number,
                                                        Value = obj.Id.ToString()
                                                    }).ToList();

            objOfReservationViewModel.ListOfReservationType = (from obj in HotelDB.Hotel_ReservationType
                                                               select new SelectListItem()
                                                               {
                                                                   Text = obj.Type_name,
                                                                   Value = obj.Id.ToString()
                                                               }).ToList();


            ViewBag.message = TempData["message"];
            ViewBag.alert_type = TempData["alert_type"];
            ViewBag.requestData = _bookingService.GetAllRoom();

            return View();

            //int pageIndex = 1;
            //int pageSize = 15;
            //pageIndex = page.HasValue ? page.Value : 1;
            //if (search != null)
            //{
            //    //ViewBag.state = _bookingService.GetComplain(complainId);

            //    ViewBag.search = search;
            //    return View(_bookingService.GetAllRoom().Where(m => m.status == 0).Where(e => e.Room_number.Contains(search) || e.Room_number.Contains(search)).OrderBy(e => e.room_type).ToPagedList(pageIndex, pageSize));
            //}
            //return View(_bookingService.GetAllRoom().Where(e => e.status == 0).OrderByDescending(e => e.created_date).ToPagedList(pageIndex, pageSize));

        }

        public ActionResult RoomIndexx()
        {
            return View();
        }

        public ActionResult AddRoom(RoomVModel room)
        {
            var user_id = User.Identity.GetUserId();
          
                var roomobj = new Hotel_Room
                {
                    Room_number = room.RoomNumber,
                    room_category = room.RoomCategory,
                    room_type = room.RoomType,
                    RoomImage1 = room.RoomImage,
                    Capacity = room.Capacity,
                    created_date = DateTime.Now,
                    

                    //Priority = comp.Priority,
                    //Type = comp.Type,
                    //UserId = user_id,
                    //Comment = comp.Comment,
                    //CreateDate = DateTime.Now,
                    //Deleted = 0,
                    //Resolved = "No"
                };

            roomobj.Id = _bookingService.AddRoom(roomobj);
            

            return RedirectToAction("Index");
        }

        public ActionResult UpdateRoom()
        {
            return View();
        }

        public ActionResult DeleteRoom()
        {
            return View();
        }


        public ActionResult BookingIndex()
        {
            HotelDB = new HotelDBEntities();
            ReservationVModel objOfReservationViewModel = new ReservationVModel();

            objOfReservationViewModel.ListOfGuest = (from obj in HotelDB.Hotel_guest
                                                     select new SelectListItem()
                                                     {
                                                         Text = obj.First_name,
                                                         Value = obj.Id.ToString()
                                                     }).ToList();


            objOfReservationViewModel.ListOfPaymentType = (from obj in HotelDB.Hotel_PaymentOption
                                                           select new SelectListItem()
                                                           {
                                                               Text = obj.Options,
                                                               Value = obj.Id.ToString()
                                                           }).ToList();

            objOfReservationViewModel.ListOfRoom = (from obj in HotelDB.Hotel_Room
                                                    select new SelectListItem()
                                                    {
                                                        Text = obj.Room_number,
                                                        Value = obj.Id.ToString()
                                                    }).ToList();

            objOfReservationViewModel.ListOfReservationType = (from obj in HotelDB.Hotel_ReservationType
                                                               select new SelectListItem()
                                                               {
                                                                   Text = obj.Type_name,
                                                                   Value = obj.Id.ToString()
                                                               }).ToList();

            return View();
        }

        public ActionResult IndexBooking()
        {
            return View();
        }


        public ActionResult MakeBooking()
        {
            return View();
        }

        public ActionResult UpdateBooking()
        {
            return View();
        }

        public ActionResult DeleteBooking()
        {
            return View();
        }



        public ActionResult IndexGuest()
        {
            return View();
        }

        public ActionResult AddGuest()
        {
            return View();
        }

        public ActionResult UpdateGuest()
        {
            return View();
        }

        public ActionResult DeleteGuest()
        {
            return View();
        }

        public ActionResult BookingAudit()
        {
            return View();
        }

        public ActionResult IndexAmenity()
        {
            return View();
        }

        public ActionResult AddAmenity()
        {
            return View();
        }

        public ActionResult UpdateAmenity()
        {
            return View();
        }

        public ActionResult DeleteAmenity()
        {
            return View();
        }

        public ActionResult IndexHotel()
        {
            return View();
        }

        public ActionResult AddHotel()
        {
            return View();
        }

        public ActionResult UpdateHotel()
        {
            return View();
        }

        public ActionResult DeleteHotel()
        {
            return View();
        }

        public ActionResult IndexAddon()
        {
            return View();
        }

        public ActionResult AddAddon()
        {
            return View();
        }

        public ActionResult UpdateAddon()
        {
            return View();
        }

        public ActionResult DeleteAddon()
        {
            return View();
        }
    }
}