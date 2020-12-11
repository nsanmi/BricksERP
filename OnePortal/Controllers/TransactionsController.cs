using HRM.DAL.IService;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkFlow.DAL.IService;
using WorkFlow.DAL.Logic;

namespace OnePortal.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {

        IWorkflowService _workflowService;
        IEmployeeService _employeeService;
        IProcessService _processService;
        IVendorService _vendorService;
        ITransactionTokenService _transactionTokenService;

        public TransactionsController(IProcessService processService, IWorkflowService workflowService, IEmployeeService employeeService, IVendorService vendorService, ITransactionTokenService transactionTokenService)
        {
            _workflowService = workflowService;
            _employeeService = employeeService;
            _processService = processService;
            _vendorService = vendorService;
            _transactionTokenService = transactionTokenService;
        }
        // GET: Transactions
        public ActionResult Manage(Guid id)
        {
            var bLWorkflow = new BLWorkflow(id);
            var definition = bLWorkflow.GetDefinition();
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);
            ViewBag.workflow_id = id;
            ViewBag.workflow = bLWorkflow.GetWorkflow();
            var workflow = bLWorkflow.GetWorkflow();

            var list_steps = new List<int> { 1, 2, 3 };
            //get the latest approved step
            var max = 5;
            var available_steps = list_steps.Where(e => e <= max);

            return View();
        }
    }
}