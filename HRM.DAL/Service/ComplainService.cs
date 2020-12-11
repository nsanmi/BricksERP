using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM.DAL.IService;
using HRM.DAL.Models;



namespace HRM.DAL.Service
{
    public class ComplainService : IComplainService
    {
        readonly oneportalEntities _context = new oneportalEntities();

        public int AddComplain(ws_complain complain)
        {
           
            _context.ws_complain.Add(complain);
           
            _context.SaveChanges();
            return complain.Id; 
        }

        public ws_complain GetComplain(int ComplainId)
        {
            return _context.ws_complain.FirstOrDefault(m => m.Id == ComplainId);
        }

        public void UpdateComplain(ws_complain complain)
        {
            var complain_old = _context.ws_complain.First(e => e.Id == complain.Id);
            
            complain_old.Comment = complain.Comment;
            complain_old.UpdateDate = DateTime.Now;
            _context.Entry(complain_old).State = EntityState.Modified;

            _context.SaveChanges();
            
        }
        public IQueryable<ws_complain> GetAllComplain()
        {
            return _context.ws_complain.AsQueryable();
        }

        public void AddComplainFiles(ws_complain_files complainFiles)
        {
            _context.ws_complain_files.Add(complainFiles);

            _context.SaveChanges();
        }

        public int DeleteComplain(int id)
        {
            var existingComplain = _context.ws_complain.First(m => m.Id == id);
            existingComplain.Deleted = 1;
            _context.Entry(existingComplain).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingComplain.Id;
        }

        public int ResolvedComplain(int id, string userId)
        {
            var resolve_old = _context.ws_complain.First(m => m.Id == id);
            resolve_old.Resolved = "Yes";
            resolve_old.ResolutionDate = DateTime.Now;
            resolve_old.ResolvedBy = userId;
            _context.Entry(resolve_old).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return resolve_old.Id;
        }

        public List<string> GetComplainAdmin()
        {
            var complainAdmin = _context.ws_complain_admin.Where(m => m.active == 1).Select(m => m.admin_hrm_employee.emp_work_email).ToList();
            return complainAdmin;
        }

        public List<string> ResolvedNotification()
        {
            //var userEmail = _context.admin_hrm_employee.Where(x=>x.u)
            var notification = _context.admin_hrm_employee.Select(m => m.emp_work_email).ToList();
            return notification;
        }
    }
}
