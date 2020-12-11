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
    public class AmenityService: IAmenityService
    {
        readonly HotelDBEntities _context = new HotelDBEntities();

        public int AddAmenity(Hotel_amenity amenity)
        {

            _context.Hotel_amenity.Add(amenity);

            _context.SaveChanges();
            return amenity.Id;
        }

        public Hotel_amenity GetAmenity(int amenityId)
        {
            return _context.Hotel_amenity.FirstOrDefault(m => m.Id == amenityId);
        }

        public void UpdateAmenity(Hotel_amenity amenity)
        {
            var amenity_old = _context.Hotel_amenity.First(e => e.Id == amenity.Id);

            amenity_old.amenity_name = amenity.amenity_name;
            amenity_old.amenity_status = amenity.amenity_status;
            amenity_old.amenity_category = amenity.amenity_category;
            amenity_old.amenity_description = amenity.amenity_description;
            _context.Entry(amenity_old).State = EntityState.Modified;

            _context.SaveChanges();

        }
        public IQueryable<Hotel_amenity> GetAllAmenity()
        {

            return _context.Hotel_amenity.AsQueryable();
        }

        //public void AddRoomFiles(ws_complain_files complainFiles)
        //{
        //    _context.ws_complain_files.Add(complainFiles);

        //    _context.SaveChanges();
        //}

        public int DeleteAmenity(int id)
        {
            var existingAmenity = _context.Hotel_amenity.First(m => m.Id == id);
            existingAmenity.amenity_status = 4;
            _context.Entry(existingAmenity).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingAmenity.Id;
        }

    }
}
