//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkFlow.DAL.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class bpm_workflow_document
    {
        public System.Guid id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public System.Guid workflow_id { get; set; }
        public System.DateTime created_at { get; set; }
    
        public virtual bpm_workflow bpm_workflow { get; set; }
    }
}
