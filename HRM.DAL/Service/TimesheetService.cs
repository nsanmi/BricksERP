using System;
using System.Collections.Generic;
using System.Linq;
using HRM.DAL.IService;
using HRM.DAL.Models;

namespace HRM.DAL.Service
{
    public class TimesheetService:ITimesheetService
    {
        readonly oneportalEntities _context = new oneportalEntities();

        public void AddTimesheetItem(List<admin_hrm_timesheet_item> time_sheets)
        {
            var timesheet = time_sheets.FirstOrDefault();
            DeleteMonthTimeSheet(timesheet.date.Month, timesheet.date.Year, timesheet.emp_number);

            foreach (var time_sheet in time_sheets)
            {
                _context.admin_hrm_timesheet_item.Add(time_sheet);
            }
            _context.SaveChanges();
        }


        public int AddTimesheet(admin_hrm_timesheet timesheet)
        {
            _context.admin_hrm_timesheet.Add(timesheet);
            _context.SaveChanges();

            return timesheet.timesheet_id;
        }

        public admin_hrm_timesheet GetEmpMonthTimesheet(int month, int year, int emp_number)
        {
            var date = new DateTime(year, month, 1);
            var end_date = date.AddDays(DateTime.DaysInMonth(year, month));
            return _context.admin_hrm_timesheet.FirstOrDefault(e => e.start_date >= date && e.end_date <= end_date && e.emp_number == emp_number);
        }

        public IQueryable<admin_hrm_timesheet_item> GetTimesheets(int month,int year,int emp_number)
        {
            var date = new DateTime(year, month, 1);
            var end_date = date.AddDays(DateTime.DaysInMonth(year, month));
            return _context.admin_hrm_timesheet_item.Where(e => e.date >= date && e.date <= end_date && e.emp_number==emp_number);
        }

        public IQueryable<admin_hrm_timesheet_item> GetTimesheetItems(int timesheet_id)
        {
            return _context.admin_hrm_timesheet_item.Where(e => e.timesheet_id==timesheet_id);
        }

        public IQueryable<admin_hrm_timesheet> GetMonthlyAudit(int year,int month)
        {
            return _context.admin_hrm_timesheet.Where(e => e.start_date.Year == year && e.start_date.Month==month);
        }

        public admin_hrm_timesheet GetLastestTimesheet(int emp_number)
        {
            return _context.admin_hrm_timesheet.OrderByDescending(o=>o.timesheet_id).FirstOrDefault(e => e.emp_number == emp_number);
        }

        public IQueryable<admin_hrm_timesheet_action_log> GetTimesheetLogs(int timesheet_id)
        {
            return _context.admin_hrm_timesheet_action_log.Where(e => e.timesheet_id == timesheet_id);
        }

        public admin_hrm_timesheet GetTimesheet(int timesheet_id)
        {
            return _context.admin_hrm_timesheet.FirstOrDefault(e => e.timesheet_id == timesheet_id);
        }

        public IQueryable<admin_hrm_timesheet> GetTimesheets()
        {
            return _context.admin_hrm_timesheet;
        }

        public admin_hrm_timesheet GetTimesheetByDates(DateTime start_date,DateTime end_date, int emp_number)
        {
            return _context.admin_hrm_timesheet.FirstOrDefault(e => e.start_date == start_date && e.end_date==end_date && e.emp_number==emp_number);
        }

        public void UpdateTimesheet(admin_hrm_timesheet timesheet)
        {
            var old = GetTimesheet(timesheet.timesheet_id);
            _context.Entry(old).CurrentValues.SetValues(timesheet);
            _context.SaveChanges();
        }

        public void AddTimesheetLog(admin_hrm_timesheet_action_log log)
        {
            _context.admin_hrm_timesheet_action_log.Add(log);
            _context.SaveChanges();
        }

        public void DeleteMonthTimeSheet(int month,int year, int emp_number)
        {
            var start_date = new DateTime(year, month, 1);
            var end_date = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            var timesheets = _context.admin_hrm_timesheet_item.Where(e => e.emp_number == emp_number && (e.date >= start_date && e.date <= end_date));
            _context.admin_hrm_timesheet_item.RemoveRange(timesheets);
            _context.SaveChanges();
        }

        public IQueryable<admin_hrm_timesheet> GetAllTimesheet()
        {
            return _context.admin_hrm_timesheet.AsQueryable();
        }

        public IQueryable<admin_hrm_timesheet_item> GetAllTimesheetItem()
        {
            return _context.admin_hrm_timesheet_item.AsQueryable();
        }
    }
}
