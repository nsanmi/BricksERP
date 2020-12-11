using HRM.DAL.IService;
using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace HRM.DAL.Service
{
    public  class LeaveService: ILeaveService
    {
        readonly oneportalEntities _context = new oneportalEntities();

        public admin_hrm_leave_request AddLeaveRequest(admin_hrm_leave_request request)
        {
            _context.admin_hrm_leave_request.Add(request);
            _context.SaveChanges();

            return request;
        }
        public void AddLeave(List<admin_hrm_leave> leaves)
        {
            _context.admin_hrm_leave.AddRange(leaves);
            _context.SaveChanges();
        }

        //public void AddLeave(List<admin_hrm_leave> col)
        //{
        //    _context.admin_hrm_leave.AddRange(leaves);
        //    _context.SaveChanges();
        //}

        public void AddRequestComment(admin_hrm_leave_request_comment request_comment)
        {
            _context.admin_hrm_leave_request_comment.Add(request_comment);
            _context.SaveChanges();
        }

        public IQueryable<admin_hrm_leave_entitlement> GetEmployeeEntitlements(int emp_number)
        {
            return _context.admin_hrm_leave_entitlement.Where(e => e.emp_number == emp_number);
        }

        public IQueryable<admin_hrm_leave> GetEmployeeLeave(int emp_number)
        {
            return _context.admin_hrm_leave.Where(e => e.emp_number == emp_number);
        }

        public IQueryable<admin_hrm_leave_request> GetEmployeeLeaveRequest(int emp_number)
        {
            return _context.admin_hrm_leave_request.Where(e => e.emp_number == emp_number);
        }

        public IQueryable<admin_hrm_leave_request> GetPendingApproval(int emp_number)
        {
            var employee = _context.admin_hrm_employee.FirstOrDefault(e => e.emp_number == emp_number);
            var subs = employee.admin_hrm_emp_reportto.Select(e => e.erep_sub_emp_number).ToList();

            var request_id= _context.admin_hrm_leave.Where(e => subs.Contains(e.emp_number) && e.status == 1).Select(e=>e.leave_request_id).Distinct();
            return _context.admin_hrm_leave_request.Where(e => subs.Contains(e.emp_number) && request_id.Contains(e.id));
        }


        public IQueryable<admin_hrm_leave_request> GetApproved()
        {
            var request_id = _context.admin_hrm_leave.Where(e => e.status == 3).Select(e => e.leave_request_id).Distinct();
            return _context.admin_hrm_leave_request.Where(e => request_id.Contains(e.id));
        }


        public IQueryable<admin_hrm_leave_request> GetSubLeaves(int emp_number,int[] status)
        {
            var employee = _context.admin_hrm_employee.FirstOrDefault(e => e.emp_number == emp_number);
            var subs = employee.admin_hrm_emp_reportto.Select(e => e.erep_sub_emp_number).ToList();

            var request_id = _context.admin_hrm_leave.Where(e => subs.Contains(e.emp_number) && status.ToList().Contains(e.status.Value) ).Select(e => e.leave_request_id).Distinct();
            return _context.admin_hrm_leave_request.Where(e => subs.Contains(e.emp_number) && request_id.Contains(e.id));
        }

        public void UpdateLeave(List<admin_hrm_leave> leaves)
        {
            foreach(var leave in leaves)
            {
                var old_leave = _context.admin_hrm_leave.FirstOrDefault(e => e.id == leave.id);
                _context.Entry(old_leave).CurrentValues.SetValues(leave);
            }
            _context.SaveChanges();
        }

        public admin_hrm_leave_request GetRequest(int id)
        {
            return _context.admin_hrm_leave_request.FirstOrDefault(e => e.id == id);
        }

        public admin_hrm_leave GetLeave(int id)
        {
            return _context.admin_hrm_leave.FirstOrDefault(e => e.id == id);
        }

        public IQueryable<admin_hrm_leave> GetLeaves()
        {
            return _context.admin_hrm_leave.AsQueryable();
        }

        public IQueryable<admin_hrm_leave> GetRequestLeaves(int request_id)
        {
            return _context.admin_hrm_leave.Where(e => e.leave_request_id == request_id);
        }

        public void AddHoliday(admin_hrm_holiday holiday)
        {
            _context.admin_hrm_holiday.Add(holiday);
            _context.SaveChanges();
        }

        public IQueryable<admin_hrm_holiday> GetHolidays()
        {
            return _context.admin_hrm_holiday.AsQueryable();
        }

        public admin_hrm_holiday GetHoliday(int id)
        {
            return _context.admin_hrm_holiday.FirstOrDefault(e => e.id == id);
        }

        public void AddEntitlement(admin_hrm_leave_entitlement entitlement)
        {
            _context.admin_hrm_leave_entitlement.Add(entitlement);
            _context.SaveChanges();
        }

        public void UpdateEntitlement(admin_hrm_leave_entitlement entitlement)
        {
            var old_entitlement = _context.admin_hrm_leave_entitlement.FirstOrDefault(e => e.id == entitlement.id);
            _context.Entry(old_entitlement).CurrentValues.SetValues(entitlement);
            _context.SaveChanges();
        }

        public admin_hrm_leave_entitlement GetEntitlement(int type_id,int emp_number)
        {
            //return _context.admin_hrm_leave_entitlement.FirstOrDefault(e => e.leave_type_id == type_id && e.emp_number==emp_number && (e.from_date >= DateTime.Now && e.to_date <=DateTime.Now));
            return _context.admin_hrm_leave_entitlement.FirstOrDefault(e => e.leave_type_id == type_id && e.emp_number == emp_number );
        }


        //public IQueryable <admin_hrm_leave_request> GetOnLeaveUser()
        //{
        //    return _context.admin_hrm_leave_request.Where(e => e.leave_start_date >=   );
        //}


        //public IQueryable<admin_hrm_leave_request> GetOnLeaves(int leaveid, int status)
        //{
        //    return _context.admin_hrm_leave_request.Where(e => e.leave_start_date >= );
        //}

        public IQueryable<admin_hrm_leave> GetLeaveUser(int status)
        {
            return _context.admin_hrm_leave.Where(e => e.status == status);
        }

        

        public IQueryable<admin_hrm_leave_request> GetAllLeave()
        {
            return _context.admin_hrm_leave_request.AsQueryable();
        }

        public int CancelLeave(int id)
        {
            //var existingLeave = _context.admin_hrm_leave.First(m => m.id == id);
            //existingLeave.status = 0;
            //_context.Entry(existingLeave).State = EntityState.Modified;
            //_context.SaveChanges();
            //return existingLeave.id;

            IQueryable<admin_hrm_leave> leavesToCancel = _context.admin_hrm_leave.Where(m => m.leave_request_id == id);
            //leavesToCancel.ForEachAsync(m => { m.status = 0; });
            foreach (var leaveToCancel in leavesToCancel)
            {
                leaveToCancel.status = 0;
            }

            _context.SaveChanges();

            return 0;

        }

        

    }
}
