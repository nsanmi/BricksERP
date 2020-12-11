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
    
    public partial class pm_project
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public pm_project()
        {
            this.admin_hrm_timesheet_item = new HashSet<admin_hrm_timesheet_item>();
            this.admin_hrm_timesheet_lk_project_activity = new HashSet<admin_hrm_timesheet_lk_project_activity>();
            this.pm_project_admin = new HashSet<pm_project_admin>();
            this.pm_project_deliverables = new HashSet<pm_project_deliverables>();
            this.pm_project_files = new HashSet<pm_project_files>();
            this.pm_project_funding_period = new HashSet<pm_project_funding_period>();
            this.pm_project1 = new HashSet<pm_project>();
            this.pm_project_strategic_objective = new HashSet<pm_project_strategic_objective>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string code { get; set; }
        public Nullable<int> sub_parent { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> timesheet_only { get; set; }
        public string num { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<admin_hrm_timesheet_item> admin_hrm_timesheet_item { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<admin_hrm_timesheet_lk_project_activity> admin_hrm_timesheet_lk_project_activity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pm_project_admin> pm_project_admin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pm_project_deliverables> pm_project_deliverables { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pm_project_files> pm_project_files { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pm_project_funding_period> pm_project_funding_period { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pm_project> pm_project1 { get; set; }
        public virtual pm_project pm_project2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pm_project_strategic_objective> pm_project_strategic_objective { get; set; }
    }
}
