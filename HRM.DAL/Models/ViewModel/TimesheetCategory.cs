

namespace HRM.DAL.Models.ViewModel
{
    public  class TimesheetCategory
    {
        public TimesheetCategory(admin_hrm_timesheet_lk_project_activity proj_activity)
        {
            project_id = proj_activity.project_id;
            activity_id = proj_activity.activity_id;
            name = proj_activity.admin_hrm_activity.activity_name;
        }
        public int project_id { set; get; }
        public int activity_id { set; get; }
        public string name { set; get; }
    }
}
