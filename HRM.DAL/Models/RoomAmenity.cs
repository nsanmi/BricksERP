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
    
    public partial class RoomAmenity
    {
        public int Room_RoomID { get; set; }
        public int Amenity_AmenityID { get; set; }
        public int AmenityQuantity { get; set; }
    
        public virtual Amenity Amenity { get; set; }
        public virtual Room Room { get; set; }
    }
}