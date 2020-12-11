using Hotel.Dal.Models;
using OnePortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnePortal.Controllers
{
    public class SampleController : Controller
    {
        private HotelDBEntities HotelDB;

        public SampleController()
        {
            HotelDB = new HotelDBEntities(); 
        }
        // GET: Room
        public ActionResult Index()
        {
            RoomViewModel objOfRoomViewModel = new RoomViewModel();

            objOfRoomViewModel.ListOfRoomType = (from obj in HotelDB.Hotel_Room_Type
          select new SelectListItem()
          {
              Text = obj.RoomType,
            Value = obj.RoomTypeId.ToString()
          }).ToList();


            return View(objOfRoomViewModel);
        }
    }
}