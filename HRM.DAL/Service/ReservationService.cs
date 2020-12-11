using HRM.DAL.IService;
using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Service
{
    public class ReservationService : IReservationService
    {
        readonly oneportalEntities _context = new oneportalEntities();

        public List<dynamic> GetAvailableRooms()
        {
            //check rooms table by room type for where status == available.,,,   list of rooms //all rooms available
            //check reservations table based on check--in and check-out date, return list of room id. : booked room ids
            //

            return null;
        }
    }
}
