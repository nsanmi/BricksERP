using HRM.DAL.IService;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.IService;

namespace OnePortal.Controllers
{
    public class ProcessController : Controller
    {
        IProcessService _processService;
        IEmployeeService _employeeService;

        public ProcessController(IProcessService processService, IEmployeeService employeeService)
        {
            _processService = processService;
            _employeeService = employeeService;
        }
        // GET: Process
        public ActionResult Manage(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var processes = _processService.GetProcesses().OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize);
            return View(processes);
        }
        [HttpPost]
        public ActionResult Manage(bpm_process process)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            process.definition = "<root></root>";
            process.created_by = employee.emp_number;
            process.created_at = DateTime.Now;
            _processService.AddProcess(process);
            return RedirectToAction("Manage");
        }
    }
}