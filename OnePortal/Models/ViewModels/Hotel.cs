using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

namespace OnePortal.Models.ViewModels
{
    public class Hotel
    {
    }

    public class ImageVModel
    {

        public HttpPostedFileBase ImageFile2 { get; set; }
        public string Room_number { get; set; }
        public int room_typeid { get; set; }

        public int max_adults { get; set; }
        public int max_children { get; set; }
        public int max_people { get; set; }

        public int min_people { get; set; }


        //public float rate { get; set; }

        public float rate { get; set; }

        public DateTime created_date { get; set; }
        public DateTime updated_date { get; set; }
        public int Capacity { get; set; }
        public DateTime reservation_date { get; set; }

        public string RoomImage1 { get; set; }
        public string RoomImage2 { get; set; }
        public string RoomImage3 { get; set; }
        public string RoomImage4 { get; set; }
        public string room_category { get; set; }
        public string Comment { get; set; }
        public string Room_Description { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }


    }



    public class LaundryViewModel
    {
        public string Room_number { get; set; }

        public string First_name  { get;  set;}
        public string Last_name{get; set; }

        public int Id { get; set; }
      
        //public int Quantity { get; set; }

        public string Description { get; set; }
        public string Emp_name { get; set; }

        public string Personel { get; set; }

        public DateTime reservation_checking_date { get; set; }
        public DateTime reservation_checkout_date { get; set; }
        public DateTime collection_date { get; set; }

        public DateTime delivery_date { get; set; }
        public float Grandprice { get; set; }
        public int GrandQuantity { get; set; }
        public List <float> price { get; set; }
        public List<int> quantity { get; set; }
        public List<int> CollectionId { get; set; }
        public List<int> totalquantity { get; set; }
        public List  <float> totalprice { get; set; }
    

        public List<string> name { get; set; }
        public List<string> Room_numberr { get; set; }
        public List<SelectListItem> ListOfGuest { get; set; }
        public List<SelectListItem> ListOfRoom { get; set; }

    }

    public class LaundryVModel
    {

        public int Room_number { get; set; }
        public string First_name
        {
            get;
            set;
        }
        public string Last_name
        {
            get;
            set;
        }

       
    }


    public class ReservationVModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public string Nextofkinfullname { get; set; }
        public string NextofkinPhone { get; set; }
        public string Phone { get; set; }

        public int RoomNumber { get; set; }
        public int Room_id { get; set; }
        public float Rate { get; set; }
        public int reservation_status { get; set; } 
        public DateTime reservation_checking_date { get; set; }
        public DateTime reservation_checkout_date { get; set; }
        public int deleted { get; set; }
        public DateTime reservation_date { get; set; }
        public string reservation_comment { get; set; }
        public string room_type { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
        public int PaymentStatus { get; set; }
        public int max_adult { get; set; }
        public int max_children { get; set; }
        public string Session { get; set; }
        public List<SelectListItem> ListOfGuest { get; set; }
        public List<SelectListItem> ListOfBookingStatus { get; set; }
        public List<SelectListItem> ListOfRoom { get; set; }
        public List<SelectListItem> ListOfReservationType { get; set; }
        public List<SelectListItem> ListOfPaymentType { get; set; }

    }

    public class ConfirmationVModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public string Nextofkinfullname { get; set; }
        public string NextofkinPhone { get; set; }
        public string Phone { get; set; }

        public int RoomNumber { get; set; }
        public int Room_id { get; set; }
        public float Rate { get; set; }
        public int reservation_status { get; set; }
        public DateTime reservation_checking_date { get; set; }
        public DateTime reservation_checkout_date { get; set; }
        public int deleted { get; set; }
        public DateTime reservation_date { get; set; }
        public string reservation_comment { get; set; }
        public string room_type { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
        public int PaymentStatus { get; set; }
        public int max_adult { get; set; }
        public int max_children { get; set; }
        public string Session { get; set; }
       

    }




    public class RoomVModel
    {
        public string Title { get; set; }

        public int Status { get; set; }



            public int Id { get; set; }
            public string RoomNumber { get; set; }
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