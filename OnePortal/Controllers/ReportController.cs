using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkFlow.DAL.IService;

namespace OnePortal.Controllers
{
    public class ReportController : Controller
    {
        IWorkflowService _workflowService;
        public ReportController(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        // GET: Report
        public ActionResult Expenditure(int? project_id,int? month, int? year)
        {
            //var project = 4013;
            var project = 1;

            if (project_id.HasValue) project = project_id.Value;

            var id = Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3179");
            //var id = 1;
            ViewBag.workflows = _workflowService.GetWorkflows().Where(e => e.process_id != id && e.project_id == project);

            ViewBag.project = project;
            return View();
        }
    }
}