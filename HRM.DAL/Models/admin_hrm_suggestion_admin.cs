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
    
    public partial class admin_hrm_suggestion_admin
    {
        public int id { get; set; }
        public string suggestion_title { get; set; }
        public int employee_number { get; set; }
        public int active { get; set; }
    
        public virtual admin_hrm_employee admin_hrm_employee { get; set; }
    }
}
