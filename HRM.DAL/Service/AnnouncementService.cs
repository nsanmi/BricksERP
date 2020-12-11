using HRM.DAL.IService;
using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Service
{
    public class AnnouncementService: IAnnouncementService
    {
        readonly oneportalEntities _context = new oneportalEntities();

        public void AddAnnouncement(admin_announcement announcement)
        {
            _context.admin_announcement.Add(announcement);
            _context.SaveChanges();
        }

        public IQueryable<admin_announcement> GetAnnouncements()
        {
            return _context.admin_announcement.AsQueryable();
        }


        public void DeleteAnnouncement(int id)
        {
            _context.admin_announcement.Remove(_context.admin_announcement.FirstOrDefault(e => e.id == id));
            _context.SaveChanges();
        }
    }
}
