using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnePortal.Models
{
    public class HtmlInput
    {
        [AllowHtml]
        public string email { get; set; }
    }
}