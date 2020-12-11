using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Util
{
    public static class EmailUtil
    {
        readonly static oneportalEntities _context = new oneportalEntities();

        public static string GetTemplate(string name)
        {
            var template = _context.ws_email_template.FirstOrDefault(e => e.name == name && e.deleted == 0);
            if (template != null) return template.template;
            return "";
        }
    }
}
