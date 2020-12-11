using HRM.DAL.IService;
using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Service
{
   public class LookupService: ILookupService
    {
        readonly oneportalEntities _context = new oneportalEntities();


        public ws_lookup GetLookup(string LookupId)
        {
            return _context.ws_lookup.Find(LookupId);
        }

        public IQueryable<ws_lookup> GetAllLookup()
        {

            return _context.ws_lookup.AsQueryable();
        }
    }
}
