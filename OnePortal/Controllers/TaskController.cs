using HRM.DAL.IService;
using HRM.DAL.Models;
using HRM.DAL.Util;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnePortal.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        ITaskService _taskService;
        IActivityService _activityService;
        IEmployeeService _employeeService;
        ICommentService _commentService;
        public TaskController(ITaskService taskService, IActivityService activityService,IEmployeeService employeeService,ICommentService commentService)
        {
            _taskService = taskService;
            _activityService = activityService;
            _employeeService = employeeService;
            _commentService = commentService;
        }
        // GET: Task
        public ActionResult Tasks(int id)
        {
            return View(_activityService.GetActivity(id));
        }

        [HttpPost]
        public ActionResult AddTask(FormCollection collection)
        {
            var user_id = User.Identity.GetUserId();
            var empl = _employeeService.GetEmployeeByUserId(user_id);


            var activity_id = Convert.ToInt32(collection["activity_id"]);
            var task = collection["task"];
            var description = collection["description"];



            //var ex_date = Convert.ToDateTime(collection["expected_start_date"]);
            //var due_date = Convert.ToDateTime(collection["due_date"]);
            //var start_date = Convert.ToDateTime(collection["start_date"]);
            //var end_date = Convert.ToDateTime(collection["end_date"]);

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

            var pr_task = _taskService.AddTask(new pm_project_task
            {
                task = task,
                objective_activity_id = activity_id,
                description = description,
                expected_start_date = ex_date,
                due_date = due_date,
                start_date = start_date,
                end_date = end_date,
                status = status
            });

            var employees = collection["employee"];
            if (employees != null && employees != string.Empty)
            {
                foreach (var employee in employees.Split(','))
                {
                    var emp_number = Convert.ToInt32(employee.ToString());
                    _taskService.AddAdmin(new pm_task_admin { task_id = pr_task.id, emp_number = emp_number });

                    var emp = _employeeService.GetEmployee(emp_number);
                    var template = EmailUtil.GetTemplate("objective_creation");

                    template.Replace("{objective}", collection["objective"]);
                    template.Replace("{assigned}", string.Format("{0} {1}", empl.emp_lastname, empl.emp_firstname));
                    template.Replace("{name}", string.Format("{0} {1}", emp.emp_lastname, emp.emp_firstname));
                    template.Replace("{workspace_name}", "WorkSpace");

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

            return RedirectToAction("Tasks", new { id = activity_id });
        }

        public ActionResult Task(int id)
        {
            var comments = _commentService.GetItemComments(id, "task");
            ViewBag.comments = comments;
            return View(_taskService.GetTask(id));
        }

        public ActionResult Edit(int id)
        {
            return View(_taskService.GetTask(id));
        }

        [HttpPost]
        public ActionResult Edit()
        {
            var user_id = User.Identity.GetUserId();
            var empl = _employeeService.GetEmployeeByUserId(user_id);


            var collection = Request.Form;

            var task = collection["task"];
            var description = collection["description"];


            //var ex_date = Convert.ToDateTime(collection["expected_start_date"]);
            //var due_date = Convert.ToDateTime(collection["due_date"]);
            //var start_date = Convert.ToDateTime(collection["start_date"]);
            //var end_date = Convert.ToDateTime(collection["end_date"]);

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
            var activity_id = Convert.ToInt32(collection["activity_id"]);
            var id = Convert.ToInt32(collection["id"]);

            var pr_task = new pm_project_task
            {
                id = id,
                task = task,
                objective_activity_id = activity_id,
                description = description,
                expected_start_date = ex_date,
                due_date = due_date,
                start_date = start_date,
                end_date = end_date,
                status = status
            };
            var employees = collection["employee"];

            _taskService.UpdateTask(pr_task);
            if (employees != null && employees != string.Empty)
            {
                foreach (var employee in employees.Split(','))
                {
                    var emp_number = Convert.ToInt32(employee.ToString());
                    _taskService.AddAdmin(new pm_task_admin { task_id = id, emp_number = emp_number });


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

            return RedirectToAction("Edit", new { id = id });

        }

        [HttpPost]
        public ActionResult AddDeliverable(FormCollection collection)
        {
            var task_id = Convert.ToInt32(collection["task_id"]);

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            var deliverable = new pm_task_deliverables
            {
                name = collection["name"],
                task_id = task_id,
                status = Convert.ToInt32(collection["status"]),
                deliverable_type = Convert.ToInt32(collection["deliverable_type"]),
                created_by = employee.emp_number,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                updated_by = employee.emp_number
            };
            _taskService.AddDeliverable(deliverable);

            return RedirectToAction("Edit", new { id = task_id });
        }

        public ActionResult Files(int id, int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.task = _taskService.GetTask(id);
            return View(_taskService.GetTaskFiles(id).OrderByDescending(e => e.uploaded_at).ToPagedList(pageIndex, pageSize));
        }


        public ActionResult MyTasks(int? page)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
           
            return View(_taskService.GetUserTask(employee.emp_number).OrderByDescending(e => e.end_date).ToPagedList(pageIndex, pageSize));
        }

        public ActionResult DeleteTask(int id)
        {
            _taskService.Delete(id);
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
