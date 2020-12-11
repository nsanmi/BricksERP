using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM.DAL.Models;

namespace HRM.DAL.IService
{
    public interface ISuggestionService
    {

        int AddSuggestion(admin_hrm_suggestion suggestion);
        admin_hrm_suggestion GetSuggestion(int SuggestionId);
        void UpdateSuggestion(admin_hrm_suggestion suggestion);
        IQueryable<admin_hrm_suggestion> GetAllSuggestion();
        void AddSuggestionFiles(admin_hrm_suggestion_files SuggestionFiles);
        int DeleteSuggestion(int id);
        List<string> GetSuggestionAdmin();
        List<string> SuggestionNotification();

    }
}
