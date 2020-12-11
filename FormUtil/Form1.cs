using HRM.DAL.Service;
using HRM.DAL.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WorkFlow.DAL;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.Logic;
using WorkFlow.DAL.Service;
namespace FormUtil
{
    public partial class Form1 : Form
    {
        WorkflowService _workflowService = new WorkflowService();
        EmployeeService _employeeService = new EmployeeService();
        TransactionTokenService _transactionTokenService=new TransactionTokenService();
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_reminder_Click(object sender, EventArgs e)
        {
            var workflows = _workflowService.GetWorkflows().Where(x => x.status != 23);
            foreach(var workflow in workflows)
            {
                var bLWorkflow = new BLWorkflow(workflow.id);
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




                switch (nxt_step.Definition.Element("position").Attribute("pos_type").Value)
                {
                    case "sup":
                        //get the supervisor of the person that applied for the request
                        var requestor = _employeeService.GetEmployee(workflow.created_by);
                        var supervisors = requestor.admin_hrm_emp_reportto1.Select(x => x.erep_sup_emp_number).ToArray();

                        notification_email.to = new List<string> { requestor.admin_hrm_emp_reportto1.FirstOrDefault().admin_hrm_employee.emp_work_email };

                        NotificationUtil.SendNotifications(supervisors, "Workflow - Action needed");
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

                        NotificationUtil.SendNotifications(ids, "Workflow - Action needed");

                        foreach (var emp in employees)
                        {
                            _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = emp.emp_number, workflow_id = workflow.id, pending = 1, created_at = DateTime.Now });
                            mail_list.Add(emp.emp_work_email);
                        }

                        notification_email.to = mail_list;

                        break;
                    case "dir":
                        //get the subunit of the requestor
                        var directorate = _employeeService.GetDirectorate(workflow.created_by);

                        var emps = _employeeService.GetEmployees().Where(x => x.admin_hrm_emp_job_record.job_title_id == directorate.head);
                        var emps_ids = emps.Select(x => x.emp_number).ToArray();

                        var list = new List<string>();

                        NotificationUtil.SendNotifications(emps_ids, "Workflow - Action needed");

                        foreach (var emp in emps)
                        {
                            _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = emp.emp_number, workflow_id = workflow.id, pending = 1, created_at = DateTime.Now });
                            list.Add(emp.emp_work_email);
                        }
                        notification_email.to = list;
                        break;
                    case "emp":
                        //get the subunit of the requestor
                        var empl = _employeeService.GetEmployee(workflow.created_by);


                        var em_list = new List<string>();
                        em_list.Add(empl.emp_work_email);
                        NotificationUtil.SendNotifications(new int[] { empl.emp_number }, "Workflow - Action needed");

                        _workflowService.AddUserToWorkflow(new bpm_workflow_employee { emp_number = empl.emp_number, workflow_id = workflow.id, pending = 1, created_at = DateTime.Now });

                        notification_email.to = em_list;
                        break;
                    default:
                        break;
                }


                NotificationUtil.SendEmail(notification_email);

















            }
        }
    }
}
