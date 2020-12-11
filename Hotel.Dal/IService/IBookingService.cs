using Hotel.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Dal.IService
{
    public interface IBookingService
    {

        int AddRoom(Hotel_Room complain);
        Hotel_Room GetRoom(int RoomId);
        IQueryable<Hotel_Room> GetAllRoom();
        void UpdateRoom(Hotel_Room room);
        //void AddRoomFiles(ws_complain_files complainFiles);
        int DeleteRoom(int id);
        //int RoomBooking(int id, int guestId);
 int RoomBooking(Hotel_reservation reservation, int id, int guestId);


        Hotel_reservation GetBooking(int BookingId);
        IQueryable<Hotel_reservation> GetAllReservation();
        void UpdateBooking(Hotel_reservation reservation, int roomId);
        void BookingCheckin(Hotel_reservation reservation);
        void BookingCheckout(Hotel_reservation reservation);
        int DeleteBooking(int id);

        //Hotel_Room GetStatus(int RoomId);

        List<string> GetBookingAdmin();
        List<string> BookingNotification();

    }
}
