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
    
    public partial class ws_complain_files
    {
        public int Id { get; set; }
        public int ComplainId { get; set; }
        public string Filename { get; set; }
    
        public virtual ws_complain ws_complain { get; set; }
    }
}
