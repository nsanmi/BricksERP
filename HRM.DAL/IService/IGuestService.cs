using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.IService
{
    public interface IGuestService
    {

        int AddGuest(Guest guest);
        IQueryable<Guest> GetGuests();
        Guest GetGuest(int guest_id);
        void UpdateGuest(Guest guest);
        void DeleteGuest(int id);
        //IQueryable<admin_hrm_emp_reportto> GetSubordinates(int emp_number);
        //IQueryable<admin_hrm_emp_reportto> GetSupervisors(int emp_number);
        //IQueryable<admin_hrm_employee> GetUsers();
        //admin_hrm_employee GetEmployeeByUserId(string user_id);
        //void AddEmergencyContact(admin_hrm_emp_emergency_contacts contacts);
        //void DeleteEmergencyContact(int emp_number, int seqno);
        
        //void AddDocument(admin_hrm_uploaded_document document, int guest_id);
        //admin_hrm_uploaded_document GetDocument(Guid document_id);
        //void AddLocation(admin_hrm_emp_locations locations);
      
        //Added by Johnbosco
        void DeactivateGuest(int guest_id);
        Guest GetDeactivatedGuest(string email);
        Guest GetGuest(string email);
    }


}

