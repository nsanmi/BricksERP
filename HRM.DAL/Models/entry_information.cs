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
    
    public partial class entry_information
    {
        public int id { get; set; }
        public Nullable<int> quantity { get; set; }
        public Nullable<int> price_id { get; set; }
        public Nullable<int> entry_id { get; set; }
        public Nullable<System.DateTime> delivery_date { get; set; }
        public Nullable<int> status { get; set; }
        public string condition { get; set; }
        public string description { get; set; }
        public string comment { get; set; }
        public string amount { get; set; }
        public Nullable<int> emp_number { get; set; }
        public Nullable<int> guest_number { get; set; }
        public Nullable<int> deleted { get; set; }
    }
}