using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.IService
{
    public interface IAnnouncementService
    {
        void AddAnnouncement(admin_announcement announcement);
        IQueryable<admin_announcement> GetAnnouncements();
        void DeleteAnnouncement(int id);
    }
}
