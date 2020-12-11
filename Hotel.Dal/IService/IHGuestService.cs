using Hotel.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Dal.IService
{
    public interface IHGuestService
    {

        int AddGuest(Hotel_guest guest);
        Hotel_guest GetGuest(int guestId);
        IQueryable<Hotel_guest> GetAllGuest();
        void UpdateGuest(Hotel_guest room);
        int DeleteGuest(int id);

        void DeactivateGuest(int guest_id);
        Hotel_guest GetDeactivatedGuest(string email);
        List<string> GetBookingAdmin();
        List<string> ResolvedNotification();
        void UpdateDiscount(Hotel_guest room);
    }
}
