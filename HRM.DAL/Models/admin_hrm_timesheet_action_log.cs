//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRM.DAL.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class admin_hrm_timesheet_action_log
    {
        public int timesheet_action_log_id { get; set; }
        public string comment { get; set; }
        public string action { get; set; }
        public System.DateTime date_time { get; set; }
        public int performed_by { get; set; }
        public int timesheet_id { get; set; }
    
        public virtual admin_hrm_employee admin_hrm_employee { get; set; }
        public virtual admin_hrm_timesheet admin_hrm_timesheet { get; set; }
    }
}