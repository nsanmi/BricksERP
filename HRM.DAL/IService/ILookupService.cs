using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.IService
{
    public interface ILookupService
    {


        ws_lookup GetLookup(string LookupId);
        IQueryable<ws_lookup> GetAllLookup();
        
        
    }
}
