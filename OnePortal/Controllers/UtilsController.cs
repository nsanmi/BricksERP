using HRM.DAL.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WorkFlow.DAL;

namespace OnePortal.Controllers
{
    public class UtilsController : ApiController
    {
        public List<SearchResult> GetItems(string id)
        {
            var result = new List<SearchResult>();
            var items = Util.GetItems().Where(e => e.name.Contains(id) );
            foreach (var x in items)
            {
                result.Add(new SearchResult { id = x.id, name =x.name});
            }
            return result;
        }

        public List<SearchResult> GetVendorsByCategory(int category, string term = "")
        {
            var result = new List<SearchResult>();
            var items = Util.GetVendorsByCategory(category, term);
            foreach (var x in items)
            {
                result.Add(new SearchResult { id = x.id, name = x.name });
            }
            return result;
        }
    }
}
