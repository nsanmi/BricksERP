using HRM.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace HRM.DAL.IService
{
    public interface ILeaveService
    {
        admin_hrm_leave_request AddLeaveRequest(admin_hrm_leave_request request);
        void AddLeave(List<admin_hrm_leave> leaves);
        void AddRequestComment(admin_hrm_leave_request_comment request_comment);
        IQueryable<admin_hrm_leave_entitlement> GetEmployeeEntitlements(int emp_number);
        IQueryable<admin_hrm_leave> GetEmployeeLeave(int emp_number);
        IQueryable<admin_hrm_leave_request> GetEmployeeLeaveRequest(int emp_number);
        IQueryable<admin_hrm_leave_request> GetPendingApproval(int emp_number);
        void UpdateLeave(List<admin_hrm_leave> leaves);
        admin_hrm_leave_request GetRequest(int id);
        admin_hrm_leave GetLeave(int id);
        IQueryable<admin_hrm_leave> GetRequestLeaves(int request_id);
        void AddHoliday(admin_hrm_holiday holiday);
        IQueryable<admin_hrm_holiday> GetHolidays();
        admin_hrm_holiday GetHoliday(int id);
        void UpdateEntitlement(admin_hrm_leave_entitlement entitlement);
        admin_hrm_leave_entitlement GetEntitlement(int type_id, int emp_number);
        IQueryable<admin_hrm_leave_request> GetSubLeaves(int emp_number, int[] status);
        void AddEntitlement(admin_hrm_leave_entitlement entitlement);
        IQueryable<admin_hrm_leave_request> GetApproved();
        IQueryable<admin_hrm_leave> GetLeaves();


        //IQueryable<admin_hrm_leave> GetOnLeaveUser();
        //IQueryable<admin_hrm_leave> GetOnLeaves();
        IQueryable<admin_hrm_leave> GetLeaveUser(int status);
        
        
        IQueryable<admin_hrm_leave_request> GetAllLeave();

        int CancelLeave(int id);





    }
}
