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
    
    public partial class bpm_workflow
    {
        public System.Guid id { get; set; }
        public string workflow { get; set; }
        public System.DateTime created_at { get; set; }
        public int created_by { get; set; }
        public int status { get; set; }
        public System.Guid process_id { get; set; }
        public int project_id { get; set; }
        public string title { get; set; }
        public int deleted { get; set; }
    }
}
