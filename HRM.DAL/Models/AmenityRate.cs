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
    
    public partial class AmenityRate
    {
        public int AmenityRateID { get; set; }
        public decimal Rate { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int Amenity_AmenityID { get; set; }
    
        public virtual Amenity Amenity { get; set; }
    }
}