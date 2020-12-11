using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM.DAL.Models;


namespace HRM.DAL.IService
{
    public interface IComplainService
    {
        int AddComplain(ws_complain complain);
        ws_complain GetComplain(int ComplainId);
        IQueryable<ws_complain> GetAllComplain();
        void UpdateComplain(ws_complain complain);
        void AddComplainFiles(ws_complain_files complainFiles);
        int DeleteComplain(int id);
        int ResolvedComplain(int id, string userId);
        List<string> GetComplainAdmin(); 
        List<string> ResolvedNotification();
    }

}
