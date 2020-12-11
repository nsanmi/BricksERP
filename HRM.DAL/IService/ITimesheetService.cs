using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HRM.DAL.IService
{
    public interface ITimesheetService
    {
        void AddTimesheetItem(List<admin_hrm_timesheet_item> time_sheets);
        IQueryable<admin_hrm_timesheet_item> GetTimesheets(int month, int year, int emp_number);
        IQueryable<admin_hrm_timesheet_item> GetTimesheetItems(int timesheet_id);
        IQueryable<admin_hrm_timesheet_action_log> GetTimesheetLogs(int timesheet_id);
        admin_hrm_timesheet GetTimesheet(int timesheet_id);
        admin_hrm_timesheet GetLastestTimesheet(int emp_number);
        int AddTimesheet(admin_hrm_timesheet timesheet);
        admin_hrm_timesheet GetTimesheetByDates(DateTime start_date, DateTime end_date,int emp_number);
        void UpdateTimesheet(admin_hrm_timesheet timesheet);
        void AddTimesheetLog(admin_hrm_timesheet_action_log log);
        void DeleteMonthTimeSheet(int month, int year, int emp_number);
        IQueryable<admin_hrm_timesheet> GetTimesheets();
        IQueryable<admin_hrm_timesheet> GetMonthlyAudit(int year, int month);
        admin_hrm_timesheet GetEmpMonthTimesheet(int month, int year, int emp_number);
        IQueryable<admin_hrm_timesheet> GetAllTimesheet();
        IQueryable<admin_hrm_timesheet_item> GetAllTimesheetItem();

    }
}
