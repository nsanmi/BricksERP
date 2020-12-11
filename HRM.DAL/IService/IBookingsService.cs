using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.IService
{
   public interface IBookingsService
    {

        int AddGuest(Guest guests);
        Guest GetGuest(int guestId);
        IQueryable<Guest> GetAllGuest();
        void UpdateGuest(Guest guests);
        int DeleteGuest(int id);
        void DeactivateGuest(int guest_number);
        Guest GetDeactivatedGuest(string email);



        //int AddBooking(booking book);
        //booking GetBooking(int bookingId);
        //IQueryable<booking> GetAllBookings();
        //void UpdateBooking(booking book);

        //int DeleteBooking(int id);
        //int CompletedBooking(int id, string userId);
        //List<string> GetBookingAdmin();
        //List<string> CompletedBookingNotification();

    }


}

