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
    
    public partial class admin_hrm_uploaded_document
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public admin_hrm_uploaded_document()
        {
            this.admin_hrm_emp_file = new HashSet<admin_hrm_emp_file>();
        }
    
        public System.Guid file_id { get; set; }
        public string file_name { get; set; }
        public string description { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public System.DateTime date_added { get; set; }
        public string added_by { get; set; }
        public string location_file { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<admin_hrm_emp_file> admin_hrm_emp_file { get; set; }
    }
}