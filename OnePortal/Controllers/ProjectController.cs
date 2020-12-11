using HRM.DAL.IService;
using HRM.DAL.Models;
using HRM.DAL.Util;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OnePortal.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        IProjectService _projectService;
        IEmployeeService _employeeService;
        ITaskService _taskService;
        public ProjectController(IProjectService projectService,IEmployeeService employeeService, ITaskService taskService)
        {
            _projectService = projectService;
            _taskService = taskService;
            _employeeService = employeeService;
        }
        // GET: Project
        public ActionResult Manage(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var projects=_projectService.GetProjects().Where(e=>e.timesheet_only==0).OrderByDescending(e=>e.id).ToPagedList(pageIndex, pageSize);
            return View(projects);
        }

        [HttpPost]
        public ActionResult Manage(FormCollection collection)
        {
            var user_id = User.Identity.GetUserId();
            var empl = _employeeService.GetEmployeeByUserId(user_id);

            var name = collection["name"];
            var description = collection["description"];
            var code = collection["code"];
            var status = Convert.ToInt32(collection["status"]);
            var timesheet_only = Convert.ToInt32(collection["timesheet_only"]);

            var project=   _projectService.AddProject(new pm_project
            {
                
                name = name,
                description = description,
                code = code,
                status = status,
                timesheet_only = timesheet_only,
                created_at = DateTime.Now
            });

            var employees = collection["employee"];
            if (employees != null && employees != string.Empty)
            {
                foreach (var employee in employees.Split(','))
                {
                    var emp_number = Convert.ToInt32(employee.ToString());
                    _projectService.AddProjectAdmin(new pm_project_admin { project_id = project.id, emp_number = emp_number });
                    //generate the url of the email
                    string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    Domain += "/project/home/" + project.id;


                    var emp = _employeeService.GetEmployee(emp_number);
                    var template =EmailUtil.GetTemplate("project_creation");

                    template=template.Replace("{project}", name);
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
                    
                    NotificationUtil.SendNotifications(new int[] { Convert.ToInt32(employee) }, template,Domain);

                }
            }


            ViewBag.message = "Project created successfully.";


            return RedirectToAction("Manage");
        }

        public ActionResult Edit(int id)
        {
            ViewBag.project = _projectService.GetProject(id);
            return View();
        }

        public ActionResult EditObjective(int id)
        {
            return View(_projectService.GetObjective(id));
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection collection)
        {
            var user_id = User.Identity.GetUserId();
            var empl = _employeeService.GetEmployeeByUserId(user_id);


            var project_id = Convert.ToInt32(collection["project_id"]);
            var name = collection["name"];
            var description = WebUtility.HtmlEncode(collection["description"].ToString()) ;
            var code = collection["code"];
            var status = Convert.ToInt32(collection["status"]);
            var timesheet_only = Convert.ToInt32(collection["timesheet_only"]);
            var num = collection["num"].ToString();
            int? sub_parent = null;
            if (collection["sub_parent"] != null)
            {
                sub_parent = Convert.ToInt32(collection["sub_parent"]);
            }
            _projectService.UpdateProject(new pm_project
            {
                id = project_id,
                name = name,
                description = description,
                code = code,
                status = status,
                timesheet_only = timesheet_only,
                updated_at = DateTime.Now,
                num =num,
                sub_parent=sub_parent
            });

            var employees =  collection["employee"];
            if (employees != null && employees != string.Empty)
            {
                foreach(var employee in employees.Split(','))
                {
                    var emp_number = Convert.ToInt32(employee.ToString());
                    _projectService.AddProjectAdmin(new pm_project_admin { project_id = project_id, emp_number =emp_number});

                    var emp = _employeeService.GetEmployee(emp_number);
                    var template = EmailUtil.GetTemplate("project_creation");

                    template = template.Replace("{project}", name);
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

                    NotificationUtil.SendNotifications(new int[] {Convert.ToInt32(employee) }, template);
                }
            }

            ViewBag.message = "Project Updated successfully.";
            return RedirectToAction("Edit", new { id = project_id });
        }

        [HttpPost]
        public ActionResult FundingPeriod(FormCollection collection)
        {
            var project_id = Convert.ToInt32(collection["project_id"]);

            var funder = Convert.ToInt32(collection["funder"]);
            var start_date = Convert.ToDateTime(collection["start_date"]);
            var end_date = Convert.ToDateTime(collection["end_date"]);
            var description = collection["description"];

            _projectService.AddFundingPeriod(new pm_project_funding_period
            {
                funder_id = funder,
                project_id = project_id,
                start_date = start_date,
                end_date = end_date,
                description=description
            });

            return RedirectToAction("Edit",new { id = project_id });
        }


        public ActionResult RemoveAdmin(int emp_number,int project_id)
        {
            _projectService.RemoveAdmin(new pm_project_admin
            {
                emp_number = emp_number,
                project_id = project_id
            });
            ViewBag.message = "Changes made successfully";
            return RedirectToAction("Edit", new { id = project_id });
        }

        public ActionResult RemoveOAdmin(int emp_number, int objective_id)
        {
            _projectService.RemoveObjectiveAdmin(new pm_objective_admin
            {
                emp_number = emp_number,
                objective_id = objective_id
            });
            ViewBag.message = "Changes made successfully";
            return RedirectToAction("EditObjective", new { id = objective_id });
        }


        public ActionResult Home(int id)
        {
            var project = _projectService.GetProject(id);
            return View(project);
        }


        [HttpPost]
        public ActionResult AddObjective(FormCollection collection)
        {
            var user_id = User.Identity.GetUserId();
            var empl = _employeeService.GetEmployeeByUserId(user_id);

            var objective = _projectService.AddProjectObjective(new pm_project_strategic_objective
            {
                objectve = collection["objective"],
                code = collection["code"],
                description = collection["description"],
                project_id = Convert.ToInt32(collection["project_id"])
            });

            var employees = collection["employee"];
            if (employees != null && employees != string.Empty)
            {
                foreach (var employee in employees.Split(','))
                {
                    var emp_number = Convert.ToInt32(employee.ToString());
                    _projectService.AddObjectiveAdmin(new pm_objective_admin { objective_id = objective.id, emp_number = emp_number });


                    var emp = _employeeService.GetEmployee(emp_number);
                    var template = EmailUtil.GetTemplate("objective_creation");

                    template = template.Replace("{objective}", collection["objective"]);
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

            return RedirectToAction("Home",new { id = Convert.ToInt32(collection["project_id"]) });
        }


        public ActionResult Objective(int id)
        {
            return View(_projectService.GetObjective(id));
        }


        public ActionResult ProjectSpace(int? page, int? task_id)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            return View(_taskService.GetUserTask(employee.emp_number).OrderByDescending(e => e.end_date).ToPagedList(pageIndex, pageSize));
        }

        public ActionResult Files(int id,int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.project = _projectService.GetProject(id);
            return View(_projectService.GetProjectFiles(id).OrderByDescending(e=>e.uploaded_at).ToPagedList(pageIndex, pageSize));
        }

        public ActionResult WorkPlan(int id)
        {

            return View(_projectService.GetProject(id));
        }

        public ActionResult WorkPlanO(int id)
        {

            return View(_projectService.GetObjective(id));
        }


        public ActionResult ObjectiveFiles(int id, int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.objective = _projectService.GetObjective(id);
            return View(_projectService.GetObjectiveFiles(id).OrderByDescending(e => e.uploaded_at).ToPagedList(pageIndex, pageSize));
        }


        public ActionResult MyProjects(int? page)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var projects = _projectService.GetUserProjects(employee.emp_number).Where(e => e.timesheet_only == 0 ).OrderByDescending(e => e.id).ToPagedList(pageIndex, pageSize);
            return View(projects);
        }

        public ActionResult MySubProjects(int? page)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var projects = _projectService.GetUserSubProjects(employee.emp_number).Where(e => e.timesheet_only == 0).OrderByDescending(e => e.id).ToPagedList(pageIndex, pageSize);
            return View(projects);
        }

        public ActionResult MyObjectives(int? page)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var objectives = _projectService.GetUserObjectives(employee.emp_number).OrderByDescending(e => e.id).ToPagedList(pageIndex, pageSize);
            return View(objectives);
        }

        public ActionResult DeleteProject(int id)
        {
            _projectService.DeleteProject(id);

            return RedirectToAction(Request.UrlReferrer.ToString());
        }

        public ActionResult DeleteObjective(int id, int project_id)
        {
            _projectService.DeleteObjective(id);

            return RedirectToAction("home", new { id = project_id });//RedirectToAction(Request.UrlReferrer.ToString());
        }
    }
}