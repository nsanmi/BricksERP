using HRM.DAL.IService;
using HRM.DAL.Models;
using HRM.DAL.Util;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnePortal.Controllers
{
    public class PMApiController : ApiController
    {
        IProjectService _projectService;
        IEmployeeService _employeeService;
        ITaskService _taskService;
        public PMApiController(IProjectService projectService, IEmployeeService employeeService, ITaskService taskService)
        {
            _projectService = projectService;
            _taskService = taskService;
            _employeeService = employeeService;
        }

        [HttpPost]
        public void PostProject(HttpRequestMessage message)
        {

            var user_id = User.Identity.GetUserId();
            var empl = _employeeService.GetEmployeeByUserId(user_id);

            var post_text = message.Content.ReadAsStringAsync().Result;
            var collection = JObject.Parse(post_text);

            var name = collection["name"].ToString();
            var description = WebUtility.HtmlEncode(collection["description"].ToString());
            var code = collection["code"].ToString();
            var status = Convert.ToInt32(collection["status"]);
            var sub_parent = Convert.ToInt32(collection["sub_parent"].ToString());
            var num = collection["num"].ToString();

            var project = _projectService.AddProject(new pm_project
            {

                name = name,
                description = description,
                code = code,
                status = status,
                created_at = DateTime.Now,sub_parent=sub_parent,num=num
            });

            var employees = collection["employee"].ToString();
            if (employees != null && employees != string.Empty)
            {
                foreach (var employee in JArray.Parse(employees))
                {
                    var emp_number = Convert.ToInt32(employee.ToString());
                    _projectService.AddProjectAdmin(new pm_project_admin { project_id = project.id, emp_number = emp_number });

                    string Domain = "http://worspace.mgic-nigeria.org";//Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    Domain += "/project/home/" + project.id;

                    var emp = _employeeService.GetEmployee(emp_number);
                    var template = EmailUtil.GetTemplate("project_creation");

                    template = template.Replace("{project}", name);
                    template = template.Replace("{assigned}", string.Format("{0} {1}", empl.emp_lastname, empl.emp_firstname));
                    template = template.Replace("{name}", string.Format("{0} {1}", emp.emp_lastname, emp.emp_firstname));
                    template = template.Replace("{workspace_name}", "WorkSpace");

                    template += string.Format(" <a href='{0}'>Click here</a>", Domain);

                    var email = new Email
                    {
                        body = template,
                        subject = "Project - Workspace"
                    };


                    email.to = new List<string> { emp.emp_work_email };
                    email.IsHtml = true;

                    NotificationUtil.SendEmail(email);

                    NotificationUtil.SendNotifications(new int[] { Convert.ToInt32(employee) }, template, Domain);
                }
            }

            
        }

        [HttpPost]
        public void Postobjective(HttpRequestMessage message)
        {
            var post_text = message.Content.ReadAsStringAsync().Result;
            var collection = JObject.Parse(post_text);

            var user_id = User.Identity.GetUserId();
            var empl = _employeeService.GetEmployeeByUserId(user_id);

            var num = collection["num"].ToString();


            var objective = _projectService.AddProjectObjective(new pm_project_strategic_objective
            {
                objectve = collection["objective"].ToString(),
                code = collection["code"].ToString(),
                description = WebUtility.HtmlEncode(collection["description"].ToString()),
                project_id = Convert.ToInt32(collection["project_id"]),num=num
            });

            var employees = collection["employee"].ToString();
            if (employees != null && employees != string.Empty)
            {
                foreach (var employee in JArray.Parse(employees))
                {
                    var emp_number = Convert.ToInt32(employee.ToString());
                    _projectService.AddObjectiveAdmin(new pm_objective_admin { objective_id = objective.id, emp_number = emp_number });


                    var emp = _employeeService.GetEmployee(emp_number);
                    var template = EmailUtil.GetTemplate("objective_creation");

                    template = template.Replace("{objective}", collection["objective"].ToString());
                    template = template.Replace("{assigned}", string.Format("{0} {1}", empl.emp_lastname, empl.emp_firstname));
                    template = template.Replace("{name}", string.Format("{0} {1}", emp.emp_lastname, emp.emp_firstname));
                    template = template.Replace("{workspace_name}", "WorkSpace");

                    var email = new Email
                    {
                        body = template,
                        subject = "Objective - Workspace"
                    };


                    email.to = new List<string> { emp.emp_work_email };
                    email.IsHtml = true;

                    NotificationUtil.SendEmail(email);
                }
            }

        }

        [HttpPut]
        public void Putobjective(HttpRequestMessage message)
        {
            var post_text = message.Content.ReadAsStringAsync().Result;
            var collection = JObject.Parse(post_text);

            var user_id = User.Identity.GetUserId();
            var empl = _employeeService.GetEmployeeByUserId(user_id);

            var num = collection["num"].ToString();

            _projectService.UpdateObjective(new pm_project_strategic_objective
            {
                id = Convert.ToInt32(collection["id"]),
                objectve = collection["objective"].ToString(),
                code = collection["code"].ToString(),
                description = WebUtility.HtmlEncode(collection["description"].ToString()),
                project_id = Convert.ToInt32(collection["project_id"]),num=num
                
            });

            var employees = collection["employee"].ToString();
            if (employees != null && employees != string.Empty)
            {
                foreach (var employee in JArray.Parse(employees))
                {
                    var emp_number = Convert.ToInt32(employee.ToString());
                    _projectService.AddObjectiveAdmin(new pm_objective_admin { objective_id = Convert.ToInt32(collection["id"]), emp_number = emp_number });


                    var emp = _employeeService.GetEmployee(emp_number);
                    var template = EmailUtil.GetTemplate("objective_creation");

                    template = template.Replace("{objective}", collection["objective"].ToString());
                    template = template.Replace("{assigned}", string.Format("{0} {1}", empl.emp_lastname, empl.emp_firstname));
                    template = template.Replace("{name}", string.Format("{0} {1}", emp.emp_lastname, emp.emp_firstname));
                    template = template.Replace("{workspace_name}", "WorkSpace");

                    var email = new Email
                    {
                        body = template,
                        subject = "Objective - Workspace"
                    };


                    email.to = new List<string> { emp.emp_work_email };
                    email.IsHtml = true;

                    NotificationUtil.SendEmail(email);
                }
            }

        }

    }
}
