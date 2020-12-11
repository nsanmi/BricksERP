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
    public class HGuestService: IHGuestService
    {

        readonly HotelDBEntities _context = new HotelDBEntities();

        public int AddGuest(Hotel_guest guest)
        {

            _context.Hotel_guest.Add(guest);

            _context.SaveChanges();
            return guest.Id;
        }

        public Hotel_guest GetGuest(int guestId)
        {
            return _context.Hotel_guest.FirstOrDefault(m => m.Id == guestId);
        }

        //public admin_hrm_employee GetEmployee(int emp_number)
        //{
        //    return _context.admin_hrm_employee.FirstOrDefault(e => e.emp_number == emp_number);
        //}


        public void UpdateDiscount(Hotel_guest guest)
        {
            var guest_old = _context.Hotel_guest.First(e => e.Id == guest.Id);

            guest_old.First_name = guest_old.First_name;
            guest_old.Last_name = guest_old.Last_name;
            guest_old.Country = guest_old.Country;
            guest_old.State = guest_old.State;
            guest_old.Password = guest_old.Password;
            guest_old.Nextofkin_fullname = guest_old.Nextofkin_fullname;
            guest_old.Other_names = guest_old.Other_names;
            guest_old.Email = guest_old.Email;
            guest_old.Address = guest_old.Address;
            guest_old.Nextofkin_fulladdress = guest_old.Nextofkin_fulladdress;
            guest_old.Nextofkin_phonenumber = guest_old.Nextofkin_phonenumber;
            guest_old.status = guest_old.status;
            guest_old.deleted = guest.deleted;
            guest_old.AddedDate = guest_old.AddedDate;
            guest_old.DateOfLastDiscountUpdate = DateTime.Now;
            guest_old.DiscountAmount = guest.DiscountAmount;
            guest_old.DiscountPercentage = guest.DiscountPercentage;
            guest_old.DiscountapprovedBy = guest.DiscountapprovedBy;
            guest_old.EmailOfDiscountapprovedPerson = guest.EmailOfDiscountapprovedPerson;
            guest_old.emp_number = guest.emp_number;
            



            _context.Entry(guest_old).State = EntityState.Modified;

            _context.SaveChanges();

        }

        public List<string> GetBookingAdmin()
        {
            var bookingAdmin = _context.Hotel_booking_admin.Where(m => m.active == 1).Select(m => m.admin_hrm_employee.emp_work_email).ToList();
            return bookingAdmin;
        }

        public List<string> ResolvedNotification()
        {
            //var userEmail = _context.admin_hrm_employee.Where(x=>x.u)
            var notification = _context.admin_hrm_employee.Select(m => m.emp_work_email).ToList();
            return notification;
        }

        public void UpdateGuest(Hotel_guest guest)
        {
            var guest_old = _context.Hotel_guest.First(e => e.Id == guest.Id);

            guest_old.First_name = guest.First_name;
            guest_old.Last_name = guest.Last_name;
            guest_old.Country =  guest.Country;
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

        //public void AddRoomFiles(ws_complain_files complainFiles)
        //{
        //    _context.ws_complain_files.Add(complainFiles);

        //    _context.SaveChanges();
        //}

        public int DeleteGuest(int id)
        {
            var existingGuest = _context.Hotel_guest.First(m => m.Id == id);
            existingGuest.deleted = 1;
            _context.Entry(existingGuest).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingGuest.Id;
        }

        public Hotel_guest GetDeactivatedGuest(string email)
        {
            return _context.Hotel_guest.FirstOrDefault(m => m.Email == email);
        }

        public void DeactivateGuest(int guest_id)
        {
            var old_guest = _context.Hotel_guest.FirstOrDefault(e => e.Id == guest_id);
            if (old_guest != null)
            {
                old_guest.status = 1;
                old_guest.deleted = 1;
                _context.Entry(old_guest).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
    }
}
