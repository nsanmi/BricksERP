using Hotel.Dal.IService;
using Hotel.Dal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Dal.Service
{
    public class BookingService: IBookingService
    {

        readonly HotelDBEntities _context = new HotelDBEntities();

        public int AddRoom(Hotel_Room room)
        {

            _context.Hotel_Room.Add(room);

            _context.SaveChanges();
            return room.Id;
        }

        public Hotel_Room GetRoom(int RoomId)
        {
            return _context.Hotel_Room.FirstOrDefault(m => m.Id == RoomId);
        }

        public void UpdateRoom(Hotel_Room room)
        {
            var room_old = _context.Hotel_Room.First(e => e.Id == room.Id);

            room_old.RoomImage1 = room.RoomImage1;
            room_old.rate = room.rate;
            room_old.Capacity = room.Capacity;
            room_old.updated_date = DateTime.Now;
            room_old.status = room.status;
            _context.Entry(room_old).State = EntityState.Modified;

            _context.SaveChanges();

        }
        public IQueryable<Hotel_Room> GetAllRoom()
        {

            return _context.Hotel_Room.AsQueryable();
        }

        //public void AddRoomFiles(ws_complain_files complainFiles)
        //{
        //    _context.ws_complain_files.Add(complainFiles);

        //    _context.SaveChanges();
        //}

        public int DeleteRoom(int id)
        {
            var existingRoom = _context.Hotel_Room.First(m => m.Id == id);
            existingRoom.status = 4;
            _context.Entry(existingRoom).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingRoom.Id;
        }

        public int RoomBooking(Hotel_reservation reservation,int id, int guestId)
        {
            var status_old = _context.Hotel_reservation.First(m => m.Id == id);
            status_old.reservation_status = 1;
            status_old.reservation_date = DateTime.Now;
            status_old.guest_id = guestId;
            _context.Entry(status_old).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return status_old.Id;
        }
        public IQueryable<Hotel_reservation> GetAllReservation()
        {

            return _context.Hotel_reservation.AsQueryable();
        }

        public Hotel_reservation GetBooking(int BookingId)
        {
            return _context.Hotel_reservation.FirstOrDefault(m => m.Id == BookingId);
        }

        public void UpdateBooking(Hotel_reservation reservation, int roomId)
        {
            var reservation_old = _context.Hotel_reservation.First(e => e.Id == reservation.Id);

            reservation_old.reservation_status = reservation.reservation_status;
            reservation_old.reservation_comment = reservation.reservation_comment;
            reservation_old.reservation_checking_date = reservation.reservation_checking_date;
            reservation_old.Hotel_ReservationType = reservation.Hotel_ReservationType;
            reservation_old.Room_id = roomId;
            _context.Entry(reservation_old).State = EntityState.Modified;

            _context.SaveChanges();

        }

        public void BookingCheckin(Hotel_reservation reservation)
        {
            var reservation_old = _context.Hotel_reservation.First(e => e.Id == reservation.Id);

            
            reservation_old.reservation_checking_date = reservation.reservation_checking_date;
          
            reservation_old.reservation_status = reservation.reservation_status;
            _context.Entry(reservation_old).State = EntityState.Modified;

            _context.SaveChanges();

        }


        public void BookingCheckout(Hotel_reservation reservation)
        {
            var reservation_old = _context.Hotel_reservation.First(e => e.Id == reservation.Id);

          
            reservation_old.reservation_checkout_date = reservation.reservation_checkout_date;

            reservation_old.reservation_status = reservation.reservation_status;
            _context.Entry(reservation_old).State = EntityState.Modified;

            _context.SaveChanges();

        }

        public List<string> GetBookingAdmin()
        {
            var bookingAdmin = _context.Hotel_booking_admin.Select(m => m.admin_hrm_employee.emp_work_email).ToList();
            //.Select(m => m.admin_hrm_employee.emp_work_email)
            return bookingAdmin;
        }


        public int DeleteBooking(int id)
        {
            var existingBooking = _context.Hotel_reservation.First(m => m.Id == id);
            existingBooking.reservation_status = 4;
            _context.Entry(existingBooking).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingBooking.Id;
        }

        public List<string> BookingNotification()
        {
            //var userEmail = _context.admin_hrm_employee.Where(x=>x.u)
            var notification = _context.Hotel_guest.Select(m => m.Address).ToList();
            return notification;
        }


    }
}
