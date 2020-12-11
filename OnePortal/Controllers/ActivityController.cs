using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRM.DAL.IService;
using HRM.DAL.Models;
using PagedList;
using Microsoft.AspNet.Identity;
using HRM.DAL.Util;

namespace OnePortal.Controllers
{
    [Authorize]
    public class ActivityController : Controller
    {
        IActivityService _activityService;
        IProjectService _projectService;
        IEmployeeService _employeeService;
        public ActivityController(IActivityService activityService, IProjectService projectService,IEmployeeService employeeService)
        {
            _activityService = activityService;
            _projectService = projectService;
            _employeeService = employeeService;
        }
        // GET: Activity
        public ActionResult Activities(int id)
        {
            return View(_projectService.GetObjective(id));
        }

        [HttpPost]
        public ActionResult AddActivity(FormCollection collection)
        {
            var user_id = User.Identity.GetUserId();
            var empl = _employeeService.GetEmployeeByUserId(user_id);

            var objective_id = Convert.ToInt32(collection["objective_id"]);
            var activity = collection["activity"];
            var description = collection["description"];
            var dt = collection["expected_start_date"];

            DateTime? ex_date =  null;
            if (collection["expected_start_date"] != null && collection["expected_start_date"] !="")
                ex_date = Convert.ToDateTime(collection["expected_start_date"]);
            DateTime? due_date = null;
            if (collection["due_date"] != null && collection["due_date"] != "")
                due_date = Convert.ToDateTime(collection["due_date"]);

            DateTime? start_date = null;
            if (collection["start_date"] != null && collection["start_date"] != "")
                start_date = Convert.ToDateTime(collection["start_date"]);

            DateTime? end_date = null;
            if (collection["end_date"] != null && collection["end_date"] != "")
                end_date = Convert.ToDateTime(collection["end_date"]);

            var status = Convert.ToInt32(collection["status"]);
            var num = collection["num"].ToString();

            var pr_activity = _activityService.AddActivity(new pm_project_objective_activity
            {
                activity = activity,
                objective_id = objective_id,
                description = description,
                expected_start_date = ex_date,
                due_date = due_date,
                start_date = start_date,
                end_date = end_date,
                status = status,
                requirements = collection["requirements"],
                completion = Convert.ToInt32(collection["completion"]),
                performance_indicator = collection["performance_indicator"],
                target = collection["target"],
                output=collection["output"],num=num

            });

            var employees = collection["employee"];
            if (employees != null && employees != string.Empty)
            {
                foreach (var employee in employees.Split(','))
                {
                    var emp_number = Convert.ToInt32(employee.ToString());
                    _activityService.AddActivityAdmin(new pm_activity_admin { activity_id = pr_activity.id, emp_number = emp_number });


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

                    NotificationUtil.SendNotifications(new int[] { Convert.ToInt32(employee) }, template);
                }
            }

            return RedirectToAction("Objective","Project", new { id = objective_id });
        }

        public ActionResult Edit(int id)
        {
            return View(_activityService.GetActivity(id));
        }

        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {

            var user_id = User.Identity.GetUserId();
            var empl = _employeeService.GetEmployeeByUserId(user_id);

            var objective_id = Convert.ToInt32(collection["objective_id"]);
            var activity = collection["activity"];
            var description = collection["description"];
            var dt = collection["expected_start_date"];

            DateTime? ex_date = null;
            if (collection["expected_start_date"] != null && collection["expected_start_date"] != "")
                ex_date = Convert.ToDateTime(collection["expected_start_date"]);
            DateTime? due_date = null;
            if (collection["due_date"] != null && collection["due_date"] != "")
                due_date = Convert.ToDateTime(collection["due_date"]);

            DateTime? start_date = null;
            if (collection["start_date"] != null && collection["start_date"] != "")
                start_date = Convert.ToDateTime(collection["start_date"]);

            DateTime? end_date = null;
            if (collection["end_date"] != null && collection["end_date"] != "")
                end_date = Convert.ToDateTime(collection["end_date"]);

            var status = Convert.ToInt32(collection["status"]);
            var num = collection["num"].ToString();


            _activityService.Update(new pm_project_objective_activity
            {
                id=Convert.ToInt32(collection["id"]),
                activity = activity,
                objective_id = objective_id,
                description = description,
                expected_start_date = ex_date,
                due_date = due_date,
                start_date = start_date,
                end_date = end_date,
                status = status,
                requirements = collection["requirements"],
                completion = Convert.ToInt32(collection["completion"]),
                performance_indicator = collection["performance_indicator"],num=num,
                target = collection["target"],
                output = collection["output"]

            });

        
            var employees = collection["employee"];
            if (employees != null && employees != string.Empty)
            {
                foreach (var employee in employees.Split(','))
                {
                    var emp_number = Convert.ToInt32(employee.ToString());
                    _activityService.AddActivityAdmin(new pm_activity_admin { activity_id = Convert.ToInt32(collection["id"]), emp_number = emp_number });


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

                    NotificationUtil.SendNotifications(new int[] { Convert.ToInt32(employee) }, template);
                }
            }







            return Redirect(Request.UrlReferrer.ToString());
            // return RedirectToAction("Edit",new { id = Convert.ToInt32(collection["id"]) });
        }


        public ActionResult Workplan()
        {
            return View();
        }

        public ActionResult Files(int id, int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.activity = _activityService.GetActivity(id);
            return View(_activityService.GetActivityFiles(id).OrderByDescending(e => e.uploaded_at).ToPagedList(pageIndex, pageSize));
        }

        public ActionResult MyActivities(int? page)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
           
            return View(_activityService.GetUserActivities(employee.emp_number).OrderByDescending(e => e.end_date).ToPagedList(pageIndex, pageSize));
            
        }

        public ActionResult RemoveAdmin(int emp_number, int activity_id)
        {
            _activityService.RemoveActivityAdmin(new pm_activity_admin
            {
                emp_number = emp_number,
                activity_id = activity_id
            });
            ViewBag.message = "Changes made successfully";
            return RedirectToAction("Edit", new { id = activity_id });
        }


        public ActionResult DeleteActivity(int id)
        {
            _activityService.Delete(id);
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult AddBudget(pm_activity_budget budget)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            budget.created_at = DateTime.Now;
            budget.created_by = employee.emp_number;

            _activityService.AddBudget(budget);
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult DeleteBudget(int id)
        {
            _activityService.DeleteBudgetItem(id);
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}