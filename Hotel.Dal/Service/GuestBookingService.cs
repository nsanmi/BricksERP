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
    public class GuestBookingService: IGuestBookingService
    {
        readonly HotelDBEntities _context = new HotelDBEntities();

            public int AddGuest(Hotel_guest guest)
            {

                _context.Hotel_guest.Add(guest);

                _context.SaveChanges();
                return guest.Id;
            }

            public Hotel_guest GetGuestAccount(int guestId)
            {
                return _context.Hotel_guest.FirstOrDefault(m => m.Id == guestId);
            }

            public void UpdateGuest(Hotel_guest guest)
            {
                var guest_old = _context.Hotel_guest.First(e => e.Id == guest.Id);

                guest_old.First_name = guest.First_name;
                guest_old.Last_name = guest.Last_name;
                guest_old.Country = guest.Country;
                guest_old.State = guest.State;
                guest_old.Password = guest.Password;
                guest_old.Nextofkin_fullname = guest.Nextofkin_fullname;
                guest_old.Other_names = guest.Other_names;
                guest_old.Email = guest.Email;
                guest_old.Address = guest.Address;
                guest_old.Nextofkin_fulladdress = guest.Nextofkin_fulladdress;
                guest_old.Nextofkin_phonenumber = guest.Nextofkin_phonenumber;
                guest_old.status = guest.status;

                _context.Entry(guest_old).State = EntityState.Modified;

                _context.SaveChanges();

            }
            public IQueryable<Hotel_guest> GetAllGuest()
            {

                return _context.Hotel_guest.AsQueryable();
            }

        public IQueryable<Hotel_guest> GetGuests()
        {
            return _context.Hotel_guest.Where(e => e.deleted == 0).AsQueryable();
        }


        public int RoomBooking(Hotel_reservation reservation, int id, int guestId)
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

        public int AddBooking(Hotel_reservation reservation)
        {

            _context.Hotel_reservation.Add(reservation);

            _context.SaveChanges();
            return reservation.Id;
        }

        public Hotel_reservation GetBooking(int BookingId)
        {
            return _context.Hotel_reservation.FirstOrDefault(m => m.Id == BookingId);
        }




        //public void AddRoomFiles(ws_complain_files complainFiles)
        //{
        //    _context.ws_complain_files.Add(complainFiles);

        //    _context.SaveChanges();
        //}


    }
    }


