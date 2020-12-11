using HRM.DAL.IService;
using Microsoft.AspNet.Identity;
using OnePortal.Helper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WorkFlow.DAL;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.IService;
using WorkFlow.DAL.Logic;
using HRM.DAL.Util;
using System.Data.SqlClient;

namespace OnePortal.Controllers
{
    public class PaymentsController : Controller
    {
        IWorkflowService _workflowService;
        IEmployeeService _employeeService;
        IProcessService _processService;
        IVendorService _vendorService;
        ITransactionTokenService _transactionTokenService;
        IPaymentService _paymentService;

        public PaymentsController(IProcessService processService, IWorkflowService workflowService, IEmployeeService employeeService, IVendorService vendorService, ITransactionTokenService transactionTokenService,IPaymentService paymentService)
        {
            _workflowService = workflowService;
            _employeeService = employeeService;
            _processService = processService;
            _vendorService = vendorService;
            _transactionTokenService = transactionTokenService;
            _paymentService = paymentService;
        }



        // GET: Payments
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompPV()
        {

            var command = new SqlCommand();
            command.CommandText = "sp_get_pending_payments";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            var ds = DataUtil.GetDataSet(command);
            ViewBag.table = ds.Tables[0];

            return View();
        }


        public ActionResult Step(Guid id, int? step_id)
        {
            var bLWorkflow = new BLWorkflow(id);
            var step = new Step();
            if (step_id.HasValue)
            {
                step = bLWorkflow.GetStep(step_id.Value);
                ViewBag.step_id = step_id.Value;
            }
            else
            {
                step = bLWorkflow.GetCurrentStep();
            }

            ViewBag.step = step;
            ViewBag.workflow_id = id;
            ViewBag.workflow = bLWorkflow.GetWorkflow();
            var definition = bLWorkflow.GetDefinition();
            var form_view_data = new Dictionary<string, string>();



            var workflow = bLWorkflow.GetWorkflow();






            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);


#if !DEBUG
            //start 


            var user_can_perform_this_action = false;
            //check if the user is permitted to perform this action
            //get the position
            if (step.Id == 1)
            {
                //get the person making the request
                if (employee.emp_number == workflow.created_by)
                {
                    user_can_perform_this_action = true;
                }
            }
            else if (step.Id > 1)
            {
                var position = step.Definition.Element("position");
                switch (position.Attribute("pos_type").Value)
                {
                    case "sup":
                        //get the supervisor of the person that applied for the request
                        var requestor_no = workflow.admin_hrm_employee.emp_number;
                        if (employee.admin_hrm_emp_reportto.FirstOrDefault(e => e.erep_sub_emp_number == requestor_no) != null)
                        {
                            user_can_perform_this_action = true;
                        }
                        break;
                    case "jti":
                        var job_title_id = Convert.ToInt32(position.Value);
                        if (job_title_id == employee.admin_hrm_emp_job_record.job_title_id)
                        {
                            user_can_perform_this_action = true;
                        }
                        break;
                    case "dir":
                        //get the subunit of the requestor
                        var directorate = _employeeService.GetDirectorate(workflow.created_by);
                        if (directorate != null && directorate.head == employee.admin_hrm_emp_job_record.job_title_id)
                        {
                            user_can_perform_this_action = true;
                        }

                        break;
             case "emp":
                        //get the subunit of the requestor
                        var emp = _employeeService.GetEmployee(workflow.created_by);
                        if (emp != null)
                        {
                            user_can_perform_this_action = true;
                        }

                        break;
                    default:
                        break;
                }


            }
            if (!user_can_perform_this_action)
            {
                ViewBag.message = "You need an elevated access to proceed.";
                //return RedirectToAction("Step", new { id = id });
                return View("Flow");
                //return RedirectToAction("AccessDenied", "Home");
            }

            //end
#endif
           
            return View("Step_PV");
        }



        public ActionResult PV1(Guid id, int? step_id)
        {
            var bLWorkflow = new BLWorkflow(id);
            var step = new Step();
            if (step_id.HasValue)
            {
                step = bLWorkflow.GetStep(step_id.Value);
            }
            else
            {
                step = bLWorkflow.GetCurrentStep();
            }

            ViewBag.step = step;
            ViewBag.workflow_id = id;
            ViewBag.workflow = bLWorkflow.GetWorkflow();
            var definition = bLWorkflow.GetDefinition();




            var workflow = bLWorkflow.GetWorkflow();






            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            return View("Step_PV");
        }

        [HttpPost]
        public ActionResult PV(FormCollection collection)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            //check if this is a new PV
            if (collection["new"] != null && collection["new"].ToString() == "1")
            {
                var transaction = _workflowService.GetWorkflow(Guid.Parse(collection["workflow_id"]));

                var process = _processService.GetProcess(Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3179"));
                var process_element = XElement.Parse(process.definition);

                var code_count = _workflowService.GetWorkflows().Count(e => e.bpm_process.process_code == process.process_code) + 1;

                process_element.Element("process_id").SetValue(Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3179"));
                process_element.Element("project").SetValue(transaction.project_id);
                process_element.Element("created_at").SetValue(DateTime.Now);
                process_element.Element("created_by").SetValue(employee.emp_number);
                process_element.Element("process_number").SetValue(process.process_code + code_count.ToString("D5"));

                var wflow = new bpm_workflow();

                wflow.id = Guid.NewGuid();
                wflow.workflow = process_element.ToString();
                wflow.created_at = DateTime.Now;
                wflow.created_by = employee.emp_number;
                wflow.title = collection["title"].ToString();
                wflow.deleted = 0;
                wflow.process_id = transaction.process_id;
                wflow.project_id = transaction.project_id;
                wflow.status = 22;

                _workflowService.AddWorkflow(wflow);

                collection["workflow_id"] = wflow.id.ToString();
            }



            var id = new Guid(collection["workflow_id"]);
            var bLWorkflow = new BLWorkflow(id);

            var workflow = bLWorkflow.GetWorkflow();
            var step = bLWorkflow.GetCurrentStep();

            var step_type = step.Definition.Element("type").Value;
            var definition = bLWorkflow.GetDefinition();
            var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);

            if (step.Id == 1)
            {
                definition.Element("form").Add(new XElement("pay_to", collection["pay_to"].ToString()));
                definition.Element("form").Add(new XElement("pay_by", collection["pay_by"].ToString()));
                definition.Element("form").Add(new XElement("purpose", collection["purpose"].ToString()));
                definition.Element("form").Add(new XElement("bank_account", collection["bank_account"].ToString()));
                definition.Element("form").Add(new XElement("account", collection["account"].ToString()));
                definition.Element("form").Add(new XElement("withholding_tax", collection["withholding_tax"].ToString()));
                definition.Element("form").Add(new XElement("net_amount", collection["net_amount"].ToString()));
                definition.Element("form").Add(new XElement("amount_ngn", collection["amount_ngn"].ToString()));
                definition.Element("form").Add(new XElement("amount_usd", collection["amount_usd"].ToString()));
                definition.Element("form").Add(new XElement("conversion_rate", collection["conversion_rate"].ToString()));



                var token_id = _transactionTokenService.AddToken(new bpm_transaction_token
                {
                    t_code = Util.Encrypt(Util.GenerateTCode()),
                    transaction_id = id,
                    created_by = employee.emp_number
                });


                var signature_element = new XElement("signature");
                signature_element.Add(new XElement("signature_id", token_id));
                signature_element.Add(new XElement("step", step.Id));
                signature_element.Add(new XElement("name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
                signature_element.Add(new XElement("emp_number", employee.emp_number));
                signature_element.Add(new XElement("created_at", DateTime.Now.ToShortDateString()));
                definition.Element("form").Element("signatures").Add(signature_element);

                //mark step as completed
                definition_step.Element("status").SetValue(1);
                definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
            }
            else if (step.Id == 2 || step.Id == 3 || step.Id == 4)
            {
#if !DEBUG
                var approval_token = collection["approval_token"];
                if (approval_token != null)
                {
                    var a_token = _transactionTokenService.GetApprovalToken(approval_token.ToString());
                    if (a_token != null && a_token.consumed == 0)
                    {
                        var result = collection["approval"].ToString();
                        if (result == step.Definition.Element("approve_value").Value)
                        {
                            //var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);
                            definition_step.Element("status").SetValue(1);

                            var token_id = _transactionTokenService.AddToken(new bpm_transaction_token
                            {
                                t_code = Util.Encrypt(Util.GenerateTCode()),
                                transaction_id = id,
                                created_by = employee.emp_number
                            });

                            var signature_element = new XElement("signature");
                            signature_element.Add(new XElement("signature_id", token_id));
                            signature_element.Add(new XElement("step", step.Id));
                            signature_element.Add(new XElement("name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
                            signature_element.Add(new XElement("emp_number", employee.emp_number));
                            signature_element.Add(new XElement("created_at", DateTime.Now.ToShortDateString()));
                            definition.Element("form").Element("signatures").Add(signature_element);

                            foreach (var form in definition_step.Element("form_values").Elements("form"))
                            {

                                var form_id = form.Attribute("id").Value;
                                definition_step.Element("form_values").Element("signatures").Add(signature_element);


                            }
                            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
                        }
                        else if (result == "dismissed")
                        {
                            workflow.status = 24;
                        }
                        else
                        {
                            definition.Element("next_step").SetValue(step.Definition.Element("fallback_step").Value);
                        }

                        a_token.consumed = 1;
                        _transactionTokenService.UpdateApprovalToken(a_token);
                    }
                    else
                    {
                        ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                        return RedirectToAction("Step", new { id = id });
                    }
                }
                else
                {
                    ViewBag.message = "Invalid operation. Please review and re submit.";
                    return RedirectToAction("Step", new { id = id });
                }
#endif

#if DEBUG

                        var result = collection["approval"].ToString();
                        if (result == step.Definition.Element("approve_value").Value)
                        {
                            //var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);
                            definition_step.Element("status").SetValue(1);

                            var token_id = _transactionTokenService.AddToken(new bpm_transaction_token
                            {
                                t_code = Util.Encrypt(Util.GenerateTCode()),
                                transaction_id = id,
                                created_by = employee.emp_number
                            });

                            var signature_element = new XElement("signature");
                            signature_element.Add(new XElement("signature_id", token_id));
                            signature_element.Add(new XElement("step", step.Id));
                            signature_element.Add(new XElement("name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
                            signature_element.Add(new XElement("emp_number", employee.emp_number));
                            signature_element.Add(new XElement("created_at", DateTime.Now.ToShortDateString()));
                            definition.Element("form").Element("signatures").Add(signature_element);

                            foreach (var form in definition_step.Element("form_values").Elements("form"))
                            {

                                var form_id = form.Attribute("id").Value;
                                definition_step.Element("form_values").Element("signatures").Add(signature_element);


                            }
                            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
                        }
                        else if (result == "dismissed")
                        {
                            workflow.status = 24;
                        }
                        else
                        {
                            definition.Element("next_step").SetValue(step.Definition.Element("fallback_step").Value);
                        }
#endif


            }
            else if (step.Id == 5)
            {
                definition_step.Element("status").SetValue(1);
                workflow.status = 23;
                //update the detail of the payment to 1
                var payment = _paymentService.GetPayments().FirstOrDefault(e => e.pv_id == workflow.id);
                payment.status = 1;
                _paymentService.Update(payment);
            }

            workflow.workflow = definition.ToString();
            _workflowService.Update(workflow);


            return RedirectToAction("Step", new { id });
        }


        public ActionResult PaymentVoucher(int? page)
        {
            int pageSize = 20;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            return View(_workflowService.GetActionWorkflows().OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize));

        }

        public ActionResult Transactions(int? page, Guid? transaction_id)
        {
            if (transaction_id.HasValue)
            {
                //ViewBag.pvWorkflow = new PVHelper().GetWorkflowPVSummary(transaction_id.Value);
                return View("PV", new PVHelper().GetWorkflowPVSummary(transaction_id.Value));
            }
            int pageSize = 20;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //get all transactions with pv
            var id = Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3179");
            var created_pvs = _paymentService.GetPayments().Select(e => e.workflow_id).ToArray();
            return View(_workflowService.GetActionWorkflows().Where(e => e.status == 23 && !created_pvs.Contains(e.id) && e.process_id != id).OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize));

        }

        [HttpPost]
        public ActionResult CreatePV(bpm_workflow workflow, FormCollection collection)
        {

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            //var process = _processService.GetProcess(workflow.process_id);
            //var process_element = XElement.Parse(process.definition);

           // var code_count = _workflowService.GetWorkflows().Count(e => e.bpm_process.process_code == process.process_code) + 1;

            //process_element.Element("process_id").SetValue(workflow.process_id);
            //process_element.Element("project").SetValue(workflow.project_id);
            //process_element.Element("created_at").SetValue(DateTime.Now);
            //process_element.Element("created_by").SetValue(employee.emp_number);
            //process_element.Element("process_number").SetValue(process.process_code + code_count.ToString("D5"));



            var transaction = _workflowService.GetWorkflow(Guid.Parse(collection["workflow_id"]));

            var process = _processService.GetProcess(Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3179"));
            var process_element = XElement.Parse(process.definition);

            var code_count = _workflowService.GetWorkflows().Count(e => e.bpm_process.process_code == process.process_code) + 1;

            process_element.Element("process_id").SetValue(Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3179"));
            process_element.Element("project").SetValue(transaction.project_id);
            process_element.Add(new XElement("workflow_id", collection["workflow_id"]));
            process_element.Element("created_at").SetValue(DateTime.Now);
            process_element.Element("created_by").SetValue(employee.emp_number);
            process_element.Element("process_number").SetValue(process.process_code + code_count.ToString("D5"));




            workflow.id = Guid.NewGuid();
            workflow.workflow = process_element.ToString();
            workflow.created_at = DateTime.Now;
            workflow.process_id = Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3179");
            workflow.title = "Payment for - " + transaction.title;
            workflow.created_by = employee.emp_number;
            workflow.project_id = transaction.project_id;

            _workflowService.AddWorkflow(workflow);

            var bLWorkflow = new BLWorkflow(workflow.id);

            var wflow = bLWorkflow.GetWorkflow();
            var step = bLWorkflow.GetCurrentStep();

            var step_type = step.Definition.Element("type").Value;
            var definition = bLWorkflow.GetDefinition();
            var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);

            if (step.Id == 1)
            {
                definition.Element("form").Add(new XElement("pay_to", collection["pay_to"].ToString()));
                definition.Element("form").Add(new XElement("pay_by", collection["pay_by"].ToString()));
                definition.Element("form").Add(new XElement("pay_from", collection["pay_from"].ToString()));
                definition.Element("form").Add(new XElement("purpose", collection["purpose"].ToString()));
                definition.Element("form").Add(new XElement("bank_name", collection["bank_name"].ToString()));
                definition.Element("form").Add(new XElement("bank_account", collection["bank_account"].ToString()));
                definition.Element("form").Add(new XElement("account_name", collection["account_name"].ToString()));
                definition.Element("form").Add(new XElement("net_amount", collection["net_amount"].ToString()));
                definition.Element("form").Add(new XElement("withholding_tax", collection["withholding_tax"].ToString()));
                definition.Element("form").Add(new XElement("amount_ngn", collection["amount_ngn"].ToString()));
                definition.Element("form").Add(new XElement("amount_usd", collection["amount_usd"].ToString()));
                definition.Element("form").Add(new XElement("conversion_rate", collection["conversion_rate"].ToString()));



                var token_id = _transactionTokenService.AddToken(new bpm_transaction_token
                {
                    t_code = Util.Encrypt(Util.GenerateTCode()),
                    transaction_id = workflow.id,
                    created_by = employee.emp_number
                });


                var signature_element = new XElement("signature");
                signature_element.Add(new XElement("signature_id", token_id));
                signature_element.Add(new XElement("step", step.Id));
                signature_element.Add(new XElement("name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
                signature_element.Add(new XElement("emp_number", employee.emp_number));
                signature_element.Add(new XElement("created_at", DateTime.Now.ToShortDateString()));
                definition.Element("form").Element("signatures").Add(signature_element);

                //mark step as completed
                definition_step.Element("status").SetValue(1);
                definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
            }

            wflow.workflow = definition.ToString();
            _workflowService.Update(wflow);

            var payment = new bpm_payments
            {
                workflow_id = Guid.Parse(collection["workflow_id"]),
                pv_id = workflow.id,
                status = 0
            };

            _paymentService.AddPayment(payment);

            return RedirectToAction("Step", new { id = workflow.id });
        }
        

        public ActionResult MakePayment(Guid transaction_id)
        {
            var payment = _paymentService.GetPayments().FirstOrDefault(e => e.pv_id == transaction_id);
            payment.status = 2;
            _paymentService.Update(payment);
            return RedirectToAction("CompPV");
        }


    }
}