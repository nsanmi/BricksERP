using HRM.DAL.Models;
using System;
using System.Linq;

namespace HRM.DAL.IService
{
    public interface IEmployeeService
    {
        
        int AddEmployee(admin_hrm_employee employee);
        IQueryable<admin_hrm_employee> GetEmployees();
        admin_hrm_employee GetEmployee(int emp_number);
        void UpdateEmployee(admin_hrm_employee employee);
        void DeleteEmployee(int id);
        IQueryable<admin_hrm_emp_reportto> GetSubordinates(int emp_number);
        IQueryable<admin_hrm_emp_reportto> GetSupervisors(int emp_number);
        IQueryable<admin_hrm_employee> GetUsers();
        admin_hrm_employee GetEmployeeByUserId(string user_id);
        void AddEmergencyContact(admin_hrm_emp_emergency_contacts contacts);
        void DeleteEmergencyContact(int emp_number, int seqno);
        void AddDependent(admin_hrm_emp_dependents dependents);
        void DeleteDependent(int emp_number, int seqno);
        void AddEducation(admin_hrm_emp_education education);
        void DeleteEducation(int id);
        void AddMembership(admin_hrm_emp_member_detail member_Detail);
        void DeleteMembership(int id);
        void AddDocument(admin_hrm_uploaded_document document, int emp_number);
        admin_hrm_uploaded_document GetDocument(Guid document_id);
        void AddJobRecord(admin_hrm_emp_job_record job_Record);
        void AddReportTo(admin_hrm_emp_reportto reportto);
        admin_hrm_directorate GetDirectorate(int emp_number);
        void AddLocation(admin_hrm_emp_locations locations);
        void UpdateEmployeeRecord(admin_hrm_emp_job_record record);
        void UpdateUsername(admin_hrm_employee employee);
        //Added by Johnbosco
        void DeactivateEmployee(int emp_number);
        admin_hrm_employee GetDeactivatedEmployee(string email);
        admin_hrm_employee GetEmployee(string email);
    }
}
