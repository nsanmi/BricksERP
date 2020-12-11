using System;

namespace HRM.DAL.Models.ViewModel
{
    public class TimesheetLog
    {
        public TimesheetLog(admin_hrm_timesheet_action_log log)
        {
            timesheet_action_log_id = log.timesheet_action_log_id;
            comment = log.comment;
            action = log.action;
            date_time = log.date_time.ToString("yyyy-MM-dd HH:mm tt");
            performed_by = log.performed_by;
            timesheet_id = log.timesheet_id;
            emp_name = string.Format("{0} {1} {2}", log.admin_hrm_employee.emp_lastname, log.admin_hrm_employee.emp_firstname, log.admin_hrm_employee.emp_middle_name);
        }

        public int timesheet_action_log_id { get; set; }
        public string comment { get; set; }
        public string action { get; set; }
        public string date_time { get; set; }
        public int performed_by { get; set; }
        public int timesheet_id { get; set; }
        public string performed_by_name { set; get; }
        public string emp_name { get; set; }

    }
}