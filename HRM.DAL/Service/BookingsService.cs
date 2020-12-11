using HRM.DAL.IService;
using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Service
{
    public class BookingsService : IBookingsService
    {
        readonly oneportalEntities _context = new oneportalEntities();


        public int AddGuest(Guest guests)
        {

            _context.Guests.Add(guests);

            _context.SaveChanges();
            return guests.GuestID;
        }

        public Guest GetGuest(int guestId)
        {
            return _context.Guests.FirstOrDefault(m => m.GuestID == guestId);
        }

        public void UpdateGuest(Guest guests)
        {
            var guest_old = _context.Guests.Include(d => d.User_Email).First(e => e.GuestID == guests.GuestID);
            //var useremail = new User_Email
            //{
            //    EmailID =  

            //};

            guest_old.FirstName = guests.FirstName;
            guest_old.LastName = guests.LastName;
            guest_old.Phones = guests.Phones;
            //guest_old.User_Email;

            //guest_old.address = guests.phone;
            //products_old.updated_date = DateTime.Now;
            _context.Entry(guest_old).State = EntityState.Modified;
            _context.SaveChanges();

        }
        public IQueryable<Guest> GetAllGuest()
        {

            return _context.Guests.AsQueryable();
        }

       
        public int DeleteGuest(int id)
        {
            var existingguest = _context.Guests.First(m => m.GuestID == id);
            //existingguest.Deleted = 1;
            _context.Entry(existingguest).State = EntityState.Modified;
            //_context.Entry  (existingproducts).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingguest.GuestID;
        }

        public Guest GetDeactivatedGuest(string email)
        {
            return _context.Guests.FirstOrDefault(m => m.LastName == email);
        }

        public void DeactivateGuest(int guestid)
        {
            var old_guest = _context.Guests.Include(m => m.User_Email).FirstOrDefault(e => e.GuestID == guestid);
            if (old_guest != null)
            {
                //old_guest.active = 0;
                //old_guest.deleted = 1;
                _context.Entry(old_guest).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public Guest GetGuest(string email)
        {
            return _context.Guests.FirstOrDefault(m => m.Email == email && m.Deleted == 0);
        }


    }
}
