using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkFlow.DAL;

namespace OnePortal.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        public ActionResult Index()
        {
            return View();
        }

        public FileResult Download(Guid id)
        {
            var document = Util.GetWorkflowDocument(id);
            //var FileVirtualPath = "~/App_Data/uploads/" + ImageName;
            return File(document.location, "application/force-download", Path.GetFileName(document.location));
        }

        public ActionResult Downloads()
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/uploads/"));
            System.IO.FileInfo[] fileNames = dir.GetFiles("*.*"); List<string> items = new List<string>();
            foreach (var file in fileNames)
            {
                items.Add(file.Name);
            }
            return View(items);
        }
    }
}