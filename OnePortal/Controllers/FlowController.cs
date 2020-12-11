using HRM.DAL.IService;
using HRM.DAL.Util;
using Microsoft.AspNet.Identity;
using OnePortal.Helper;
using OnePortal.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WorkFlow.DAL;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.IService;
using WorkFlow.DAL.Logic;

namespace OnePortal.Controllers
{
    public class FlowController : Controller
    {
        IWorkflowService _workflowService;
        IEmployeeService _employeeService;
        IProcessService _processService;
        IVendorService _vendorService;
        ITransactionTokenService _transactionTokenService;

        public FlowController(IProcessService processService, IWorkflowService workflowService, IEmployeeService employeeService, IVendorService vendorService, ITransactionTokenService transactionTokenService)
        {
            _workflowService = workflowService;
            _employeeService = employeeService;
            _processService = processService;
            _vendorService = vendorService;
            _transactionTokenService = transactionTokenService;
        }



            public ActionResult Documents(Guid id)
            {
                return View(Util.GetWorkflowDocument(id));
            }

            public FileResult Document(Guid id)
            {
                var document = Util.GetWorkflowDocument(id);
                //var FileVirtualPath = "~/App_Data/uploads/" + ImageName;
                return File(document.location, "application/force-download", Path.GetFileName(document.location));
            }

            [Authorize(Roles = "procurement")]
            public ActionResult Edit(Guid id)
            {
                var bLWorkflow = new BLWorkflow(id);
                var workflow = bLWorkflow.GetWorkflow();
                if (workflow.process_id.ToString().ToUpper() == "94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317" || workflow.process_id.ToString().ToUpper() == "10471746-0236-4F79-BE27-8AA1CBFF2D92" || workflow.process_id.ToString().ToUpper() == "396E4066-81F1-4311-B076-FC3C53BB3178" || workflow.process_id.ToString().ToUpper() == "2D008467-EA12-4821-A2F2-7568B76039F1")
                {
                    var definition = bLWorkflow.GetDefinition();
                    ViewBag.workflow_id = id;
                    ViewBag.workflow = workflow;


                    return View();
                }
                else
                {
                    return RedirectToAction("Wrequest");
                }

            }

            [HttpPost]
            public ActionResult Edit(FormCollection collection)
            {
                var id = new Guid(collection["workflow_id"]);
                var bLWorkflow = new BLWorkflow(id);

                var workflow = bLWorkflow.GetWorkflow();
                var step = bLWorkflow.GetCurrentStep();


                var definition = bLWorkflow.GetDefinition();
                definition.Element("form").Element("items").Elements("item").Remove();

                //foreach (var item in definition.Element("form").Element("items").Elements("item"))
                //{
                //    var item_id = item.Attribute("id").Value;
                //    item.Element("itemname").SetValue(collection["itemname_" + item_id].ToString());
                //    item.Element("quantity").SetValue(collection["quantity_" + item_id].ToString());
                //    item.Element("unit").SetValue(collection["unit_" + item_id].ToString());
                //    item.Element("description").SetValue(collection["description_" + item_id].ToString());
                //    //update the estimated prices
                //    item.Add(new XElement("unit_price", collection["est_unit_price_" + item_id].ToString()));
                //    item.Add(new XElement("total_price", collection["est_total_price_" + item_id].ToString()));
                //}


                var units = collection["unit"].ToString().Split(',');
                var quantities = collection["quantity"].ToString().Split(',');
                var itemnames = collection["itemname"].ToString().Split(',');
                var descriptions = collection["description"].ToString().Split(',');
                var amounts = collection["unit_price"].ToString().Split(',');

                for (var i = 0; i < units.Count(); i++)
                {
                    var item = new XElement("item");
                    item.Add(new XAttribute("id", i + 1));
                    item.Add(new XElement("unit", units[i]));
                    item.Add(new XElement("quantity", quantities[i]));
                    item.Add(new XElement("itemname", itemnames[i]));
                    item.Add(new XElement("description", descriptions[i]));
                    item.Add(new XElement("unit_price", amounts[i]));
                    item.Add(new XElement("est_unit_price", amounts[i]));

                    definition.Element("form").Element("items").Add(item);
                }


                if (definition.Element("form").Element("conversion_rate") != null)
                {
                    definition.Element("form").Element("conversion_rate").SetValue(collection["conversion_rate"].ToString());
                }
                else
                {
                    definition.Element("form").Add(new XElement("conversion_rate", collection["conversion_rate"].ToString()));
                }

                workflow.workflow = definition.ToString();
                _workflowService.Update(workflow);
                return RedirectToAction("Edit", new { id = id });
            }

            // GET: Workflow
            public ActionResult MyRequest(int? page)
            {
                var user_id = User.Identity.GetUserId();
                var employee = _employeeService.GetEmployeeByUserId(user_id);

                int pageSize = 20;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                var id = Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3179");
                return View(_workflowService.GetUserWorkflows(employee.emp_number).Where(e => e.process_id != id).OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize));
            }

            [HttpPost]
            public ActionResult MyRequest(bpm_workflow workflow)
            {
                var user_id = User.Identity.GetUserId();
                var employee = _employeeService.GetEmployeeByUserId(user_id);

                var process = _processService.GetProcess(workflow.process_id);
                var process_element = XElement.Parse(process.definition);

                var code_count = _workflowService.GetWorkflows().Count(e => e.bpm_process.process_code == process.process_code) + 1;

                process_element.Element("process_id").SetValue(workflow.process_id);
                process_element.Element("project").SetValue(workflow.project_id);
                process_element.Element("created_at").SetValue(DateTime.Now);
                process_element.Element("created_by").SetValue(employee.emp_number);
                process_element.Element("process_number").SetValue(process.process_code + code_count.ToString("D5"));



                workflow.id = Guid.NewGuid();
                workflow.workflow = process_element.ToString();
                workflow.created_at = DateTime.Now;
                workflow.created_by = employee.emp_number;

                _workflowService.AddWorkflow(workflow);

                return RedirectToAction("Step", new { id = workflow.id });
            }

            public ActionResult WRequest(int? page, string search = null)
            {
                var search_query = "";
                if (search != null)
                {
                    search_query = search;
                }
                var user_id = User.Identity.GetUserId();
                var employee = _employeeService.GetEmployeeByUserId(user_id);

                int pageSize = 20;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                ViewBag.page = pageIndex;
                ViewBag.search = search_query;
                if (User.IsInRole("procurement"))
                    return View("Requests", _workflowService.GetActionWorkflows().Where(e => e.status == 22 && (e.title.Contains(search_query) || e.admin_hrm_employee.emp_lastname.Contains(search) || e.admin_hrm_employee.emp_firstname.Contains(search))).OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize));
                else
                    return View(_workflowService.GetActionWorkflows(employee.emp_number).Where(e => e.bpm_workflow.title.Contains(search_query) || e.admin_hrm_employee.emp_lastname.Contains(search) || e.admin_hrm_employee.emp_firstname.Contains(search)).OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize));
            }

            public ActionResult MoveW()
            {
                return View();
            }

            public ActionResult Completed(int? page, string search = null)
            {
                var search_query = "";
                if (search != null)
                {
                    search_query = search;
                }
                var user_id = User.Identity.GetUserId();
                var employee = _employeeService.GetEmployeeByUserId(user_id);

                int pageSize = 20;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                ViewBag.page = pageIndex;
                ViewBag.search = search_query;

                return View(_workflowService.GetActionWorkflows().Where(e => e.status == 23 && (e.title.Contains(search_query) || e.admin_hrm_employee.emp_lastname.Contains(search) || e.admin_hrm_employee.emp_firstname.Contains(search))).OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize));

            }

            public ActionResult ARequests(int? page, string search = null)
            {
                var search_query = "";
                if (search != null)
                {
                    search_query = search;
                }
                var user_id = User.Identity.GetUserId();
                var employee = _employeeService.GetEmployeeByUserId(user_id);

                int pageSize = 20;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                ViewBag.page = pageIndex;
                ViewBag.search = search_query;
                return View(_workflowService.GetActionWorkflows().Where(e => e.title.Contains(search_query) || e.admin_hrm_employee.emp_lastname.Contains(search) || e.admin_hrm_employee.emp_firstname.Contains(search)).OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize));
            }

            public ActionResult PendingRequests(int? page, string search = null)
            {
                var search_query = "";
                if (search != null)
                {
                    search_query = search;
                }
                var user_id = User.Identity.GetUserId();
                var employee = _employeeService.GetEmployeeByUserId(user_id);

                int pageSize = 20;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                ViewBag.page = pageIndex;
                ViewBag.search = search_query;
                return View(_workflowService.GetActionWorkflows().Where(e => e.title.Contains(search_query) && e.status == 22).OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize));

            }

            public ActionResult PendingProcurementRequests(int? page, string search = null)
            {
                var search_query = "";
                if (search != null)
                {
                    search_query = search;
                }
                var user_id = User.Identity.GetUserId();
                var employee = _employeeService.GetEmployeeByUserId(user_id);

                int pageSize = 20;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                ViewBag.page = pageIndex;
                ViewBag.search = search_query;
                var procurementProcesses = new List<Guid>();
                procurementProcesses.Add(Guid.Parse("94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317"));
                procurementProcesses.Add(Guid.Parse("2D008467-EA12-4821-A2F2-7568B76039F1"));
                procurementProcesses.Add(Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3178"));
                return View(_workflowService.GetActionWorkflows().Where(e => e.title.Contains(search_query) && e.status == 22 && procurementProcesses.Contains(e.process_id)).OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize));
            }

            public ActionResult MyPendingApproval(int? page, string search = null)
            {
                var search_query = "";
                if (search != null)
                {
                    search_query = search;
                }
                var user_id = User.Identity.GetUserId();
                var employee = _employeeService.GetEmployeeByUserId(user_id);

                int pageSize = 20;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                ViewBag.page = pageIndex;
                ViewBag.search = search_query;
                return View(_workflowService.GetActionWorkflows(employee.emp_number).Where(e => e.bpm_workflow.title.Contains(search_query)).OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize));
            }

            public ActionResult Step(Guid id, int? step_id, string message = null)
            {
                var bLWorkflow = new BLWorkflow(id);
                var definition = bLWorkflow.GetDefinition();
                var user_id = User.Identity.GetUserId();
                var employee = _employeeService.GetEmployeeByUserId(user_id);
                ViewBag.message = message;
                ViewBag.workflow_id = id;
                ViewBag.workflow = bLWorkflow.GetWorkflow();
                ViewBag.definition = definition;
                var workflow = bLWorkflow.GetWorkflow();

                if (definition.Element("next_step").Value == "-1")
                {
                    if (employee.emp_number == Util.GetProgramApprover(-1, 0))
                    {
                        return View();
                    }
                    else
                    {
                        return View("Flow");
                    }

                }
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
                        var project_id = workflow.project_id;
                        var project = OptionUtil.GetProjects().FirstOrDefault(e => e.id == workflow.project_id);
                        if (project != null && project.sub_parent.HasValue)
                        {
                            project_id = project.sub_parent.Value;
                        }
                        var project_d = Util.GetProgramApprover(project_id, 0);


                        var emps = _employeeService.GetEmployee(project_d);

                        if (emps == employee)
                        {
                            user_can_perform_this_action = true;
                        }


                        break;
                    case "emp":

                        //get the subunit of the requestor
                        var emp = _employeeService.GetEmployee(workflow.created_by);
                        if (position.Value == "0")
                        {
                            if (emp == employee)
                            {
                                user_can_perform_this_action = true;
                            }
                        }
                        else
                        {
                            emp = _employeeService.GetEmployee(Convert.ToInt32(position.Value));
                            if (emp == employee)
                            {
                                user_can_perform_this_action = true;
                            }
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
                //System.IO.File.Copy(Server.MapPath("~/Documents/Template/PRF.pdf"), Server.MapPath("~/Documents/Procurement/Quotations/" + workflow.id + "_PRF" + ".pdf"),true);
                //new WorkflowDocumentsHelper(_transactionTokenService).createPRF(Server.MapPath("~/Documents/Procurement/Quotations/" + workflow.id + "_PRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/prf_header.png"));

                ////get all the form values in display form
                //var input_forms = step.Definition.Element("form_values").Elements("form");
                ////check if the step will output data on the form
                //if (step.Definition.Element("display_values") != null)
                //{
                //    //look through the forms for display 
                //    foreach (var form in step.Definition.Element("display_values").Elements("form"))
                //    {
                //        var form_id = form.Attribute("id").Value;
                //        var form_definition = StepHelper.GenerateStepDefinition(definition.Element("forms").Elements("form").FirstOrDefault(e => e.Attribute("id").Value == form_id), form, input_forms.FirstOrDefault(e => e.Attribute("id").Value == form_id), form_id);

                //        form_view_data.Add(form_id, StepHelper.GenerateForm(form_definition));
                //    }
                //}
                //ViewBag.form_view_data = form_view_data;
                return View();
            }

            public ActionResult FkGenerate(Guid id)
            {
                var bLWorkflow = new BLWorkflow(id);

                var definition = bLWorkflow.GetDefinition();

                var workflow = bLWorkflow.GetWorkflow();

                new WorkflowDocumentsHelper().createPRF(Server.MapPath("~/Documents/Procurement/PRF/" + workflow.id + "_PRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/prf_header.png"), definition);

                //new WorkflowDocumentsHelper().createPurchaseOrder(Server.MapPath("~/Documents/Procurement/PO/" + workflow.id + "_PO" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/po_header.png"));

                //new WorkflowDocumentsHelper().createVPRF(Server.MapPath("~/Documents/Procurement/VPRF/" + workflow.id + "_VPRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/vprf_header.png"));

                return View();
            }

            [HttpPost]
            [ValidateInput(false)]
            public ActionResult Step(FormCollection collection, HttpPostedFileBase[] files, string[] itemname, string[] description)
            {
                var user_id = User.Identity.GetUserId();
                var employee = _employeeService.GetEmployeeByUserId(user_id);
                int complete = 0;

                var id = new Guid(collection["workflow_id"]);
                var bLWorkflow = new BLWorkflow(id);

                var workflow = bLWorkflow.GetWorkflow();
                var step = bLWorkflow.GetCurrentStep();


                var definition = bLWorkflow.GetDefinition();

                var skip = false;

                XElement definition_step = null;

                if (definition.Element("next_step").Value == "-1" && workflow.process_id.ToString().ToUpper() == "94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317" && employee.emp_number == Util.GetProgramApprover(-1, 0))
                {
                    //var project_d = Util.GetProgramApprover(project_id, 0);
                    if (collection["wf_assignment"] != null && collection["wf_assignment"] == "emp")
                    {
                        //definition.Add(new XElement("actual_step", 5));
                        //step = bLWorkflow.GetStep(Convert.ToInt32(definition.Element("actual_step").Value));
                        new Procurement().AddUserToSteps(new int[] { 5, 8, 9, 10, 11, 12, 17, 18, 21 }, definition, Convert.ToInt32(collection["emp_number"]), 5);
                        step = bLWorkflow.GetStep(5);
                        definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "5");
                        skip = true;
                    }
                }
                else if (workflow.process_id.ToString().ToUpper() == "2D008467-EA12-4821-A2F2-7568B76039F1" && definition.Element("next_step").Value == "-1" && employee.emp_number == Util.GetProgramApprover(-1, 0))
                {
                    if (collection["wf_assignment"] != null && collection["wf_assignment"] == "emp")
                    {
                        //step = bLWorkflow.GetStep(Convert.ToInt32(definition.Element("actual_step").Value));
                        new Procurement().AddUserToSteps(new int[] { 6, 9, 10, 11, 12, 13, 18, 19, 22 }, definition, Convert.ToInt32(collection["emp_number"]), 6);
                        step = bLWorkflow.GetStep(6);
                        definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "6");
                        skip = true;
                    }
                }

                if (!skip)
                {
                    var step_type = step.Definition.Element("type").Value;
                    definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);

                    var user_can_perform_this_action = false;
                    //check if the user is permitted to perform this action
                    //get the position
#if !DEBUG
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
                            var project_id = workflow.project_id;
                            var project = OptionUtil.GetProjects().FirstOrDefault(e => e.id == workflow.project_id);
                            if (project != null && project.sub_parent.HasValue)
                            {
                                project_id = project.sub_parent.Value;
                            }
                            var project_d = Util.GetProgramApprover(project_id, 0);

                            var emps = _employeeService.GetEmployee(project_d);

                            //var directorate = _employeeService.GetDirectorate(workflow.created_by);
                            if (emps == employee)
                            {
                                user_can_perform_this_action = true;
                            }

                            break;
                        case "emp":
                            //get the subunit of the requestor
                            var emp = _employeeService.GetEmployee(workflow.created_by);
                            //if (emp == employee)
                            //{
                            //    user_can_perform_this_action = true;
                            //}
                            if (position.Value == "0")
                            {
                                if (emp == employee)
                                {
                                    user_can_perform_this_action = true;
                                }
                            }
                            else
                            {
                                emp = _employeeService.GetEmployee(Convert.ToInt32(position.Value));
                                if (emp == employee)
                                {
                                    user_can_perform_this_action = true;
                                }
                            }


                            break;
                        default:
                            break;
                    }


                }
#endif
                }//if (!user_can_perform_this_action)
                 //{
                 //    ViewBag.message = "You need an elevated access to proceed.";
                 //    return RedirectToAction("AccessDenied", "Home");
                 //}
                 /*
                  * Edited by Johnbosco
                  * Check for token generation before processing step
                  * Process all workflow processes individually
                  */
                if (collection["refresh_token"] != null)
                {
                    RefreshToken(employee, collection, id);

                    return RedirectToAction("Step", new { id });
                }
                //if (collection["refresh_token"] != null)
                //{
                //    var refresh = collection["refresh_token"].Split(',');;
                //    if (refresh[0] == "refresh")
                //    {

                //        var e_body = "";

                //        string dm = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                //        dm += "/workflow/step/?id=" + id;

                //        e_body = string.Format("Your action is needed for the completion of this process <strong>{0}</strong>.<br/>", workflow.title);
                //        e_body += string.Format(" <a href='{0}'>Click here </a> to view", dm);

                //        var n_email = new Email
                //        {

                //            body = e_body,
                //            subject = "Action required - Workspace"
                //        };
                //        n_email.to = new List<string> { employee.emp_work_email };
                //        n_email.IsHtml = true;

                //        var approval_token = Util.GenerateTCode();
                //        _transactionTokenService.AddApprovalToken(new bpm_approval_token { token = approval_token });

                //        e_body = string.Format("Your approval is needed for the completion of this process <strong>{0}</strong>.<br/>", workflow.title);
                //        e_body += string.Format("Your approval code is <code>{0}</code><br/><br/>", approval_token);
                //        e_body += "Note that this code will expire in 24hrs. Thanks";


                //        e_body += string.Format(" <a href='{0}'>Click here </a> to view", dm);

                //        n_email.subject = "Approval required - Workspace";
                //        n_email.body = e_body;

                //        n_email.to = new List<string> { employee.emp_work_email };

                //        NotificationUtil.SendEmail(n_email);



                //        return RedirectToAction("Step", new { id = id });
                //    }

                //}

                if (!skip)
                {
                    //procurement
                    if (workflow.process_id.ToString().ToUpper() == "94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317" || workflow.process_id.ToString().ToUpper() == "10471746-0236-4F79-BE27-8AA1CBFF2D92" || workflow.process_id.ToString().ToUpper() == "396E4066-81F1-4311-B076-FC3C53BB3178")
                    {
                        var procurement = new Procurement();
                        if (step.Id == 1)
                        {
                            procurement.AddItemsToProcure(definition, step, definition_step, collection, employee, id);
                            //upload supporting document

                            if (Request.Files.Count > 0)
                            {
                                try
                                {
                                    var file_path = "~/Documents/Procurement/SupportDoc/";
                                    //var files = Request.Files;
                                    foreach (HttpPostedFileBase file in files)
                                    {
                                        var location = Path.Combine(file_path, workflow.title.Replace(@"\", "").Replace(@"/", "") + "_" + file.FileName.Replace(@"\", string.Empty));
                                        if (!Directory.Exists(Server.MapPath(file_path)))
                                        {
                                            Directory.CreateDirectory(Server.MapPath(file_path));
                                        }
                                        file.SaveAs(Server.MapPath(Path.Combine(location)));

                                        Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = file.FileName, workflow_id = workflow.id });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string message = string.Format("{0}",
                                                ex.Message);
                                    Utils.Log(message + " - AddUserToWorkflow");
                                    // Exception raise = ex;


                                }


                            }
                        }
                        //approvals
                        else if (step.Id == 2 || step.Id == 3 || step.Id == 4 || step.Id == 6 || step.Id == 7 || step.Id == 13 || step.Id == 14 || step.Id == 15 || step.Id == 16 || step.Id == 19 || step.Id == 20)
                        {
                            var result = procurement.Approve(definition, step, definition_step, collection, employee, id, workflow);
                            if (result != "success")
                            {
                                ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                                return RedirectToAction("Step", new { id = workflow.id, message = result });
                            }

                            if (step.Id == 4 && workflow.process_id.ToString().ToUpper() == "94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317")
                            {
                                if (definition.Element("actual_step") == null)
                                {
                                    definition.Add(new XElement("actual_step", 5));
                                }
                                else
                                {
                                    definition.Element("actual_step").SetValue(5);
                                }


                                definition.Element("next_step").SetValue("-1");
                            }
                        }
                        //update cost
                        else if (step.Id == 5)
                        {

                            procurement.UpdateCost(definition, step, collection, definition_step);

                        }
                        //request for quotations
                        else if (step.Id == 8)
                        {
                            //generate the PRF
                            //WorkflowDocumentsHelper.createPRF("~/Documents/Procurement/Quotations/" + workflow.id + "_PRF" + ".pdf", workflow.pm_project.code, definition.Element("form"));
                            RFQ(workflow, collection, definition, employee, step, definition_step);


                        }
                        //upload quotations
                        else if (step.Id == 9)
                        {

                            UploadQuotes(workflow, collection, definition, step, definition_step);

                        }
                        //bid comparison
                        else if (step.Id == 10)
                        {
                            BidComparison(workflow, collection, definition, employee, step, definition_step);
                            //var vendor_id = collection["vendor"].ToString();



                        }
                        //upload HQ approval
                        else if (step.Id == 11)
                        {
                            var result = HQApproval(workflow, collection, definition, step, definition_step, employee);
                            if (result != "success")
                            {
                                ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                                return RedirectToAction("Step", new { id = workflow.id, message = result });
                            }
                        }
                        //purchase order
                        else if (step.Id == 12)
                        {
                            procurement.CreatePO(definition, step, collection, definition_step);


                        }
                        //job completion 16
                        else if (step.Id == 17)
                        {
                            JobCompletion(workflow, collection, definition, step, definition_step, files);

                        }
                        //17
                        else if (step.Id == 18)
                        {
                            procurement.CreateVPRF(definition, step, definition_step, collection);


                        }
                        if (step.Id == 21 || step.Id == 20)
                        {
                            definition_step.Element("status").SetValue(1);
                            workflow.status = 23;
                            complete = 1;
                        }



                    }
                    //procurement NAIIS A9BE0559-94E4-4A6E-AE46-74A1344DF7EF
                    else if (workflow.process_id.ToString().ToUpper() == "2D008467-EA12-4821-A2F2-7568B76039F1")
                    {
                        if (collection["wf_assignment"] != null && collection["wf_assignment"] == "emp")
                        {
                            // new Procurement().AddUserToSteps(new int[] { 6, 9, 10, 11, 12, 13, 14, 15 }, definition, Convert.ToInt32(collection["emp_number"]), 6);
                        }

                        var procurement = new Procurement();
                        if (step.Id == 1)
                        {
                            procurement.AddItemsToProcure(definition, step, definition_step, collection, employee, id);

                            if (Request.Files.Count > 0)
                            {
                                try
                                {
                                    var file_path = "~/Documents/Procurement/SupportDoc/";
                                    //var files = Request.Files;
                                    foreach (HttpPostedFileBase file in files)
                                    {
                                        var location = Path.Combine(file_path, workflow.title + "_" + file.FileName.Replace(@"\", string.Empty));
                                        if (!Directory.Exists(Server.MapPath(file_path)))
                                        {
                                            Directory.CreateDirectory(Server.MapPath(file_path));
                                        }
                                        file.SaveAs(Server.MapPath(Path.Combine(location)));

                                        Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = file.FileName, workflow_id = workflow.id });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string message = string.Format("{0}",
                                                ex.Message);
                                    Utils.Log(message + " - AddUserToWorkflow");
                                    //Exception raise = ex;


                                }


                            }
                        }
                        //approvals
                        else if (step.Id == 2 || step.Id == 3 || step.Id == 4 || step.Id == 5 || step.Id == 7 || step.Id == 8 || step.Id == 14 || step.Id == 15 || step.Id == 16 || step.Id == 17 || step.Id == 20 || step.Id == 21)
                        {
                            var result = procurement.Approve(definition, step, definition_step, collection, employee, id, workflow);
                            if (result != "success")
                            {
                                ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                                return RedirectToAction("Step", new { id = workflow.id, message = result });
                            }

                            if (step.Id == 5)
                            {
                                if (definition.Element("actual_step") == null)
                                {
                                    definition.Add(new XElement("actual_step", 5));
                                }
                                else
                                {
                                    definition.Element("actual_step").SetValue(5);
                                }


                                definition.Element("next_step").SetValue("-1");
                            }
                        }
                        //update cost
                        else if (step.Id == 6)
                        {

                            procurement.UpdateCost(definition, step, collection, definition_step);

                        }
                        //request for quotations
                        else if (step.Id == 9)
                        {
                            //generate the PRF
                            //WorkflowDocumentsHelper.createPRF("~/Documents/Procurement/Quotations/" + workflow.id + "_PRF" + ".pdf", workflow.pm_project.code, definition.Element("form"));
                            RFQ(workflow, collection, definition, employee, step, definition_step);


                        }
                        //upload quotations
                        else if (step.Id == 10)
                        {

                            UploadQuotes(workflow, collection, definition, step, definition_step);

                        }
                        //bid comparison
                        else if (step.Id == 11)
                        {
                            BidComparison(workflow, collection, definition, employee, step, definition_step);
                            //var vendor_id = collection["vendor"].ToString();



                        }
                        //upload HQ approval
                        else if (step.Id == 12)
                        {
                            var result = HQApproval(workflow, collection, definition, step, definition_step, employee);
                            if (result != "success")
                            {
                                ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                                return RedirectToAction("Step", new { id = workflow.id, message = result });
                            }
                        }
                        //purchase order
                        else if (step.Id == 13)
                        {
                            procurement.CreatePO(definition, step, collection, definition_step);


                        }
                        //job completion 16
                        else if (step.Id == 18)
                        {
                            JobCompletion(workflow, collection, definition, step, definition_step, files);

                        }
                        //17
                        else if (step.Id == 19)
                        {
                            procurement.CreateVPRF(definition, step, definition_step, collection);


                        }
                        if (step.Id == 22 || step.Id == 21)
                        {
                            definition_step.Element("status").SetValue(1);
                            workflow.status = 23;
                            complete = 1;
                        }

                    }
                    else if (workflow.process_id.ToString().ToUpper() == "A9BE0559-94E4-4A6E-AE46-74A1344DF7DF" || workflow.process_id.ToString().ToUpper() == "A9BE0559-94E4-4A6E-AE46-74A1344DF7EF")
                    {
                        //travel authorization
                        if (step.Id == 1)
                        {
                            definition.Element("form").Add(new XElement("departure", collection["departure"].ToString()));
                            definition.Element("form").Add(new XElement("destination", collection["destination"].ToString()));
                            definition.Element("form").Add(new XElement("travel_class", collection["travel_class"].ToString()));
                            definition.Element("form").Add(new XElement("start_date", collection["start_date"].ToString()));
                            definition.Element("form").Add(new XElement("end_date", collection["end_date"].ToString()));
                            definition.Element("form").Add(new XElement("purpose_of_travel", collection["purpose_of_travel"].ToString()));


                            var second_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "2");
                            second_step.Element("next_step_position_id").SetValue(employee.emp_number);
                            var third_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "3");
                            third_step.Element("position").SetValue(employee.emp_number);

                            var token_id = _transactionTokenService.AddToken(new bpm_transaction_token
                            {
                                t_code = Util.Encrypt(Util.GenerateTCode()),
                                transaction_id = id,
                                created_by = employee.emp_number
                            });

                            //var form_id = form.Attribute("id").Value;
                            if (definition.Element("form").Element("signatures") == null)
                            {
                                definition.Element("form").Add(new XElement("signatures"));
                            }

                            var signature_element = new XElement("signature");
                            signature_element.Add(new XElement("signature_id", token_id));
                            signature_element.Add(new XElement("step", step.Id));
                            signature_element.Add(new XElement("name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
                            signature_element.Add(new XElement("emp_number", employee.emp_number));
                            signature_element.Add(new XElement("created_at", DateTime.Now.ToShortDateString()));
                            definition.Element("form").Element("signatures").Add(signature_element);

                            if (Request.Files.Count > 0)
                            {
                                try
                                {
                                    var file_path = "~/Documents/Travel/SupportDocs/";
                                    //var files = Request.Files;
                                    foreach (HttpPostedFileBase file in files)
                                    {
                                        var location = Path.Combine(file_path, workflow.title + "_" + file.FileName.Replace(@"\", string.Empty));
                                        if (!Directory.Exists(Server.MapPath(file_path)))
                                        {
                                            Directory.CreateDirectory(Server.MapPath(file_path));
                                        }
                                        file.SaveAs(Server.MapPath(Path.Combine(location)));

                                        Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = file.FileName, workflow_id = workflow.id });
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }

                            //mark step as completed
                            definition_step.Element("status").SetValue(1);
                            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
                        }
                        else if (step.Id == 3)
                        {
                            //get the type of travel
                            var travel_expenses = Util.GetTravelExpenses(Convert.ToInt32(definition.Element("form").Element("travel_class").Value));
                            if (travel_expenses.Any())
                            {
                                if (definition.Element("form").Element("expenses") == null)
                                {
                                    definition.Element("form").Add(new XElement("expenses"));
                                }
                                foreach (var expense in travel_expenses)
                                {
                                    //check if the dates are null
                                    if ((collection[expense.expense_id + "_start_date"] != null && collection[expense.expense_id + "_start_date"].ToString() != "") && (collection[expense.expense_id + "_end_date"] != null && collection[expense.expense_id + "_end_date"].ToString() != "") && (collection[expense.expense_id + "_no_of_days"] != null && collection[expense.expense_id + "_no_of_days"].ToString() != ""))
                                    {
                                        var element = new XElement("expense");
                                        element.Add(new XElement("id", expense.expense_id));
                                        element.Add(new XElement("name", expense.bpm_travel_expenses.name));
                                        element.Add(new XElement("rate", expense.amount));
                                        element.Add(new XElement("start_date", collection[expense.expense_id + "_start_date"].ToString()));
                                        element.Add(new XElement("end_date", collection[expense.expense_id + "_end_date"].ToString()));
                                        element.Add(new XElement("no_of_days", collection[expense.expense_id + "_no_of_days"].ToString()));
                                        definition.Element("form").Element("expenses").Add(element);
                                    }


                                }
                            }
                            //check for hotel
                            if (collection["hotel"] != null)
                            {
                                if ((collection["h_start_date"] != null && collection["h_start_date"].ToString() != "") && (collection["h_end_date"] != null && collection["h_end_date"].ToString() != "") & (collection["h_no_of_days"] != null && collection["h_no_of_days"].ToString() != ""))
                                {
                                    var element = new XElement("expense");
                                    element.Add(new XElement("name", "Hotel"));
                                    element.Add(new XElement("rate", collection["hotel"].ToString()));
                                    element.Add(new XElement("start_date", collection["h_start_date"].ToString()));
                                    element.Add(new XElement("end_date", collection["h_end_date"].ToString()));
                                    element.Add(new XElement("no_of_days", collection["h_no_of_days"].ToString()));
                                    definition.Element("form").Element("expenses").Add(element);
                                }
                            }

                            //check for hotel
                            if (collection["transportation"] != null && collection["transportation"].ToString() != "")
                            {
                                if (collection["t_start_date"] != null && collection["t_end_date"] != null && collection["t_no_of_days"] != null)
                                {
                                    var element = new XElement("expense");
                                    element.Add(new XElement("name", "Transportation"));
                                    element.Add(new XElement("rate", collection["transportation"].ToString()));
                                    element.Add(new XElement("start_date", collection["t_start_date"].ToString()));
                                    element.Add(new XElement("end_date", collection["t_end_date"].ToString()));
                                    element.Add(new XElement("no_of_days", collection["t_no_of_days"].ToString()));
                                    definition.Element("form").Element("expenses").Add(element);
                                }
                            }

                            //check for other expenses
                            if ((collection["other"] != null && collection["other"].ToString() != "") && (collection["other_amount"] != null && collection["other_amount"].ToString() != ""))
                            {
                                var others = collection["other"].ToString().Split(',');
                                var other_amounts = collection["other_amount"].ToString().Split(',');
                                //var other_starts= collection["o_start_date"].ToString().Split(',');
                                //var other_ends = collection["o_end_date"].ToString().Split(',');
                                if (others.Count() == other_amounts.Count())
                                {
                                    for (var i = 0; i < others.Count(); i++)
                                    {
                                        if (others[i].Trim() == "") continue;
                                        var element = new XElement("expense");
                                        //element.Add(new XElement("id", i+1));
                                        element.Add(new XElement("name", others[i]));
                                        element.Add(new XElement("rate", other_amounts[i]));
                                        if (collection["o_start_date"] != null)
                                        {
                                            var other_starts = collection["o_start_date"].ToString().Split(',');
                                            element.Add(new XElement("start_date", other_starts[i]));
                                        }
                                        else
                                        {
                                            element.Add(new XElement("start_date", ""));
                                        }

                                        if (collection["o_start_date"] != null)
                                        {
                                            var other_ends = collection["o_end_date"].ToString().Split(',');
                                            element.Add(new XElement("end_date", other_ends[i]));
                                        }
                                        else
                                        {
                                            element.Add(new XElement("end_date", ""));
                                        }

                                        if (collection["o_no_of_days"] != null)
                                        {
                                            var days = collection["o_no_of_days"].ToString().Split(',');
                                            element.Add(new XElement("no_of_days", days[i]));
                                        }
                                        else
                                        {
                                            element.Add(new XElement("no_of_days", "1"));
                                        }

                                        definition.Element("form").Element("expenses").Add(element);
                                    }
                                }
                            }


                            definition.Element("form").Add(new XElement("account_name", collection["account_name"].ToString()));
                            definition.Element("form").Add(new XElement("account_number", collection["account_number"].ToString()));
                            definition.Element("form").Add(new XElement("bank_name", collection["bank_name"].ToString()));

                            //mark step as completed
                            definition_step.Element("status").SetValue(1);
                            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);

                        }

                        else if (step.Id == 2 || step.Id == 4 || step.Id == 5)
                        {
                            var result = new Procurement().Approve(definition, step, definition_step, collection, employee, id, workflow);
                            if (result != "success")
                            {
                                ViewBag.message = result;
                                return RedirectToAction("Step", new { id = workflow.id, message = result });
                            }


                        }
                        if (step.Id == 6 || step.Id == 5)
                        {
                            definition_step.Element("status").SetValue(1);
                            workflow.status = 23;
                            complete = 1;
                        }
                    }
                    else if (workflow.process_id.ToString().ToUpper() == "84E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317")
                    {
                        var procurement = new Procurement();
                        if (step.Id == 1)
                        {
                            procurement.AddItemsToPayment(definition, step, definition_step, collection, employee, id);
                            //upload supporting document

                            if (files.Count() > 0)
                            {
                                try
                                {
                                    var file_path = "~/Documents/Procurement_pay/SupportDoc/";
                                    //var files = Request.Files;
                                    foreach (HttpPostedFileBase file in files)
                                    {
                                        var location = Path.Combine(file_path, workflow.title + "_" + file.FileName.Replace(@"\", string.Empty));
                                        if (!Directory.Exists(Server.MapPath(file_path)))
                                        {
                                            Directory.CreateDirectory(Server.MapPath(file_path));
                                        }
                                        file.SaveAs(Server.MapPath(location));

                                        Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = "Supporting Document", workflow_id = workflow.id });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string message = string.Format("{0}",
                                                ex.Message);
                                    Utils.Log(message + " - Upload file step 1");
                                    // Exception raise = ex;


                                }


                            }
                        }
                        else if (step.Id == 2 || step.Id == 3 || step.Id == 4 || step.Id == 5)
                        {
                            var result = procurement.Approve(definition, step, definition_step, collection, employee, id, workflow);
                            if (result != "success")
                            {
                                ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                                return RedirectToAction("Step", new { id = workflow.id, message = result });
                            }
                        }

                        if (step.Id == 6 || step.Id == 5)
                        {
                            definition_step.Element("status").SetValue(1);
                            workflow.status = 23;
                            complete = 1;
                        }
                    }

                    else if (workflow.process_id.ToString().ToUpper() == "B9BE0559-94E4-4A6E-AE46-74A1344DF7E7" || workflow.process_id.ToString().ToUpper() == "34F7BC4E-6BF4-4386-A8AE-CDAC77CF3AD1")
                    {
                        var procurement = new Procurement();
                        if (step.Id == 1)
                        {
                            procurement.AddItemsToPayment(definition, step, definition_step, collection, employee, id);
                            //upload supporting document

                            if (files.Count() > 0)
                            {
                                try
                                {
                                    var file_path = "~/Documents/Staff_pay/SupportDoc/";
                                    //var files = Request.Files;
                                    foreach (HttpPostedFileBase file in files)
                                    {
                                        var location = Path.Combine(file_path, workflow.title + "_" + file.FileName.Replace(@"\", string.Empty));
                                        if (!Directory.Exists(Server.MapPath(file_path)))
                                        {
                                            Directory.CreateDirectory(Server.MapPath(file_path));
                                        }
                                        file.SaveAs(Server.MapPath(location));

                                        Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = "Supporting Document", workflow_id = workflow.id });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string message = string.Format("{0}",
                                                ex.Message);
                                    Utils.Log(message + " - Upload file step 1");
                                    // Exception raise = ex;


                                }


                            }
                        }
                        else if (step.Id == 2 || step.Id == 3 || step.Id == 4 || step.Id == 5 || step.Id == 6)
                        {
                            var result = procurement.Approve(definition, step, definition_step, collection, employee, id, workflow);
                            if (result != "success")
                            {
                                ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                                return RedirectToAction("Step", new { id = workflow.id, message = result });
                            }
                        }

                        if (step.Id == 6 || step.Id == 7)
                        {
                            definition_step.Element("status").SetValue(1);
                            workflow.status = 23;
                            complete = 1;
                        }
                    }
                    //new procurement
                    if (workflow.process_id.ToString().ToUpper().Equals("C7902F4A-36DD-4FDC-9B0C-B7059034010D"))
                    {//definition is the whole xml definitin for the current process
                        NewProcurementProcess2(employee, step, definition, collection, files);
                        if (ViewBag.message != "" && ViewBag.message != null)
                        {
                            return RedirectToAction("Step", new { id = id, message = ViewBag.message });
                        }
                        return RedirectToAction("Step", new { id = id });
                    }
                }

                if (collection["comment"] != null)
                {
                    //add comment to the particular step
                    definition_step.Element("comments").SetValue(collection["comment"].ToString());

                    //add comment to the whole request document
                    var comment = new XElement("comment");
                    comment.Add(new XElement("step", step.Id));
                    comment.Add(new XElement("employee_id", employee.emp_number));
                    comment.Add(new XElement("info", collection["comment"].ToString()));
                    comment.Add(new XElement("employee_name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
                    comment.Add(new XElement("created_at", DateTime.Now));

                    definition.Element("comments").Add(comment);
                }

                //set the user that completed the step
                definition_step.Element("created_at").SetValue(DateTime.Now);
                definition_step.Element("created_by").SetValue(employee.emp_number);
                definition_step.Element("created_by_name").SetValue(string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname));

                workflow.workflow = definition.ToString();
                _workflowService.Update(workflow);

                if (complete == 1)
                {
                    generate_doc(id);
                }
                var next_position = definition.Element("next_step");
                var nxt_step = bLWorkflow.GetStep(Convert.ToInt32(next_position.Value));

                var email_body = "";

                string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                Domain += "/workflow/step/?id=" + id;

                email_body = string.Format("Your action is needed for the completion of this process <strong>{0}</strong>.<br/>", workflow.title);
                email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

                var notification_email = new Email
                {

                    body = email_body,
                    subject = workflow.title + " - Action required - Workspace"
                };
                notification_email.to = new List<string> { employee.emp_work_email };
                notification_email.IsHtml = true;

                if (next_position.Value == "-1")
                {
                    var emp_id = Util.GetProgramApprover(-1, 0);
                    var ap_emp = _employeeService.GetEmployee(emp_id);
                    notification_email.to = new List<string> { ap_emp.emp_work_email };
                    _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = emp_id, workflow_id = id, pending = 1, created_at = DateTime.Now });
                }
                else
                {
                    if (nxt_step.Definition.Element("type").Value.ToString() == "approval")
                    {
                        var approval_token = Util.GenerateTCode();
                        _transactionTokenService.AddApprovalToken(new bpm_approval_token { token = approval_token });

                        email_body = string.Format("Your approval is needed for the completion of this process <strong>{0}</strong>.<br/>", workflow.title);
                        email_body += string.Format("Your approval code is <code>{0}</code><br/><br/>", approval_token);
                        email_body += "Note that this code will expire in 24hrs. Thanks";


                        email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

                        notification_email.subject = workflow.title + " Approval - Workspace";
                        notification_email.body = email_body;
                        notification_email.to = new List<string> { employee.emp_work_email };
                    }

#if !DEBUG

                switch (nxt_step.Definition.Element("position").Attribute("pos_type").Value)
                {
                    case "sup":
                        //get the supervisor of the person that applied for the request
                        var requestor = _employeeService.GetEmployee(workflow.created_by);
                        var supervisors = requestor.admin_hrm_emp_reportto1.Select(e => e.erep_sup_emp_number).ToArray();

                        notification_email.to = new List<string> { requestor.admin_hrm_emp_reportto1.FirstOrDefault().admin_hrm_employee.emp_work_email };

                        // NotificationUtil.SendNotifications(supervisors, "Workflow - Action needed");
                        foreach (var sup in supervisors)
                        {
                            _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = sup, workflow_id = id, pending = 1, created_at = DateTime.Now });
                        }

                        break;
                    case "jti":
                        var job_title_id = Convert.ToInt32(step.Definition.Element("next_step_position_id").Value);

                        var employees = _employeeService.GetEmployees().Where(e => e.admin_hrm_emp_job_record.job_title_id == job_title_id);
                        var ids = employees.Select(e => e.emp_number).ToArray();

                        var mail_list = new List<string>();

                        //NotificationUtil.SendNotifications(ids, "Workflow - Action needed");

                        foreach (var emp in employees)
                        {
                            _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = emp.emp_number, workflow_id = id, pending = 1, created_at = DateTime.Now });
                            mail_list.Add(emp.emp_work_email);
                        }

                        notification_email.to = mail_list;

                        break;
                    case "dir":
                        //get the director of the selected grant
                        //var directorate = _employeeService.GetDirectorate(workflow.created_by);
                        var project_id = workflow.project_id;
                        var project = OptionUtil.GetProjects().FirstOrDefault(e => e.id == workflow.project_id);
                        if (project != null && project.sub_parent.HasValue)
                        {
                            project_id = project.sub_parent.Value;
                        }
                        var project_d = Util.GetProgramApprover(project_id, 0);


                        var emps = _employeeService.GetEmployee(project_d);///.Where(e => e.admin_hrm_emp_job_record.job_title_id == directorate.head);


                        var list = new List<string>();

                        //NotificationUtil.SendNotifications(new int[] { project_d }, "Workflow - Action needed");
                        _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = emps.emp_number, workflow_id = id, pending = 1, created_at = DateTime.Now });
                        list.Add(emps.emp_work_email);

                        notification_email.to = list;
                        break;
                    case "emp":
                        //get the subunit of the requestor
                        var empl = _employeeService.GetEmployee(workflow.created_by);

                        if (Convert.ToInt32(nxt_step.Definition.Element("position").Value) > 0)
                        {
                            empl = _employeeService.GetEmployee(Convert.ToInt32(nxt_step.Definition.Element("position").Value));
                        }

                        var em_list = new List<string>();
                        em_list.Add(empl.emp_work_email);
                        //NotificationUtil.SendNotifications(new int[] { empl.emp_number }, "Workflow - Action needed");

                        _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = empl.emp_number, workflow_id = id, pending = 1, created_at = DateTime.Now });

                        notification_email.to = em_list;
                        break;
                    default:
                        break;
                }

#endif
                    NotificationUtil.SendEmail(notification_email);
                }
                //if (step.Definition.Element("next_step_position_id") != null)
                //{
                //    //get the responsible people for the next step
                //    var level_id = Convert.ToInt32(step.Definition.Element("next_step_position_id").Value);
                //    //if the level id=0, notify the employees supervior
                //    if (level_id == 0)
                //    {
                //        var supervisors = employee.admin_hrm_emp_reportto1.Select(e => e.erep_sup_emp_number).ToArray();
                //        NotificationUtil.SendNotifications(supervisors, "Action needed");
                //    }
                //    else
                //    {
                //        //get the job title with the level_id
                //        var job_title = OptionUtil.GetJobTitles().FirstOrDefault(e => e.id == level_id);
                //        //get all the users belonging to that level
                //        var employees = job_title.admin_hrm_emp_job_record.Select(e => e.employee_number).ToArray();

                //        NotificationUtil.SendNotifications(employees, "Action needed");
                //    }
                //}
                process_document(step.Id, workflow, definition);

                return RedirectToAction("Step", new { id = id });

            }

            public ActionResult Discard(string id)
            {
                var workflow = _workflowService.GetWorkflow(Guid.Parse(id));
                workflow.deleted = 1;
                _workflowService.Update(workflow);
                return Redirect(Request.UrlReferrer.ToString());
            }

            public void UploadFile(HttpPostedFileBase file, string control_name, string file_path, bpm_workflow workflow)
            {

                //var file = Request.Files["job_completion"];
                //var location = Path.Combine(file_path, workflow.title + "_" + file.FileName);
                //if (!Directory.Exists(Server.MapPath(file_path)))
                //{
                //    Directory.CreateDirectory(Server.MapPath(file_path));
                //}
                //file.SaveAs(Server.MapPath(Path.Combine(location)));
            }

            private void process_document(int step, bpm_workflow workflow, XElement definition)
            {
                //return;
                //procurement
                if (workflow.process_id.ToString().ToUpper() == "94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317" || workflow.process_id.ToString().ToUpper() == "2D008467-EA12-4821-A2F2-7568B76039F1" || workflow.process_id.ToString().ToUpper() == "10471746-0236-4F79-BE27-8AA1CBFF2D92" || workflow.process_id.ToString().ToUpper() == "396E4066-81F1-4311-B076-FC3C53BB3178")
                {
                    switch (step)
                    {
                        case 7:
                            //PRF
                            new WorkflowDocumentsHelper().createPRF(Server.MapPath("~/Documents/Procurement/PRF/" + workflow.id + "_PRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/prf_header.png"), definition);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Procurement/PRF/" + workflow.id + "_PRF" + ".pdf", name = "Purchase Request Form", workflow_id = workflow.id });
                            break;
                        case 18:
                            //PO
                            new WorkflowDocumentsHelper().createPurchaseOrder(Server.MapPath("~/Documents/Procurement/PO/" + workflow.id + "_PO" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/po_header.png"), definition);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Procurement/PO/" + workflow.id + "_PO" + ".pdf", name = "Purchase Order Form", workflow_id = workflow.id });
                            break;
                        case 21:
                            //VPRF
                            new WorkflowDocumentsHelper().createVPRF(Server.MapPath("~/Documents/Procurement/VPRF/" + workflow.id + "_VPRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/vprf_header.png"), definition);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Procurement/VPRF/" + workflow.id + "_VPRF" + ".pdf", name = "Vendor Payment Request Form", workflow_id = workflow.id });

                            break;
                        default:
                            break;
                    }
                }

                //advances
                else if (workflow.process_id.ToString().ToUpper() == "A9BE0559-94E4-4A6E-AE46-74A1344DF7DF")
                {
                    switch (step)
                    {
                        case 5:
                            new WorkflowDocumentsHelper().createTravelAdvance(Server.MapPath("~/Documents/Travel/Advance/" + workflow.id + "_Advance" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/travel_advance_header.png"), workflow.admin_hrm_employee, workflow.created_at, definition);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Advance/" + workflow.id + "_Advance" + ".pdf", name = "Travel Advance Form", workflow_id = workflow.id });

                            //Travel authorization
                            new WorkflowDocumentsHelper().createTravelAuthorization(Server.MapPath("~/Documents/Travel/Authorization/" + workflow.id + "_Auth" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/header.png"), workflow.admin_hrm_employee, workflow.created_at, definition);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Authorization/" + workflow.id + "_Auth" + ".pdf", name = "Travel Authorization Form", workflow_id = workflow.id });

                            break;
                        default:
                            break;
                    }
                }
                //reimbursement
                else if (workflow.process_id.ToString().ToUpper() == "A9BE0559-94E4-4A6E-AE46-74A1344DF7EF")
                {
                    switch (step)
                    {
                        case 5:
                            new WorkflowDocumentsHelper().createTravelAdvance(Server.MapPath("~/Documents/Travel/Advance/" + workflow.id + "_Advance" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/reimbursement_header.png"), workflow.admin_hrm_employee, workflow.created_at, definition);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Advance/" + workflow.id + "_Advance" + ".pdf", name = "Travel Advance Form", workflow_id = workflow.id });

                            //Travel authorization
                            new WorkflowDocumentsHelper().createTravelAuthorization(Server.MapPath("~/Documents/Travel/Authorization/" + workflow.id + "_Auth" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/header.png"), workflow.admin_hrm_employee, workflow.created_at, definition);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Authorization/" + workflow.id + "_Auth" + ".pdf", name = "Travel Authorization Form", workflow_id = workflow.id });

                            break;
                        default:
                            break;
                    }
                }
                else if (workflow.process_id.ToString().ToUpper() == "B9BE0559-94E4-4A6E-AE46-74A1344DF7E7" || workflow.process_id.ToString().ToUpper() == "34F7BC4E-6BF4-4386-A8AE-CDAC77CF3AD1")
                {
                    switch (step)
                    {
                        case 6:

                            var path = "~/Documents/Staff_Pay/PRF/" + workflow.id + "_PS" + ".pdf";
                            new WorkflowDocumentsHelper().createPRFPaymentSchedule(Server.MapPath(path), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/prf_header.png"), definition);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = path, name = "PRF", workflow_id = workflow.id });

                            path = "~/Documents/Staff_Pay/RF/" + workflow.id + "_RS" + ".pdf";
                            new WorkflowDocumentsHelper().createReimbursement(Server.MapPath(path), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/reimbursement_header.png"), workflow.admin_hrm_employee, definition);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = path, name = "Reimbursement", workflow_id = workflow.id });



                            break;
                        default:
                            break;
                    }
                }

                else if (workflow.process_id.ToString().ToUpper() == "84E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317")
                {
                    switch (step)
                    {
                        case 5:
                            var path = "~/Documents/Procurement_Pay/PRF/" + workflow.id + "_PS" + ".pdf";
                            new WorkflowDocumentsHelper().createPRFPaymentSchedule(Server.MapPath(path), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/prf_header.png"), definition);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = path, name = "Payment schedule", workflow_id = workflow.id });


                            path = "~/Documents/Procurement_Pay/VPRF/" + workflow.id + "_VPRF" + ".pdf";
                            new WorkflowDocumentsHelper().createPRFPaymentSchedule(Server.MapPath(path), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/vprf_header.png"), definition);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = path, name = "VPRF", workflow_id = workflow.id });

                            break;
                        default:
                            break;
                    }
                }
            }

            public ActionResult Tracker()
            {

                var ids = new Guid[] { Guid.Parse("2D008467-EA12-4821-A2F2-7568B76039F1"), Guid.Parse("94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317"), Guid.Parse("10471746-0236-4F79-BE27-8AA1CBFF2D92"), Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3178") };

                ViewBag.workflows = _workflowService.GetWorkflows().Where(e => ids.Contains(e.process_id));
                return View();
            }

            public ActionResult TrackerTravel()
            {

                var ids = new Guid[] { Guid.Parse("2D008467-EA12-4821-A2F2-7568B76039F1"), Guid.Parse("94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317"), Guid.Parse("10471746-0236-4F79-BE27-8AA1CBFF2D92"), Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3178") };

                ViewBag.workflows = _workflowService.GetWorkflows().Where(e => ids.Contains(e.process_id));
                return View();
            }

            public ActionResult Reverse(Guid workflow_id, int step_id)
            {
                return RedirectToAction("Step", new { id = workflow_id });
            }

            public void BidComparison(bpm_workflow workflow, FormCollection collection, XElement definition, HRM.DAL.Models.admin_hrm_employee employee, Step step, XElement definition_step)
            {
                //var vendor_id = collection["vendor"].ToString();

                //definition.Element("vendors").Elements("vendor").FirstOrDefault(e => e.Element("id").Value == vendor_id).Add(new XAttribute("selected", "1"));
                if (Request.Files["bid_comparison"] != null)
                {

                    var file_path = "~/Documents/Procurement/BidComparison/";
                    var file = Request.Files["bid_comparison"];
                    var location = Path.Combine(file_path, workflow.id + "_" + file.FileName.Replace(@"\", string.Empty));
                    if (!Directory.Exists(Server.MapPath(file_path)))
                    {
                        Directory.CreateDirectory(Server.MapPath(file_path));
                    }
                    file.SaveAs(Server.MapPath(Path.Combine(location)));

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = "Bid Comparison", workflow_id = workflow.id });
                }

                if (collection["vendor"] != null && collection["vendor"].ToString() != "")
                {
                    var vendor_id = collection["vendor"].ToString();
                    if (definition.Element("vendors").Elements("vendor").FirstOrDefault(e => e.Element("id").Value == vendor_id).Attribute("selected") == null)
                        definition.Element("vendors").Elements("vendor").FirstOrDefault(e => e.Element("id").Value == vendor_id).Add(new XAttribute("selected", "1"));

                    var vendor_element = definition.Element("vendors").Elements("vendor").FirstOrDefault(e => e.Element("id").Value == vendor_id);

                    //update the price with the price stated by the vendor
                    foreach (var item in definition.Element("form").Element("items").Elements("item"))
                    {
                        var item_id = item.Attribute("id").Value;

                        //get the item from vendor items
                        //var v_item = vendor_element.Element("items").Elements("itm").FirstOrDefault(e => e.Element("id").Value == item_id);

                        var quantity = item.Element("quantity").Value;
                        //update the estimated prices
                        item.Add(new XElement("unit_price", item.Element("unit_price").Value));
                        item.Add(new XElement("total_price", Convert.ToDouble(item.Element("unit_price").Value) * Convert.ToDouble(quantity)));
                    }


                    //check if you are sending it to hq
                    if (collection["hq"].ToString() == "yes")
                    {
                        //get all the files
                        var files = Util.GetWorkflowDocuments(workflow.id);
                        var email = new Email
                        {

                            body = "Hi " + employee.emp_firstname + ",<br/> Attached are the documents for <strong>" + workflow.title + "</strong>",
                            subject = workflow.title + " documents"
                        };
                        var attachments = new List<string>();
                        foreach (var w_file in files)
                        {
                            if (System.IO.File.Exists(Server.MapPath(w_file.location)))
                                attachments.Add(Server.MapPath(w_file.location));
                        }
                        email.attachments = attachments;

                        email.to = new List<string> { employee.emp_work_email };
                        email.IsHtml = true;
                        NotificationUtil.SendEmail(email);
                    }

                    //mark step as completed
                    definition_step.Element("status").SetValue(1);
                    definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
                }

            }

            public void JobCompletion(bpm_workflow workflow, FormCollection collection, XElement definition, Step step, XElement definition_step, HttpPostedFileBase[] files)
            {
                //var skip = collection["skip_form"];
                //if (skip == null || skip.ToString() != "on")
                //{
                //    var file_path = "~/Documents/Procurement/JobCompletion/";
                //    var file = Request.Files["job_completion"];
                //    var location = Path.Combine(file_path, workflow.title + "_" + file.FileName);
                //    if (!Directory.Exists(Server.MapPath(file_path)))
                //    {
                //        Directory.CreateDirectory(Server.MapPath(file_path));
                //    }
                //    file.SaveAs(Server.MapPath(Path.Combine(location)));

                //    Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = "Job Completion", workflow_id = workflow.id });
                //}



                if (files.Count() > 0)
                {
                    try
                    {
                        var file_path = "~/Documents/Procurement/JobCompletion/";
                        //var files = Request.Files;
                        foreach (HttpPostedFileBase file in files)
                        {
                            var location = Path.Combine(file_path, workflow.id + "_" + file.FileName.Replace(@"\", string.Empty));
                            if (!Directory.Exists(Server.MapPath(file_path)))
                            {
                                Directory.CreateDirectory(Server.MapPath(file_path));
                            }
                            file.SaveAs(Server.MapPath(location));

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = "Job Completion", workflow_id = workflow.id });
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = string.Format("{0}",
                                    ex.Message
                                   );
                        Utils.Log(message + " - JobCompletion");

                    }


                }

                //mark step as completed
                definition_step.Element("status").SetValue(1);
                definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
            }

            public string HQApproval(bpm_workflow workflow, FormCollection collection, XElement definition, Step step, XElement definition_step, HRM.DAL.Models.admin_hrm_employee employee)
            {
#if !DEBUG
            //var approval_token = collection["approval_token"];
            //if (approval_token != null)
            //{
            //    var a_token = _transactionTokenService.GetApprovalToken(approval_token.ToString());
            //    if (a_token != null && a_token.consumed == 0)
            //    {
            //        var result = collection["approval"].ToString();
            //        if (result == step.Definition.Element("approve_value").Value)
            //        {
            //            //var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);
            //            definition_step.Element("status").SetValue(1);

            //            var token_id = _transactionTokenService.AddToken(new bpm_transaction_token
            //            {
            //                t_code = Util.Encrypt(Util.GenerateTCode()),
            //                transaction_id = workflow.id,
            //                created_by = employee.emp_number
            //            });

            //            var signature_element = new XElement("signature");
            //            signature_element.Add(new XElement("signature_id", token_id));
            //            signature_element.Add(new XElement("step", step.Id));
            //            signature_element.Add(new XElement("name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
            //            signature_element.Add(new XElement("emp_number", employee.emp_number));
            //            signature_element.Add(new XElement("created_at", DateTime.Now.ToShortDateString()));
            //            definition.Element("form").Element("signatures").Add(signature_element);

            //            foreach (var form in definition_step.Element("form_values").Elements("form"))
            //            {

            //                var form_id = form.Attribute("id").Value;
            //                definition_step.Element("form_values").Element("signatures").Add(signature_element);
            //                //definition_step.Element("form_values").Elements("form").FirstOrDefault(e => e.Attribute("id").Value == form_id).Element("signatures").Add(signature_element);
            //                //definition.Element("forms").Elements("form").FirstOrDefault(e => e.Attribute("id").Value == form_id).Element("signatures").Add(signature_element);

            //            }
            //            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
            //        }
            //        else if (result == "dismissed")
            //        {
            //            workflow.status = 24;
            //        }
            //        else
            //        {
            //            definition.Element("next_step").SetValue(step.Definition.Element("fallback_step").Value);
            //        }

            //        a_token.consumed = 1;
            //        _transactionTokenService.UpdateApprovalToken(a_token);
            //    }
            //    else
            //    {
            //        return "Your token is expired. Please click refresh to generate a new one.";
            //        //return RedirectToAction("Step", new { id = workflow.id });
            //    }
            //}
            //else
            //{
            //    return "Invalid operation. Please review and re submit.";
            //    //return RedirectToAction("Step", new { id = id });
            //}
#endif
                if (Request.Files["hq"] != null)
                {

                    var file_path = "~/Documents/Procurement/HQ/";
                    var file = Request.Files["hq"];
                    var location = Path.Combine(file_path, workflow.title + "_" + file.FileName);
                    if (!Directory.Exists(Server.MapPath(file_path)))
                    {
                        Directory.CreateDirectory(Server.MapPath(file_path));
                    }
                    file.SaveAs(Server.MapPath(Path.Combine(location)));

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = "HQ Approval", workflow_id = workflow.id });
                }

                //mark step as completed
                definition_step.Element("status").SetValue(1);
                definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);

                return "success";
            }

            public void RFQ(bpm_workflow workflow, FormCollection collection, XElement definition, HRM.DAL.Models.admin_hrm_employee employee, Step step, XElement definition_step)
            {
                var email_info = "";
                if (collection["email_info"] != null)
                {
                    email_info = collection["email_info"].ToString();
                }
                var date = collection["t_start_date"].ToString();
                //get the details of the vendors
                var vendors = collection["vendors"].ToString().Split(',');
                if (vendors.Count() > 0)
                {
                    definition.Element("vendors").Elements().Remove();
                    foreach (var vendor_id in vendors)
                    {

                        var vendor = _vendorService.GetVendor(Convert.ToInt32(vendor_id));

                        //add vendor to the list of selected vendors
                        var vndor = new XElement("vendor");
                        vndor.Add(new XElement("name", vendor.company_name));
                        vndor.Add(new XElement("id", vendor.id));
                        vndor.Add(new XElement("items", vendor.id));

                        definition.Element("vendors").Add(vndor);

                        //var vendor_address = "<strong>" + vendor.company_name + "</strong><br/>";
                        //vendor_address += vendor.address + "<br/><br/>";


                        var path = "~/Documents/Procurement/RFQ/" + Regex.Replace(workflow.title, @"[^0-9a-zA-Z]+", "") + "_" + workflow.id + "_" + vendor.id + ".pdf";


                        new WorkflowDocumentsHelper().createRFQ(Server.MapPath(path), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/header.png"), vendor.company_name, vendor.address, string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname), employee.admin_hrm_emp_job_record.admin_hrm_lkup_job_title.job_title, date, email_info, definition);

                        Util.AddWorkflowDocument(new bpm_workflow_document { location = path, name = "RFQ - " + vendor.name, workflow_id = workflow.id });


                        var file_path = Server.MapPath(path);
                        //PdfBuilder.HtmlToPdf(vendor_address + email_info, file_path);
                        ////PdfBuilder.CreatePDFFromHTMLFile(body, file_path);

                        //Util.AddWorkflowDocument(new bpm_workflow_document { location = path, name = "RFQ - " + vendor.name, workflow_id = workflow.id });

                        var email = new Email
                        {

                            body = "",
                            subject = "RFQ"
                        };
                        email.attachments = new List<string> { file_path };
                        var mail_list = new List<string> { employee.emp_work_email };
                        try
                        {
                            if (vendor.email != "")
                            {
                                foreach (var v_mail in vendor.email.Split(';').ToList())
                                {
                                    mail_list.Add(v_mail.Trim());
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }

                        email.to = mail_list;
                        email.IsHtml = true;
#if !DEBUG
                    NotificationUtil.SendEmail(email);
#endif
                    }
                }

                //mark step as completed
                definition_step.Element("status").SetValue(1);
                definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
            }

            public void UploadQuotes(bpm_workflow workflow, FormCollection collection, XElement definition, Step step, XElement definition_step)
            {
                try
                {
                    foreach (var vendor in definition.Element("vendors").Elements("vendor"))
                    {
                        if (vendor.Element("items").Elements("itm").Count() > 0)
                        {
                            vendor.Element("items").Elements("itm").Remove();
                        }
                    }
                    foreach (var item in definition.Element("form").Element("items").Elements("item"))
                    {
                        foreach (var vendor in definition.Element("vendors").Elements("vendor"))
                        {
                            var itm = new XElement("itm");
                            itm.Add(new XElement("id", item.Attribute("id").Value));
                            itm.Add(new XElement("unit_price", collection[vendor.Element("id").Value + "_" + item.Attribute("id").Value].ToString()));

                            vendor.Element("items").Add(itm);

                            var file = Request.Files["file_" + vendor.Element("id").Value];
                            var file_path = Path.Combine("~/Documents/Procurement/Quotations/", "" + workflow.id + '_' + vendor.Element("name").Value + "_" + file.FileName);


                            if (!Directory.Exists(Server.MapPath("~/Documents/Procurement/Quotations/")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/Documents/Procurement/Quotations/"));
                            }

                            file.SaveAs(Server.MapPath(file_path));
                            var attachment = new XElement("attachment");
                            attachment.Add(new XElement("filename", workflow.title + "_" + workflow.id + '_' + vendor.Element("name").Value + "_" + file.FileName));
                            attachment.Add(new XElement("file", file.FileName));

                            definition_step.Element("attachments").Add(attachment);

                            Util.AddWorkflowDocument(new bpm_workflow_document { location = file_path, name = "Quotation - " + vendor.Element("name").Value, workflow_id = workflow.id });
                        }
                    }
                }
                catch (Exception)
                {

                }
                //check if a vendor is selected and skip the next step
                if (collection["vendor"] != null && collection["vendor"].ToString() != "")
                {
                    var vendor_id = collection["vendor"].ToString();

                    if (definition.Element("vendors").Elements("vendor").FirstOrDefault(e => e.Element("id").Value == vendor_id).Attribute("selected") == null)
                        definition.Element("vendors").Elements("vendor").FirstOrDefault(e => e.Element("id").Value == vendor_id).Add(new XAttribute("selected", "1"));

                    //definition.Element("vendors").Elements("vendor").FirstOrDefault(e => e.Element("id").Value == vendor_id).Add(new XAttribute("selected", "1"));
                    var vendor_element = definition.Element("vendors").Elements("vendor").FirstOrDefault(e => e.Element("id").Value == vendor_id);

                    //update the price with the price stated by the vendor
                    foreach (var item in definition.Element("form").Element("items").Elements("item"))
                    {
                        var item_id = item.Attribute("id").Value;

                        //get the item from vendor items
                        var v_item = vendor_element.Element("items").Elements("itm").FirstOrDefault(e => e.Element("id").Value == item_id);

                        var quantity = item.Element("quantity").Value;
                        //update the estimated prices
                        item.Add(new XElement("vendors_unit_price", v_item.Element("unit_price").Value));
                        item.Add(new XElement("vendors_total_price", Convert.ToDouble(v_item.Element("unit_price").Value) * Convert.ToDouble(quantity)));
                    }

                    //mark step as completed
                    definition_step.Element("status").SetValue(1);
                    //skip the bid comparison since the vendor has already been selected
                    if (workflow.process_id.ToString().ToUpper() == "94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317" || workflow.process_id.ToString().ToUpper() == "10471746-0236-4F79-BE27-8AA1CBFF2D92" || workflow.process_id.ToString().ToUpper() == "396E4066-81F1-4311-B076-FC3C53BB3178" || workflow.process_id.ToString().ToUpper() == "C7902F4A-36DD-4FDC-9B0C-B7059034010D")
                    {
                        definition.Element("next_step").SetValue(12);
                    }
                    else if (workflow.process_id.ToString().ToUpper() == "2D008467-EA12-4821-A2F2-7568B76039F1")
                    {
                        definition.Element("next_step").SetValue(13);
                    }

                }
                else
                {

                    //mark step as completed
                    definition_step.Element("status").SetValue(1);
                    definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
                }
            }

            public ActionResult Buzz(Guid workflow_id)
            {
                var bLWorkflow = new BLWorkflow(workflow_id);
                var workflow = bLWorkflow.GetWorkflow();
                var definition = XElement.Parse(workflow.workflow);
                var next_position = definition.Element("next_step");
                var nxt_step = bLWorkflow.GetStep(Convert.ToInt32(next_position.Value));




                var email_body = "";

                string Domain = "https://workspace.mgic-nigeria.org";
                Domain += "/workflow/step/?id=" + workflow.id;

                email_body = string.Format("Your action is needed for the completion of this process <strong>{0}</strong>.<br/>", workflow.title);
                email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

                var notification_email = new Email
                {

                    body = email_body,
                    subject = workflow.title + " - Action required - Workspace"
                };
                //notification_email.to = new List<string> { employee.emp_work_email };
                notification_email.IsHtml = true;


                if (nxt_step.Definition.Element("type").Value.ToString() == "approval")
                {
                    var approval_token = Util.GenerateTCode();
                    _transactionTokenService.AddApprovalToken(new bpm_approval_token { token = approval_token });

                    email_body = string.Format("Your approval is needed for the completion of this process <strong>{0}</strong>.<br/>", workflow.title);
                    email_body += string.Format("Your approval code is <code>{0}</code><br/><br/>", approval_token);
                    email_body += "Note that this code will expire in 24hrs. Thanks";


                    email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

                    notification_email.subject = workflow.title + " Approval - Workspace";
                    notification_email.body = email_body;
                    //notification_email.to = new List<string> { employee.emp_work_email };
                }

                /*
                var requestor = _employeeService.GetEmployee(workflow.created_by);
                var supervisors = requestor.admin_hrm_emp_reportto1.Select(x => x.erep_sup_emp_number).ToArray();

                notification_email.to = new List<string> { requestor.admin_hrm_emp_reportto1.FirstOrDefault().admin_hrm_employee.emp_work_email };

                //NotificationUtil.SendNotifications(supervisors, "Workflow - Action needed");
                foreach (var sup in supervisors)
                {
                    _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = sup, workflow_id = workflow.id, pending = 1, created_at = DateTime.Now });
                }
                */

                switch (nxt_step.Definition.Element("position").Attribute("pos_type").Value)
                {
                    case "sup":
                        //get the supervisor of the person that applied for the request
                        var requestor = _employeeService.GetEmployee(workflow.created_by);
                        var supervisors = requestor.admin_hrm_emp_reportto1.Select(x => x.erep_sup_emp_number).ToArray();

                        notification_email.to = new List<string> { requestor.admin_hrm_emp_reportto1.FirstOrDefault().admin_hrm_employee.emp_work_email };

                        //NotificationUtil.SendNotifications(supervisors, "Workflow - Action needed");
                        foreach (var sup in supervisors)
                        {
                            _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = sup, workflow_id = workflow.id, pending = 1, created_at = DateTime.Now });
                        }

                        break;
                    case "jti":
                        var job_title_id = Convert.ToInt32(nxt_step.Definition.Element("position").Value);

                        var employees = _employeeService.GetEmployees().Where(x => x.admin_hrm_emp_job_record.job_title_id == job_title_id);
                        var ids = employees.Select(x => x.emp_number).ToArray();

                        var mail_list = new List<string>();

                        //NotificationUtil.SendNotifications(ids, "Workflow - Action needed");

                        foreach (var emp in employees)
                        {
                            _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = emp.emp_number, workflow_id = workflow.id, pending = 1, created_at = DateTime.Now });
                            mail_list.Add(emp.emp_work_email);
                        }

                        notification_email.to = mail_list;

                        break;
                    case "dir":
                        //get the subunit of the requestor
                        var project_id = workflow.project_id;
                        var project = OptionUtil.GetProjects().FirstOrDefault(e => e.id == workflow.project_id);
                        if (project != null && project.sub_parent.HasValue)
                        {
                            project_id = project.sub_parent.Value;
                        }
                        var project_d = Util.GetProgramApprover(project_id, 0);


                        var emps = _employeeService.GetEmployee(project_d);


                        var list = new List<string>();
                        _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = emps.emp_number, workflow_id = workflow.id, pending = 1, created_at = DateTime.Now });
                        list.Add(emps.emp_work_email);

                        notification_email.to = list;
                        break;
                    case "emp":
                        //get the subunit of the requestor
                        var empl = _employeeService.GetEmployee(workflow.created_by);


                        var em_list = new List<string>();
                        em_list.Add(empl.emp_work_email);
                        //NotificationUtil.SendNotifications(new int[] { empl.emp_number }, "Workflow - Action needed");

                        _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = empl.emp_number, workflow_id = workflow.id, pending = 1, created_at = DateTime.Now });

                        notification_email.to = em_list;
                        break;
                    default:
                        break;
                }


                NotificationUtil.SendEmail(notification_email);

                return Redirect(Request.UrlReferrer.ToString());

            }

            public void generate_doc(Guid workflow_id)
            {
                var bLWorkflow = new BLWorkflow(workflow_id);

                var workflow = bLWorkflow.GetWorkflow();
                var step = bLWorkflow.GetCurrentStep();
                var step_type = step.Definition.Element("type").Value;
                var definition = bLWorkflow.GetDefinition();
                var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);

                //return;
                //procurement
                if (workflow.process_id.ToString().ToUpper() == "94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317" || workflow.process_id.ToString().ToUpper() == "2D008467-EA12-4821-A2F2-7568B76039F1" || workflow.process_id.ToString().ToUpper() == "10471746-0236-4F79-BE27-8AA1CBFF2D92" || workflow.process_id.ToString().ToUpper() == "396E4066-81F1-4311-B076-FC3C53BB3178")
                {
                    new WorkflowDocumentsHelper().createPRF(Server.MapPath("~/Documents/Procurement/PRF/" + Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+", "_") + "_" + workflow.id + "_PRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/prf_header.png"), definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Procurement/PRF/" + Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+", "_") + "_" + workflow.id + "_PRF" + ".pdf", name = "Purchase Request Form", workflow_id = workflow.id });

                    new WorkflowDocumentsHelper().createPurchaseOrder(Server.MapPath("~/Documents/Procurement/PO/" + Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+", "_") + "_" + workflow.id + "_PO" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/po_header.png"), definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Procurement/PO/" + Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+", "_") + "_" + workflow.id + "_PO" + ".pdf", name = "Purchase Order Form", workflow_id = workflow.id });

                    new WorkflowDocumentsHelper().createVPRF(Server.MapPath("~/Documents/Procurement/VPRF/" + Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+", "_") + "_" + workflow.id + "_VPRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/vprf_header.png"), definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Procurement/VPRF/" + Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+", "_") + "_" + workflow.id + "_VPRF" + ".pdf", name = "Vendor Payment Request Form", workflow_id = workflow.id });
                }
                //travel advance documents
                else if (workflow.process_id.ToString().ToUpper() == "A9BE0559-94E4-4A6E-AE46-74A1344DF7DF")
                {
                    new WorkflowDocumentsHelper().createTravelAdvance(Server.MapPath("~/Documents/Travel/Advance/" + workflow.id + "_Advance" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/travel_advance_header.png"), workflow.admin_hrm_employee, workflow.created_at, definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Advance/" + workflow.id + "_Advance" + ".pdf", name = "Travel Advance Form", workflow_id = workflow.id });

                    //Travel authorization
                    new WorkflowDocumentsHelper().createTravelAuthorization(Server.MapPath("~/Documents/Travel/Authorization/" + workflow.id + "_Auth" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/header.png"), workflow.admin_hrm_employee, workflow.created_at, definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Authorization/" + workflow.id + "_Auth" + ".pdf", name = "Travel Authorization Form", workflow_id = workflow.id });
                }
                //travel reimbursement documents
                else if (workflow.process_id.ToString().ToUpper() == "A9BE0559-94E4-4A6E-AE46-74A1344DF7EF")
                {
                    new WorkflowDocumentsHelper().createTravelAdvance(Server.MapPath("~/Documents/Travel/Advance/" + workflow.id + "_Advance" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/reimbursement_header.png"), workflow.admin_hrm_employee, workflow.created_at, definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Advance/" + workflow.id + "_Advance" + ".pdf", name = "Travel Advance Form", workflow_id = workflow.id });

                    //Travel authorization
                    new WorkflowDocumentsHelper().createTravelAuthorization(Server.MapPath("~/Documents/Travel/Authorization/" + workflow.id + "_Auth" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/header.png"), workflow.admin_hrm_employee, workflow.created_at, definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Authorization/" + workflow.id + "_Auth" + ".pdf", name = "Travel Authorization Form", workflow_id = workflow.id });
                }
                else if (workflow.process_id.ToString().ToUpper() == "84E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317")
                {
                    //var path = "~/Documents/Procurement_Pay/PRF/" + "_" + DateTime.Now.Second + "_" + Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+", "_") + workflow.id + "_PS" + ".pdf";
                    //new WorkflowDocumentsHelper().createPRFPaymentSchedule(Server.MapPath(path), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/header.png"));

                    //Util.AddWorkflowDocument(new bpm_workflow_document { location = path, name = "Payment schedule", workflow_id = workflow.id });
                    var path = "~/Documents/Procurement_Pay/PRF/" + workflow.id + "_PS" + ".pdf";
                    new WorkflowDocumentsHelper().createPRFPaymentSchedule(Server.MapPath(path), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/prf_header.png"), definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = path, name = "Payment schedule", workflow_id = workflow.id });


                    path = "~/Documents/Procurement_Pay/VPRF/" + workflow.id + "_VPRF" + ".pdf";
                    new WorkflowDocumentsHelper().createPRFPaymentSchedule(Server.MapPath(path), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/vprf_header.png"), definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = path, name = "VPRF", workflow_id = workflow.id });

                }
                //child support
                else if (workflow.process_id.ToString().ToUpper() == "B9BE0559-94E4-4A6E-AE46-74A1344DF7E7" || workflow.process_id.ToString().ToUpper() == "34F7BC4E-6BF4-4386-A8AE-CDAC77CF3AD1")
                {
                    var path = "~/Documents/Staff_Pay/PRF/" + Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+", "_") + "_" + DateTime.Now.Second + "_" + workflow.id + "_PS" + ".pdf";
                    new WorkflowDocumentsHelper().createPRFPaymentSchedule(Server.MapPath(path), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/prf_header.png"), definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = path, name = "PRF", workflow_id = workflow.id });

                    path = "~/Documents/Staff_Pay/RF/" + Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+", "_") + "_" + DateTime.Now.Second + "_" + workflow.id + "_RS" + ".pdf";
                    new WorkflowDocumentsHelper().createReimbursement(Server.MapPath(path), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/reimbursement_header.png"), workflow.admin_hrm_employee, definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = path, name = "Reimbursement", workflow_id = workflow.id });
                }
                //new procurement flow
                else if (workflow.process_id.ToString().ToUpper() == "C7902F4A-36DD-4FDC-9B0C-B7059034010D")
                {
                    if (step.Id >= 6)
                    {
                        new WorkflowDocumentsHelper().NewCreatePRF(Server.MapPath("~/Documents/Procurement/PRF/" + Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+", "_") + "_" + workflow.id + "_PRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/prf_header.png"), definition);
                        Util.AddWorkflowDocument(new bpm_workflow_document
                        {
                            location = "~/Documents/Procurement/PRF/" +
                                       Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+",
                                           "_") + "_" + workflow.id + "_PRF" + ".pdf",
                            name = "Purchase Request Form",
                            workflow_id = workflow.id
                        });
                    }
                    if (step.Id >= 17)
                    {
                        new WorkflowDocumentsHelper().NewCreatePurchaseOrder(
                            Server.MapPath("~/Documents/Procurement/PO/" +
                                           Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+",
                                               "_") + "_" + workflow.id + "_PO" + ".pdf"), workflow.pm_project.code,
                            definition.Element("form"), Server.MapPath("~/Documents/Images/po_header.png"), definition);
                        Util.AddWorkflowDocument(new bpm_workflow_document
                        {
                            location = "~/Documents/Procurement/PO/" +
                                       Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+",
                                           "_") + "_" + workflow.id + "_PO" + ".pdf",
                            name = "Purchase Order Form",
                            workflow_id = workflow.id
                        });
                    }
                    if (step.Id >= 21)
                    {
                        new WorkflowDocumentsHelper().NewCreateVPRF(Server.MapPath("~/Documents/Procurement/VPRF/" + Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+", "_") + "_" + workflow.id + "_VPRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/vprf_header.png"), definition);
                        Util.AddWorkflowDocument(new bpm_workflow_document
                        {
                            location = "~/Documents/Procurement/VPRF/" +
                                       Regex.Replace(workflow.title.Replace(@"\", "").Replace(@"/", ""), @"[^0-9a-zA-Z]+",
                                           "_") + "_" + workflow.id + "_VPRF" + ".pdf",
                            name = "Vendor Payment Request Form",
                            workflow_id = workflow.id
                        });
                    }
                }
            }

            public ActionResult generate_document(Guid workflow_id)
            {
                generate_doc(workflow_id);

                return Redirect(Request.UrlReferrer.ToString());
            }

            public ActionResult AddFile(FormCollection collection, HttpPostedFileBase[] files)
            {
                var id = new Guid(collection["workflow_id"]);
                var bLWorkflow = new BLWorkflow(id);

                var workflow = bLWorkflow.GetWorkflow();

                try
                {
                    var file_path = "~/Documents/Attachments/";
                    //var files = Request.Files;
                    foreach (HttpPostedFileBase file in files)
                    {
                        var location = Path.Combine(file_path, workflow.id + "_" + file.FileName);
                        if (!Directory.Exists(Server.MapPath(file_path)))
                        {
                            Directory.CreateDirectory(Server.MapPath(file_path));
                        }
                        file.SaveAs(Server.MapPath(Path.Combine(location)));

                        Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = file.FileName, workflow_id = workflow.id });
                    }
                }
                catch (Exception)
                {

                }

                return Redirect(Request.UrlReferrer.ToString());
            }

            /*
             * Added and Edited By Johnbosco
             * for a new procurement flow requested by procurement team
             */
            public void RefreshToken(HRM.DAL.Models.admin_hrm_employee employee, FormCollection collection, Guid id)
            {
                var bLWorkflow = new BLWorkflow(id);
                var workflow = bLWorkflow.GetWorkflow();
                var refreshToken = collection["refresh_token"].Split(',');
                if (refreshToken[0] == "refresh")
                {
                    var e_body = "";
                    string dm = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    dm += "/workflow/step/?id=" + id;

                    e_body = string.Format("Your action is needed for the completion of this process <strong>{0}</strong>.<br/>", workflow.title);
                    e_body += string.Format(" <a href='{0}'>Click here </a> to view", dm);

                    var n_email = new Email
                    {

                        body = e_body,
                        subject = "Action required - Workspace"
                    };
                    n_email.to = new List<string> { employee.emp_work_email };
                    n_email.IsHtml = true;

                    var approval_token = Util.GenerateTCode();
                    _transactionTokenService.AddApprovalToken(new bpm_approval_token { token = approval_token });

                    e_body = string.Format("Your approval is needed for the completion of this process <strong>{0}</strong>.<br/>", workflow.title);
                    e_body += string.Format("Your approval code is <code>{0}</code><br/><br/>", approval_token);
                    e_body += "Note that this code will expire in 24hrs. Thanks";


                    e_body += string.Format(" <a href='{0}'>Click here </a> to view", dm);

                    n_email.subject = "Approval required - Workspace";
                    n_email.body = e_body;

                    n_email.to = new List<string> { employee.emp_work_email };

                    NotificationUtil.SendEmailTest(n_email);
                }
            }

            private void NewProcurementProcess(HRM.DAL.Models.admin_hrm_employee employee, Step step, XElement definition,
                FormCollection collection, HttpPostedFileBase[] files)
            {
                //Get the step Id
                int stepId = 0;
                XElement definition_step = null;
                XElement next_position = null;
                //init new_procurement and get the workflow id from the DB
                var new_procurement = new NewProcurement();
                var id = new Guid(collection["workflow_id"]);
                var bLWorkflow = new BLWorkflow(id);
                var workflow = bLWorkflow.GetWorkflow();
                Step nxt_step = null;
                if (step != null)
                {
                    stepId = step.Id;
                    definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);
                    //Get the next step
                    next_position = definition_step.Element("next_step");
                    nxt_step = bLWorkflow.GetStep(Convert.ToInt32(next_position.Value));
                }
                else
                {
                    stepId = -1;
                }
                //init email body variable and the notification email
                var email_body = "";
                var notification_email = new Email
                {
                    body = email_body,
                    subject = workflow.title + " - Action required - Workspace"
                };
                //notification_email.to = new List<string> { employee.emp_work_email };
                notification_email.IsHtml = true;
                /*
                 * Switch the steps
                 * NOTE: the default is approval
                 */
                var result = "";
                switch (stepId)
                {
                    case (int)NewProcurementProcessTag.ProcurementRequest:
                        //Add the new_procurement Items to the xml doc
                        new_procurement.AddItemsToProcure(definition, step, definition_step, collection, employee, id);
                        //upload supporting document
                        if (Request.Files.Count > 0)
                        {
                            try
                            {
                                var file_path = "~/Documents/new_procurement/SupportDoc/";
                                foreach (HttpPostedFileBase file in files)
                                {
                                    var location = Path.Combine(file_path, workflow.title.Replace(@"\", "").Replace(@"/", "") + "_" + file.FileName.Replace(@"\", string.Empty));
                                    if (!Directory.Exists(Server.MapPath(file_path)))
                                    {
                                        Directory.CreateDirectory(Server.MapPath(file_path));
                                    }
                                    file.SaveAs(Server.MapPath(Path.Combine(location)));

                                    Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = file.FileName, workflow_id = workflow.id });
                                }
                            }
                            catch (Exception ex)
                            {
                                string message = string.Format("{0}",
                                    ex.Message);
                                Utils.Log(message + " - AddUserToWorkflow");
                                Utils.LogError(ex);
                                // Exception raise = ex;
                            }
                        }
                        if (nxt_step.Definition.Element("type").Value.ToString() == "approval")
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Approval);
                        }
                        break;
                    case (int)NewProcurementProcessTag.CountryDirectorApproval:
                        result = new_procurement.Approve(definition, step, definition_step, collection, employee, id, workflow);
                        if (result != "success")
                        {
                            ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                            return;
                        }
                        else
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Assignment);
                        }
                        break;
                    case (int)NewProcurementProcessTag.AssignProcurementOfficer:
                        if (employee.emp_number == Util.GetProgramApprover(-1, 0))
                        {
                            if (collection["wf_assignment"] != null && collection["wf_assignment"] == "emp")
                            {
                                new_procurement.AddUserToSteps(new int[] { 8, 9, 10, 11, 12, 18, 19, 22 }, definition, Convert.ToInt32(collection["emp_number"]), nxt_step.Id);
                                //step = bLWorkflow.GetStep();
                                definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Id.ToString());
                                definition_step.Element("status").SetValue(1);

                                next_position = definition_step.Element("next_step");
                                nxt_step = bLWorkflow.GetStep(Convert.ToInt32(next_position.Value));
                                SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Attention);
                            }
                        }
                        break;
                    case (int)NewProcurementProcessTag.RequestForQuotation:
                        RFQ(workflow, collection, definition, employee, step, definition_step);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Attention);
                        break;
                    case (int)NewProcurementProcessTag.UploadQuotation:
                        UploadQuotes(workflow, collection, definition, step, definition_step);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Attention);
                        break;
                    case (int)NewProcurementProcessTag.PurchaseOrder:
                        new_procurement.CreatePO(definition, step, collection, definition_step);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Approval);
                        break;
                    case (int)NewProcurementProcessTag.JobCompletion:
                        JobCompletion(workflow, collection, definition, step, definition_step, files);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Attention);
                        break;
                    case (int)NewProcurementProcessTag.VendorPaymentRequestForm:
                        new_procurement.CreateVPRF(definition, step, definition_step, collection);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Approval);
                        break;
                    case (int)NewProcurementProcessTag.ThirdCountryDirectorApproval:
                        result = new_procurement.Approve(definition, step, definition_step, collection, employee, id, workflow);
                        if (result != "success")
                        {
                            ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                            return;
                        }
                        else
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Attention);
                        }
                        break;
                    default:
                        result = new_procurement.Approve(definition, step, definition_step, collection, employee, id, workflow);
                        if (result != "success")
                        {
                            ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                            return;
                        }
                        else
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Approval);
                        }
                        break;
                }
                if (collection["comment"] != null && collection["comment"] != "")
                {
                    //add comment to the particular step
                    definition_step.Element("comments").SetValue(collection["comment"].ToString());
                    //add comment to the whole request document
                    var comment = new XElement("comment");
                    comment.Add(new XElement("step", step.Id));
                    comment.Add(new XElement("employee_id", employee.emp_number));
                    comment.Add(new XElement("info", collection["comment"].ToString()));
                    comment.Add(new XElement("employee_name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
                    comment.Add(new XElement("created_at", DateTime.Now));

                    definition.Element("comments").Add(comment);
                }
                //set the user that completed the step if stepId != -1
                //step id -1 was used when Aflow assignment was not part of thte steps
                if (stepId != -1)
                {
                    definition_step.Element("created_at").SetValue(DateTime.Now);
                    definition_step.Element("created_by").SetValue(employee.emp_number);
                    definition_step.Element("created_by_name").SetValue(string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname));

                    workflow.workflow = definition.ToString();
                    _workflowService.Update(workflow);
                }
                else
                {
                    workflow.workflow = definition.ToString();
                    _workflowService.Update(workflow);
                }

                NotificationUtil.NewSendEmail(notification_email);

                new_process_document(step.Id, workflow, definition);
            }

            private void ProcurementAdmin(HRM.DAL.Models.admin_hrm_employee employee, Step step, XElement definition,
                FormCollection collection, HttpPostedFileBase[] files)
            {

                //Step Id
                int stepId = 0;
                XElement definition_step = null;
                XElement next_position = null;

                var procurement_admin = new ProcurementAdmin();
                var id = new Guid(collection["workflow_id"]);
                var bLWorkflow = new BLWorkflow(id);
                var workflow = bLWorkflow.GetWorkflow();
                Step nxt_step = null;
                if (step != null)
                {
                    stepId = step.Id;
                    definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);
                    //Get the next step
                    next_position = definition_step.Element("next_step");
                    nxt_step = bLWorkflow.GetStep(Convert.ToInt32(next_position.Value));
                }
                else
                {
                    stepId = -1;
                }
                //init email body variable and the notification email
                var email_body = "";
                var notification_email = new Email
                {
                    body = email_body,
                    subject = workflow.title + " - Action required - Workspace"
                };
                //notification_email.to = new List<string> { employee.emp_work_email };
                notification_email.IsHtml = true;
                /*
                 * Switch the steps
                 * NOTE: the default is approval
                 */
                var result = "";
                switch (stepId)
                {
                    case (int)ProcurementAdminTag.ProcurementRequest:
                        //Add the new_procurement Items to the xml doc
                        procurement_admin.AddItemsToProcure(definition, step, definition_step, collection, employee, id);
                        //upload supporting document
                        if (Request.Files.Count > 0)
                        {
                            try
                            {
                                var file_path = "~/Documents/procurement_admin/SupportDoc/";
                                foreach (HttpPostedFileBase file in files)
                                {
                                    var location = Path.Combine(file_path, workflow.title.Replace(@"\", "").Replace(@"/", "") + "_" + file.FileName.Replace(@"\", string.Empty));
                                    if (!Directory.Exists(Server.MapPath(file_path)))
                                    {
                                        Directory.CreateDirectory(Server.MapPath(file_path));
                                    }
                                    file.SaveAs(Server.MapPath(Path.Combine(location)));

                                    Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = file.FileName, workflow_id = workflow.id });
                                }
                            }
                            catch (Exception ex)
                            {
                                string message = string.Format("{0}",
                                    ex.Message);
                                Utils.Log(message + " - AddUserToWorkflow");
                                Utils.LogError(ex);
                                // Exception raise = ex;
                            }
                        }
                        if (nxt_step.Definition.Element("type").Value.ToString() == "approval")
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Approval);
                        }
                        break;
                    case (int)ProcurementAdminTag.CountryDirectorApproval:
                        result = procurement_admin.Approve(definition, step, definition_step, collection, employee, id, workflow);
                        if (result != "success")
                        {
                            ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                            return;
                        }
                        else
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Assignment);
                        }
                        break;
                    case (int)ProcurementAdminTag.UpdateCost:
                        if (employee.emp_number == Util.GetProgramApprover(-1, 0))
                        {
                            if (collection["wf_assignment"] != null && collection["wf_assignment"] == "emp")
                            {
                                procurement_admin.AddUserToSteps(new int[] { 7, 9, 10, 11, 12, 18, 19, 22 }, definition, Convert.ToInt32(collection["emp_number"]), nxt_step.Id);
                                //step = bLWorkflow.GetStep();
                                definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Id.ToString());
                                definition_step.Element("status").SetValue(1);

                                next_position = definition_step.Element("next_step");
                                nxt_step = bLWorkflow.GetStep(Convert.ToInt32(next_position.Value));
                                SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Attention);
                            }
                        }
                        break;
                    case (int)ProcurementAdminTag.FirstComplianceReview:
                        RFQ(workflow, collection, definition, employee, step, definition_step);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Attention);
                        break;
                    case (int)ProcurementAdminTag.RequestForQuotation:
                        RFQ(workflow, collection, definition, employee, step, definition_step);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Attention);
                        break;
                    case (int)ProcurementAdminTag.UploadQuotation:
                        UploadQuotes(workflow, collection, definition, step, definition_step);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Attention);
                        break;
                    case (int)ProcurementAdminTag.PurchaseOrder:
                        procurement_admin.CreatePO(definition, step, collection, definition_step);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Approval);
                        break;
                    case (int)ProcurementAdminTag.JobCompletion:
                        JobCompletion(workflow, collection, definition, step, definition_step, files);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Attention);
                        break;
                    case (int)ProcurementAdminTag.VendorPaymentRequestForm:
                        procurement_admin.CreateVPRF(definition, step, definition_step, collection);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Approval);
                        break;
                    case (int)ProcurementAdminTag.ThirdCountryDirectorApproval:
                        result = procurement_admin.Approve(definition, step, definition_step, collection, employee, id, workflow);
                        if (result != "success")
                        {
                            ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                            return;
                        }
                        else
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Attention);
                        }
                        break;
                    default:
                        result = procurement_admin.Approve(definition, step, definition_step, collection, employee, id, workflow);
                        if (result != "success")
                        {
                            ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                            return;
                        }
                        else
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, nxt_step, step, collection, definition, MailType.Approval);
                        }
                        break;
                }
                if (collection["comment"] != null && collection["comment"] != "")
                {
                    //add comment to the particular step
                    definition_step.Element("comments").SetValue(collection["comment"].ToString());
                    //add comment to the whole request document
                    var comment = new XElement("comment");
                    comment.Add(new XElement("step", step.Id));
                    comment.Add(new XElement("employee_id", employee.emp_number));
                    comment.Add(new XElement("info", collection["comment"].ToString()));
                    comment.Add(new XElement("employee_name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
                    comment.Add(new XElement("created_at", DateTime.Now));

                    definition.Element("comments").Add(comment);
                }
                //set the user that completed the step if stepId != -1
                //step id -1 was used when Aflow assignment was not part of thte steps
                if (stepId != -1)
                {
                    definition_step.Element("created_at").SetValue(DateTime.Now);
                    definition_step.Element("created_by").SetValue(employee.emp_number);
                    definition_step.Element("created_by_name").SetValue(string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname));

                    workflow.workflow = definition.ToString();
                    _workflowService.Update(workflow);
                }
                else
                {
                    workflow.workflow = definition.ToString();
                    _workflowService.Update(workflow);
                }

                NotificationUtil.NewSendEmail(notification_email);

                new_process_document(step.Id, workflow, definition);
            }

            private void SendEmailForApproval(string email_body, Email notification_email, bpm_workflow workflow, Guid id,
                HRM.DAL.Models.admin_hrm_employee employee, Step nxt_step, Step step, FormCollection collection, XElement definition, MailType mailType)
            {
                //Get the url of the flow
                string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                Domain += "/workflow/step/?id=" + id;

                var approval_token = Util.GenerateTCode();
                _transactionTokenService.AddApprovalToken(new bpm_approval_token { token = approval_token });

                email_body = string.Format("Your approval is needed for the completion of this process <strong>{0}</strong>.<br/>", workflow.title);
                email_body += string.Format("Your approval code is <code>{0}</code><br/><br/>", approval_token);
                email_body += "Note that this code will expire in 24hrs. Thanks";

                email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

                notification_email.subject = workflow.title + " Approval - Workspace";
                notification_email.body = email_body;
                notification_email.to = new List<string> { employee.emp_work_email };

                /*
                 * Get the supervisor or the approving officer that is responsible for the next step
                 * This is a switch statement and check with the values in the xml for a specific step
                 * Check if it was a rejection or approval
                 */
                var rs = collection["approval"];
                if (!string.IsNullOrEmpty(rs) && rs.Equals("rejected"))
                {
                    //get the step and send the email to the person in the step 
                    XElement fall_back_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == collection["fall_back"]);
                    int created_by = int.Parse(fall_back_step.Element("created_by").Value);
                    var fall_bac = _employeeService.GetEmployee(created_by);
                    if (int.Parse(fall_back_step.Element("code").Value) == 1)
                    {
                        email_body = string.Format("Your are needed to review the request <strong>{0}</strong>.<br/>", workflow.title);
                        email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);
                        notification_email.body = email_body;
                    }
#if DEBUG
                    notification_email.to = new List<string> { employee.emp_work_email };
#else
                notification_email.to = new List<string> { fall_bac.emp_work_email };
#endif
                    _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = fall_bac.emp_number, workflow_id = id, pending = 1, created_at = DateTime.Now });
                }

                if (mailType == MailType.Assignment)
                {
                    //prepare email to procurement lead two
                    var lead2 = Util.GetProgramApprover(-1, 0);
                    var lead_emp = _employeeService.GetEmployee(lead2);
                    email_body = string.Format("Your are needed to assign the request <strong>{0}</strong> to a procurement officer.<br/>", workflow.title);
                    email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);
                    notification_email.body = email_body;
#if DEBUG
                    notification_email.to = new List<string> { employee.emp_work_email };
#else
                notification_email.to = new List<string> { lead_emp.emp_work_email };
#endif
                    _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = lead_emp.emp_number, workflow_id = id, pending = 1, created_at = DateTime.Now });
                }
                else if (mailType == MailType.Attention)
                {
                    //prepare email to procurement officer
                    var proc_officer = _employeeService.GetEmployee(Convert.ToInt32(collection["emp_number"]));
                    if (proc_officer == null)
                    {
                        var emp_id = Convert.ToInt32(step.Definition.Element("position").Value);
                        proc_officer = _employeeService.GetEmployee(emp_id);
                    }
                    var name = proc_officer.emp_lastname + " " + proc_officer.emp_firstname;
                    email_body = string.Format("Hi <strong>{0}</strong>,<br/>", name);
                    email_body += string.Format("The request <strong>{0}</strong> has been assigned to you.<br/>", workflow.title);
                    email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);
                    notification_email.body = email_body;
#if DEBUG
                    notification_email.to = new List<string> { employee.emp_work_email };
#else
                notification_email.to = new List<string> { proc_officer.emp_work_email };
#endif
                    _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = proc_officer.emp_number, workflow_id = id, pending = 1, created_at = DateTime.Now });
                }
                else
                {
                    switch (nxt_step.Definition.Element("position").Attribute("pos_type").Value)
                    {
                        case "sup":
                            //get the supervisor of the person that applied for the request
                            var requestor = _employeeService.GetEmployee(workflow.created_by);
                            var supervisors = requestor.admin_hrm_emp_reportto1.Select(e => e.erep_sup_emp_number).ToArray();
#if DEBUG
                            notification_email.to = new List<string> { employee.emp_work_email };
#else
                        notification_email.to = new List<string> { requestor.admin_hrm_emp_reportto1.FirstOrDefault().admin_hrm_employee.emp_work_email };
#endif
                            // NotificationUtil.SendNotifications(supervisors, "Workflow - Action needed");
                            foreach (var sup in supervisors)
                            {
                                _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = sup, workflow_id = id, pending = 1, created_at = DateTime.Now });
                            }
                            break;
                        case "jti":
                            var job_title_id = Convert.ToInt32(step.Definition.Element("next_step_position_id").Value);

                            var employees = _employeeService.GetEmployees().Where(e => e.admin_hrm_emp_job_record.job_title_id == job_title_id);
                            var ids = employees.Select(e => e.emp_number).ToArray();

                            var mail_list = new List<string>();

                            //NotificationUtil.SendNotifications(ids, "Workflow - Action needed");

                            foreach (var emp in employees)
                            {
                                _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = emp.emp_number, workflow_id = id, pending = 1, created_at = DateTime.Now });
                                mail_list.Add(emp.emp_work_email);
                            }
#if DEBUG
                            notification_email.to = new List<string> { employee.emp_work_email };
#else
                        notification_email.to = mail_list;
#endif
                            break;
                        case "dir":
                            //get the director of the selected grant
                            //var directorate = _employeeService.GetDirectorate(workflow.created_by);
                            var project_id = workflow.project_id;
                            var project = OptionUtil.GetProjects().FirstOrDefault(e => e.id == workflow.project_id);
                            if (project != null && project.sub_parent.HasValue)
                            {
                                project_id = project.sub_parent.Value;
                            }
                            var project_d = Util.GetProgramApprover(project_id, 0);

                            var emps = _employeeService.GetEmployee(project_d);
                            var list = new List<string>();

#if DEBUG
                            _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = employee.emp_number, workflow_id = id, pending = 1, created_at = DateTime.Now });
                            notification_email.to = new List<string> { employee.emp_work_email };
#else
                        _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = emps.emp_number, workflow_id = id, pending = 1, created_at = DateTime.Now });
                        list.Add(emps.emp_work_email);
#endif
                            //notification_email.to = list;
                            break;
                        case "emp":
                            //get the subunit of the requestor
                            var empl = _employeeService.GetEmployee(workflow.created_by);

                            if (nxt_step.Definition.Element("position").Value != null)
                            {
                                empl = _employeeService.GetEmployee(Convert.ToInt32(nxt_step.Definition.Element("position").Value));
                            }

                            var em_list = new List<string>();
                            em_list.Add(empl.emp_work_email);
                            //NotificationUtil.SendNotifications(new int[] { empl.emp_number }, "Workflow - Action needed");

                            _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = empl.emp_number, workflow_id = id, pending = 1, created_at = DateTime.Now });
                            //This code was uncommented for the procurement flow to send assignment mails to the employee it was assigned to.
                            notification_email.to = em_list;
                            break;
                    }
                }
            }

            private void ProcessAndSendMail(Email email, bpm_workflow workflow, admin_hrm_employee current_employee, FormCollection collection,
                admin_hrm_employee next_employee, MailType mailType)
            {
                /*
                 * Todo get the current step, next step, definition, guid
                 */
            }

            public void new_process_document(int step, bpm_workflow workflow, XElement definition)
            {
                switch (step)
                {
                    case 6:
                        //PRF
                        try
                        {
                            new WorkflowDocumentsHelper().NewCreatePRF(Server.MapPath("~/Documents/Procurement/PRF/" + workflow.id + "_PRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/prf_header.png"), definition);
                            Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Procurement/PRF/" + workflow.id + "_PRF" + ".pdf", name = "Purchase Request Form", workflow_id = workflow.id });
                        }
                        catch (Exception e)
                        {
                            Utils.LogError(e);
                        }
                        break;
                    case 17:
                        //PO
                        try
                        {
                            new WorkflowDocumentsHelper().NewCreatePurchaseOrder(
                                Server.MapPath("~/Documents/Procurement/PO/" + workflow.id + "_PO" + ".pdf"),
                                workflow.pm_project.code, definition.Element("form"),
                                Server.MapPath("~/Documents/Images/po_header.png"), definition);
                            Util.AddWorkflowDocument(new bpm_workflow_document
                            {
                                location = "~/Documents/Procurement/PO/" + workflow.id + "_PO" + ".pdf",
                                name = "Purchase Order Form",
                                workflow_id = workflow.id
                            });
                        }
                        catch (Exception e)
                        {
                            Utils.LogError(e);
                        }
                        break;
                    case 21:
                        //VPRF
                        try
                        {
                            new WorkflowDocumentsHelper().NewCreateVPRF(
                                Server.MapPath("~/Documents/Procurement/VPRF/" + workflow.id + "_VPRF" + ".pdf"),
                                workflow.pm_project.code, definition.Element("form"),
                                Server.MapPath("~/Documents/Images/vprf_header.png"), definition);
                            Util.AddWorkflowDocument(new bpm_workflow_document
                            {
                                location = "~/Documents/Procurement/VPRF/" + workflow.id + "_VPRF" + ".pdf",
                                name = "Vendor Payment Request Form",
                                workflow_id = workflow.id
                            });
                        }
                        catch (Exception e)
                        {
                            Utils.LogError(e);
                        }
                        break;
                    default:
                        break;
                }
            }

            private void NewProcurementProcess2(HRM.DAL.Models.admin_hrm_employee employee, Step step, XElement definition,
                FormCollection collection, HttpPostedFileBase[] files)
            {
                //Get the step Id
                int stepId = 0;
                XElement current_step_element_tag = null; //formerly current_step_element_tag is the current step element
                XElement next_step_element_tag = null; //formerly next_step_element_tag is the next step element
                                                       //init new_procurement and get the workflow id from the DB
                var new_procurement = new NewProcurement();
                var id = new Guid(collection["workflow_id"]);
                var bLWorkflow = new BLWorkflow(id);
                var workflow = bLWorkflow.GetWorkflow();
                Step next_step_class = null;//formerly next_step_class
                if (step != null)
                {
                    stepId = step.Id;
                    //old implementation
                    //current_step_element_tag = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);
                    current_step_element_tag = step.Definition;
                    //Get the next step
                    next_step_element_tag = current_step_element_tag.Element("next_step");
                    next_step_class = bLWorkflow.GetStep(Convert.ToInt32(next_step_element_tag.Value));
                }
                else
                {
                    stepId = -1;
                }
                //init email body variable and the notification email
                var email_body = "";
                var notification_email = new Email
                {
                    body = email_body,
                    subject = workflow.title + " - Action required - Workspace"
                };
                //notification_email.to = new List<string> { employee.emp_work_email };
                notification_email.IsHtml = true;
                /*
                 * Switch the steps
                 * NOTE: the default is approval
                 */
                var result = "";
                switch (stepId)
                {
                    case (int)NewProcurementProcessTag.ProcurementRequest:
                        //Add the new_procurement Items to the xml doc
                        new_procurement.AddItemsToProcure(definition, step, current_step_element_tag, collection, employee, id);
                        //upload supporting document
                        if (Request.Files.Count > 0)
                        {
                            try
                            {
                                var file_path = "~/Documents/new_procurement/SupportDoc/";
                                foreach (HttpPostedFileBase file in files)
                                {
                                    var location = Path.Combine(file_path, workflow.title.Replace(@"\", "").Replace(@"/", "") + "_" + file.FileName.Replace(@"\", string.Empty));
                                    if (!Directory.Exists(Server.MapPath(file_path)))
                                    {
                                        Directory.CreateDirectory(Server.MapPath(file_path));
                                    }
                                    file.SaveAs(Server.MapPath(Path.Combine(location)));

                                    Util.AddWorkflowDocument(new bpm_workflow_document { location = location, name = file.FileName, workflow_id = workflow.id });
                                }
                            }
                            catch (Exception ex)
                            {
                                string message = string.Format("{0}",
                                    ex.Message);
                                Utils.Log(message + " - AddFileForWorkflow");
                                Utils.LogError(ex);
                                // Exception raise = ex;
                            }
                        }
                        if (next_step_class.Definition.Element("type").Value.ToString() == "approval")
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, next_step_class, step, collection, definition, MailType.Approval);
                        }
                        break;
                    case (int)NewProcurementProcessTag.CountryDirectorApproval:
                        result = new_procurement.Approve(definition, step, current_step_element_tag, collection, employee, id, workflow);
                        if (result != "success")
                        {
                            ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                            return;
                        }
                        else
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, next_step_class, step, collection, definition, MailType.Assignment);
                        }
                        break;
                    case (int)NewProcurementProcessTag.AssignProcurementOfficer:
                        if (employee.emp_number == Util.GetProgramApprover(-1, 0))
                        {
                            if (collection["wf_assignment"] != null && collection["wf_assignment"] == "emp")
                            {
                                new_procurement.AddUserToSteps(new int[] { 8, 9, 10, 11, 12, 18, 19, 22 }, definition, Convert.ToInt32(collection["emp_number"]), next_step_class.Id);
                                //step = bLWorkflow.GetStep();
                                current_step_element_tag = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Id.ToString());
                                current_step_element_tag.Element("status").SetValue(1);

                                next_step_element_tag = current_step_element_tag.Element("next_step");
                                next_step_class = bLWorkflow.GetStep(Convert.ToInt32(next_step_element_tag.Value));
                                SendEmailForApproval(email_body, notification_email, workflow, id, employee, next_step_class, step, collection, definition, MailType.Attention);
                            }
                        }
                        break;
                    case (int)NewProcurementProcessTag.RequestForQuotation:
                        RFQ(workflow, collection, definition, employee, step, current_step_element_tag);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, next_step_class, step, collection, definition, MailType.Attention);
                        break;
                    case (int)NewProcurementProcessTag.UploadQuotation:
                        UploadQuotes(workflow, collection, definition, step, current_step_element_tag);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, next_step_class, step, collection, definition, MailType.Attention);
                        break;
                    case (int)NewProcurementProcessTag.PurchaseOrder:
                        new_procurement.CreatePO(definition, step, collection, current_step_element_tag);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, next_step_class, step, collection, definition, MailType.Approval);
                        break;
                    case (int)NewProcurementProcessTag.JobCompletion:
                        JobCompletion(workflow, collection, definition, step, current_step_element_tag, files);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, next_step_class, step, collection, definition, MailType.Attention);
                        break;
                    case (int)NewProcurementProcessTag.VendorPaymentRequestForm:
                        new_procurement.CreateVPRF(definition, step, current_step_element_tag, collection);
                        SendEmailForApproval(email_body, notification_email, workflow, id, employee, next_step_class, step, collection, definition, MailType.Approval);
                        break;
                    case (int)NewProcurementProcessTag.ThirdCountryDirectorApproval:
                        result = new_procurement.Approve(definition, step, current_step_element_tag, collection, employee, id, workflow);
                        if (result != "success")
                        {
                            ViewBag.message = "Your token is expired. Please click refresh to generate a new one.";
                            return;
                        }
                        else
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, next_step_class, step, collection, definition, MailType.Attention);
                        }
                        break;
                    default:
                        result = new_procurement.Approve(definition, step, current_step_element_tag, collection, employee, id, workflow);
                        if (result != "success")
                        {
                            ViewBag.message = result;
                            return;
                        }
                        else
                        {
                            SendEmailForApproval(email_body, notification_email, workflow, id, employee, next_step_class, step, collection, definition, MailType.Approval);
                        }
                        break;
                }
                if (collection["comment"] != null && collection["comment"] != "")
                {
                    //add comment to the particular step
                    current_step_element_tag.Element("comments").SetValue(collection["comment"].ToString());
                    //add comment to the whole request document
                    var comment = new XElement("comment");
                    comment.Add(new XElement("step", step.Id));
                    comment.Add(new XElement("employee_id", employee.emp_number));
                    comment.Add(new XElement("info", collection["comment"].ToString()));
                    comment.Add(new XElement("employee_name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
                    comment.Add(new XElement("created_at", DateTime.Now));

                    definition.Element("comments").Add(comment);
                }
                //set the user that completed the step if stepId != -1
                //step id -1 was used when Aflow assignment was not part of thte steps
                if (stepId != -1)
                {
                    current_step_element_tag.Element("created_at").SetValue(DateTime.Now);
                    current_step_element_tag.Element("created_by").SetValue(employee.emp_number);
                    current_step_element_tag.Element("created_by_name").SetValue(string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname));

                    workflow.workflow = definition.ToString();
                    _workflowService.Update(workflow);
                }
                else
                {
                    workflow.workflow = definition.ToString();
                    _workflowService.Update(workflow);
                }

                NotificationUtil.NewSendEmail(notification_email);

                new_process_document(step.Id, workflow, definition);
            }
        }
    }



