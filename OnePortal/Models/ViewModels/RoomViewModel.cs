using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnePortal.Models.ViewModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public string  RoomNumber { get; set; }
        public int HoteiId { get; set; }
        public int LocationId { get; set; }
        public string RoomType { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomCategory { get; set; }
        public int status { get; set; }
        public float Rate { get; set; }
        public int PromoId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public string RoomImage { get; set; }
        public HttpPostedFileBase image { get; set; }

        public List<SelectListItem> ListOfRoomType { get; set; }
        public List<SelectListItem> ListOfBookingStatus { get; set; }



    }
}