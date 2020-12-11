using HRM.DAL.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Models
{
    public class TimesheetItem
    {
        public TimesheetItem(admin_hrm_timesheet_item item)
        {
            timesheet_id = item.timesheet_id;
            timesheet_item_id = item.timesheet_item_id;
            date = item.date;
            duration = item.duration.Value;
            comment = item.comment;
            project_id = item.project_id;
            emp_number = item.emp_number;
            activity_id = item.activity_id;
            week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
            item_title = OptionUtil.GetTimeshetItemTitle(item.project_id, item.activity_id);

        }

        public int timesheet_item_id { get; set; }
        public int timesheet_id { get; set; }
        public DateTime date { get; set; }
        public int duration { get; set; }
        public string comment { get; set; }
        public int project_id { get; set; }
        public int emp_number { get; set; }
        public int activity_id { get; set; }
        public int week { get; set; }
        public string item_title { get; set; }
    }
}
