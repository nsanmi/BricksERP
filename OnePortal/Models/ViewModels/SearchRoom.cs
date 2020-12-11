using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnePortal.Models.ViewModels
{
    public class SearchRoom
    {
        public DateTime check_in_date { get; set; }
        public DateTime check_out_date { get; set; }
        public int room_type { get; set; }
    }

    public class GetRoom
    {
        public DateTime checkin { get; set; }
        public DateTime checkout { get; set; }
        public int room_type { get; set; }
    }
}