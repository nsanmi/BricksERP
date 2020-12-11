using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using HRM.DAL.IService;
using HRM.DAL.Models;

namespace HRM.DAL.Service
{

    public class SuggestionService : ISuggestionService
    {
            readonly oneportalEntities _context = new oneportalEntities();

            public int AddSuggestion(admin_hrm_suggestion suggestion)
            {
                _context.admin_hrm_suggestion.Add(suggestion);

                _context.SaveChanges();
                return suggestion.suggestion_id;
            }

            public admin_hrm_suggestion GetSuggestion(int SuggestionId)
            {
                return _context.admin_hrm_suggestion.FirstOrDefault(m => m.suggestion_id == SuggestionId);
            }

          
            public IQueryable<admin_hrm_suggestion> GetAllSuggestion()
            {

                return _context.admin_hrm_suggestion.AsQueryable();
            }

            public void AddSuggestionFiles(admin_hrm_suggestion_files suggestionFiles)
            {
                _context.admin_hrm_suggestion_files.Add(suggestionFiles);

                _context.SaveChanges();
            }

            public int DeleteSuggestion(int id)
            {
                var existingSuggestion = _context.admin_hrm_suggestion.First(m => m.suggestion_id == id);
                 existingSuggestion.deleted = 1;
                _context.Entry(existingSuggestion).State = EntityState.Modified;
                //_context.Entry(existingComplain).CurrentValues.SetValues(id);
                _context.SaveChanges();
                return existingSuggestion.suggestion_id;
            }

        //public List<string> GetSuggestions()
        //{

        //    var suggest = _context.admin_hrm_suggestion.Select(m => m.suggestion_id).ToList();
        //    return suggest.;
        //}

        public List<string> GetSuggestionAdmin()
        {
            var suggestionAdmin = _context.admin_hrm_suggestion_admin.Where(m => m.active == 1).Select(m => m.admin_hrm_employee.emp_work_email).ToList();
            return suggestionAdmin;
        }


        public List<string> SuggestionNotification()
            {
                //var userEmail = _context.admin_hrm_employee.Where(x=>x.u)
                var notification = _context.admin_hrm_employee.Select(m => m.emp_work_email).ToList();
                return notification;
            }

        public void UpdateSuggestion(admin_hrm_suggestion suggestion)
        {
            var suggestion_old = _context.admin_hrm_suggestion.First(e => e.suggestion_id == suggestion.suggestion_id);

            suggestion_old.comment = suggestion.comment;
            
            _context.Entry(suggestion_old).State = EntityState.Modified;

            _context.SaveChanges();

        }






    }
}
