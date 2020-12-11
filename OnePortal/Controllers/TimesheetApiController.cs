using HRM.DAL.IService;
using HRM.DAL.Models;
using HRM.DAL.Models.ViewModel;
using HRM.DAL.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace OnePortal.Controllers
{
    public class TimesheetApiController : ApiController
    {
        ITimesheetService _timesheetService;
        ILeaveService _leaveService;
        public TimesheetApiController(ITimesheetService timesheetService,ILeaveService leaveService)
        {
            _timesheetService = timesheetService;
            _leaveService = leaveService;
        }

        [HttpPost]
        public List<TimesheetItem> PostTimesheets(HttpRequestMessage request)
        {

            var timesheet_items = _timesheetService.GetTimesheets(8, 2017, 35);
            var time_items = new List<TimesheetItem>();
            foreach (var item in timesheet_items)
            {
                time_items.Add(new TimesheetItem(item));
            }
            return time_items;

        }

        
        public List<TimesheetItem> GetTimesheetItems(int id)
        {

            var timesheet_items = _timesheetService.GetTimesheetItems(id);
            var time_sheet = _timesheetService.GetTimesheet(id);
            var leaves = _leaveService.GetLeaves().Where(e=>e.date >= time_sheet.start_date || e.date <=time_sheet.end_date);
            var time_items = new List<TimesheetItem>();
            foreach (var item in timesheet_items)
            {
                time_items.Add(new TimesheetItem(item));
            }
            //check if there is any leaves during this period
            if (leaves.Any())
            {
                foreach(var leave in leaves)
                {

                }
            }
            return time_items;

        }

        [HttpPost]
        public int NewTimesheet(int month,int year)
        {
            var start_date = new DateTime(year,month,1);
            var timesheet = new admin_hrm_timesheet();
            timesheet.start_date = start_date;
            timesheet.end_date = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            timesheet.emp_number = 35;
            timesheet.state = "NOT SUBMITTED";

            _timesheetService.AddTimesheet(timesheet);
            return timesheet.timesheet_id;
        }



        public List<TimesheetLog> GetTimesheetLogs(int id)
        {
            var logs = new List<TimesheetLog>();
            var timesheet_logs = _timesheetService.GetTimesheetLogs(id);
            foreach(var t_log in timesheet_logs)
            {
                logs.Add(new TimesheetLog(t_log));
            }

            return logs;
        }


        public List<TimesheetCategory> GetCategories()
        {
            return OptionUtil.GetCategories(); 
        }

    }
}
