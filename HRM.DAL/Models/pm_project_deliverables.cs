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
    
    public partial class pm_project_deliverables
    {
        public int id { get; set; }
        public string name { get; set; }
        public int deliverable_type { get; set; }
        public int project_id { get; set; }
        public int status { get; set; }
        public System.DateTime created_at { get; set; }
        public int created_by { get; set; }
        public System.DateTime updated_at { get; set; }
        public int updated_by { get; set; }
    
        public virtual admin_hrm_employee admin_hrm_employee { get; set; }
        public virtual admin_hrm_employee admin_hrm_employee1 { get; set; }
        public virtual pm_project pm_project { get; set; }
        public virtual ws_lookup ws_lookup { get; set; }
        public virtual ws_lookup ws_lookup1 { get; set; }
    }
}
