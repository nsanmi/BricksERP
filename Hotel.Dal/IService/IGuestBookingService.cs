using Hotel.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Dal.IService
{
    public interface IGuestBookingService
    {
        int AddGuest(Hotel_guest guest);
        Hotel_guest GetGuestAccount(int guestId);
        IQueryable<Hotel_guest> GetGuests();
        IQueryable<Hotel_guest> GetAllGuest();
        void UpdateGuest(Hotel_guest room);


        int AddBooking(Hotel_reservation reservation);
        int RoomBooking(Hotel_reservation reservation,int Id,  int guestId);
        Hotel_reservation GetBooking(int BookingId);
        //void UpdateBooking(Hotel_reservation reservation, int roomId);
        //void BookingCheckin(Hotel_reservation reservation);
        //void BookingCheckout(Hotel_reservation reservation);


    }
}
