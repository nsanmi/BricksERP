using Hotel.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Dal.IService
{
    public interface IAmenityService
    {
        int AddAmenity(Hotel_amenity amenity);
        Hotel_amenity GetAmenity(int amenityId);
        IQueryable<Hotel_amenity> GetAllAmenity();
        void UpdateAmenity(Hotel_amenity amenity);
        int DeleteAmenity(int id);

    }
}
