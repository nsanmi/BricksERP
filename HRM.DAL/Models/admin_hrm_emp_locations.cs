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
    
    public partial class admin_hrm_emp_locations
    {
        public int id { get; set; }
        public int emp_number { get; set; }
        public int location_id { get; set; }
    
        public virtual admin_hrm_employee admin_hrm_employee { get; set; }
        public virtual admin_hrm_location admin_hrm_location { get; set; }
    }
}