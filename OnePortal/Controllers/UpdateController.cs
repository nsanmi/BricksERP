using HRM.DAL.IService;
using HRM.DAL.Util;
using OnePortal.Helper;
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

namespace OnePortal.Controllers
{
    [Authorize]
    public class UpdateController:Controller
    {

        IWorkflowService _workflowService;
        IProcessService _processService;
        IVendorService _vendorService;
        ITransactionTokenService _transactionTokenService;
        IEmployeeService _employeeService;

        public UpdateController(IProcessService processService, IWorkflowService workflowService,IVendorService vendorService, ITransactionTokenService transactionTokenService, IEmployeeService employeeService)
        {
            _workflowService = workflowService;
            _processService = processService;
            _vendorService = vendorService;
            _transactionTokenService = transactionTokenService;
            _employeeService = employeeService;
        }

        public string UpdatePrAdmin()
        {
            var id = Guid.Parse("94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317");
            //get workflows <position pos_type="jti">19</position>
            var workflows = _workflowService.GetWorkflows().Where(e => e.process_id == id).ToList();
            foreach(var workflow in workflows)
            {
                try
                {
                    var definition = XElement.Parse(workflow.workflow);

                    var step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "19");
                    //step.Element("position").SetValue("18");
                    //step.Element("position").Attribute("pos_type").SetValue("jti");
                    //workflow.workflow = definition.ToString();
                    //_workflowService.Update(workflow);
                    if (step.Element("position").Value == "29")
                    {
                        step.Element("position").SetValue("14");
                        step.Element("position").Attribute("pos_type").SetValue("jti");
                        workflow.workflow = definition.ToString();
                        _workflowService.Update(workflow);
                    }







                }
                catch (Exception)
                {
                    return "Error";
                }
               
            }

            return  "Success";
        }
        public ActionResult MoveToStep(Guid id,int step)
        {
            try
            {
                //get workflows <position pos_type="jti">19</position>
                var workflow = _workflowService.GetWorkflows().FirstOrDefault(e => e.id == id);
                var definition = XElement.Parse(workflow.workflow);
                definition.Element("next_step").SetValue(step);
                workflow.workflow = definition.ToString();
                _workflowService.Update(workflow);


                var bLWorkflow = new BLWorkflow(id);
                workflow = bLWorkflow.GetWorkflow();
                definition = XElement.Parse(workflow.workflow);
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
            catch (Exception)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
        }
        public ActionResult Generate()
        {
            var id = Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3178");
            //get workflows <position pos_type="jti">19</position>
            var workflows = _workflowService.GetWorkflows().Where(e => e.process_id == id).ToList();
            foreach (var workflow in workflows)
            {
                var definition = XElement.Parse(workflow.workflow);


                if (definition.Element("next_step").Value == "6")
                {

                    new WorkflowDocumentsHelper().createTravelAdvance(Server.MapPath("~/Documents/Travel/Advance/" + workflow.title.Replace(' ', '_') + "_" + workflow.id + "_Advance" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/travel_advance_header.png"), workflow.admin_hrm_employee, workflow.created_at,definition);

                    //Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Advance/" + workflow.title.Replace(' ', '_') + "_" + workflow.id + "_Advance" + ".pdf", name = "Travel Advance Form", workflow_id = workflow.id });


                    new WorkflowDocumentsHelper().createTravelAuthorization(Server.MapPath("~/Documents/Travel/Authorization/" + workflow.title.Replace(' ', '_') + "_" + workflow.id + "_Auth" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/header.png"), workflow.admin_hrm_employee, workflow.created_at,definition);

                    //Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Authorization/" + workflow.title.Replace(' ', '_') + "_" + workflow.id + "_Auth" + ".pdf", name = "Travel Authorization Form", workflow_id = workflow.id });

                }

            }

            return View();

        }



        public string GeneratePO(Guid id)
        {
            var workflow = _workflowService.GetWorkflow(id);
            XElement definition = XElement.Parse(workflow.workflow);


            try
            {
                new WorkflowDocumentsHelper().createPurchaseOrder(Server.MapPath("~/Documents/Procurement/PO/" + workflow.title.Replace(' ', '_') + "_" + DateTime.Now.Second + "_" + workflow.id + "_PO" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/prf_header.png"),definition);

                Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Procurement/PO/" + workflow.title.Replace(' ', '_') + "_" + workflow.id + "_PO" + ".pdf", name = "Purchase Order", workflow_id = workflow.id });

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GenerateVPRF(Guid id)
        {
            var workflow = _workflowService.GetWorkflow(id);
            XElement definition = XElement.Parse(workflow.workflow);
            try
            {
                new WorkflowDocumentsHelper().createVPRF(Server.MapPath("~/Documents/Procurement/VPRF/" + workflow.title.Replace(' ', '_') + "_" + DateTime.Now.Second + "_" + workflow.id + "_VPRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/vprf_header.png"),definition);

                Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Procurement/VPRF/" + workflow.title.Replace(' ', '_') + "_" + workflow.id + "_VPRF" + ".pdf", name = "Vendor Payment Request Form", workflow_id = workflow.id });
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CompleteTransaction(Guid id)
        {
            var result = "";
            //var id = Guid.Parse("10471746-0236-4F79-BE27-8AA1CBFF2D92");
            //get workflows <position pos_type="jti">19</position>
            var workflows = _workflowService.GetWorkflows().Where(e => e.process_id == id).ToList();
            foreach (var workflow in workflows)
            {

               
                //var workflow = _workflowService.GetWorkflow(id);
                XElement definition = XElement.Parse(workflow.workflow);
                var current_step = definition.Element("next_step").Value;
                //var definition = XElement.Parse(workflow.workflow);
                if (current_step != "6") continue;
                var step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == current_step);
                step.Element("status").SetValue("1");


                workflow.status = 23;
                workflow.workflow = definition.ToString();
                _workflowService.Update(workflow);


                try
                {
                    new WorkflowDocumentsHelper().createPRFPaymentSchedule(Server.MapPath("~/Documents/Procurement_Pay/PRF/" + workflow.title.Replace(' ', '_') + "_" + DateTime.Now.Second + "_" + workflow.id + "_VPRF" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/vprf_header.png"),definition);

                    Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Procurement_Pay/PRF/" + workflow.title.Replace(' ', '_') + "_" + workflow.id + "_PS" + ".pdf", name = "Payment schedule", workflow_id = workflow.id });
                    result+= "success";
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }
            return result;
        }


        public string CompleteTravel()
        {
            var result = "";
            var ids = new Guid[] {Guid.Parse("A9BE0559-94E4-4A6E-AE46-74A1344DF7DF"),Guid.Parse("A9BE0559-94E4-4A6E-AE46-74A1344DF7EF") };
            var workflows = _workflowService.GetWorkflows().Where(e => ids.Contains(e.process_id)).ToList();
            foreach (var workflow in workflows)
            {
                XElement definition = XElement.Parse(workflow.workflow);
                var current_step = definition.Element("next_step").Value;
                //var definition = XElement.Parse(workflow.workflow);
                if (current_step != "6") continue;
                var step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == current_step);
                step.Element("status").SetValue("1");


                workflow.status = 23;
                workflow.workflow = definition.ToString();
                _workflowService.Update(workflow);

                try
                {

                    new WorkflowDocumentsHelper().createTravelAdvance(Server.MapPath("~/Documents/Travel/Advance/" + workflow.title.Replace(' ', '_') + "_" + workflow.id + "_Advance" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/travel_advance_header.png"), workflow.admin_hrm_employee, workflow.created_at,definition);

                Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Advance/" + workflow.title.Replace(' ', '_') + "_" + workflow.id + "_Advance" + ".pdf", name = "Travel Advance Form", workflow_id = workflow.id });

                //Travel authorization
                new WorkflowDocumentsHelper().createTravelAuthorization(Server.MapPath("~/Documents/Travel/Authorization/" + workflow.title.Replace(' ', '_') + "_" + workflow.id + "_Auth" + ".pdf"), workflow.pm_project.code, definition.Element("form"), Server.MapPath("~/Documents/Images/header.png"), workflow.admin_hrm_employee, workflow.created_at,definition);

                Util.AddWorkflowDocument(new bpm_workflow_document { location = "~/Documents/Travel/Authorization/" + workflow.title.Replace(' ', '_') + "_" + workflow.id + "_Auth" + ".pdf", name = "Travel Authorization Form", workflow_id = workflow.id });
                    result += "success";
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }

            return result;
        }
    }
}