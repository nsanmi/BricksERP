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
    
    public partial class admin_hrm_leave_request_comment
    {
        public int id { get; set; }
        public int leave_request_id { get; set; }
        public Nullable<System.DateTime> created { get; set; }
        public string created_by_name { get; set; }
        public Nullable<int> created_by_id { get; set; }
        public Nullable<int> created_by_emp_number { get; set; }
        public string comments { get; set; }
    
        public virtual admin_hrm_leave_request admin_hrm_leave_request { get; set; }
    }
}
