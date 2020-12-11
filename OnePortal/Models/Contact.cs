using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnePortal.Models
{
    public class Contact
    {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        

        public Contact()
        {
            id = -1;
            FirstName = string.Empty;
            LastName = string.Empty;
        }
    }
}