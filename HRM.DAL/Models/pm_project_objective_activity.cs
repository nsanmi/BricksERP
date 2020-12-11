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
    
    public partial class pm_project_objective_activity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public pm_project_objective_activity()
        {
            this.pm_activity_admin = new HashSet<pm_activity_admin>();
            this.pm_activity_budget = new HashSet<pm_activity_budget>();
            this.pm_activity_deliverables = new HashSet<pm_activity_deliverables>();
            this.pm_activity_files = new HashSet<pm_activity_files>();
            this.pm_project_task = new HashSet<pm_project_task>();
        }
    
        public int id { get; set; }
        public string activity { get; set; }
        public Nullable<int> objective_id { get; set; }
        public string description { get; set; }
        public Nullable<System.DateTime> expected_start_date { get; set; }
        public Nullable<System.DateTime> due_date { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public Nullable<int> status { get; set; }
        public string comment { get; set; }
        public string requirements { get; set; }
        public string output { get; set; }
        public string performance_indicator { get; set; }
        public string target { get; set; }
        public Nullable<int> completion { get; set; }
        public string num { get; set; }
        public int deleted { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pm_activity_admin> pm_activity_admin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pm_activity_budget> pm_activity_budget { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pm_activity_deliverables> pm_activity_deliverables { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pm_activity_files> pm_activity_files { get; set; }
        public virtual pm_project_strategic_objective pm_project_strategic_objective { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pm_project_task> pm_project_task { get; set; }
    }
}
