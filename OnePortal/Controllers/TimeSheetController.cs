using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRM.DAL.Util;
using HRM.DAL.Models;
using HRM.DAL.IService;
using PagedList;
using Microsoft.AspNet.Identity;
using OnePortal.Helper;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using OnePortal.Models.ViewModels;
using System.Data;

namespace OnePortal.Controllers
{
    [Authorize]
    public class TimeSheetController : Controller
    {
        ITimesheetService _timesheetService;
        IEmployeeService _employeeService;
        public TimeSheetController(ITimesheetService timesheetService, IEmployeeService employeeService)
        {
            _timesheetService = timesheetService;
            _employeeService = employeeService;
        }

        [PermissionFilter(permission = "timesheet.edit")]
        public ActionResult Edit(int id)
        {
            var timesheet = _timesheetService.GetTimesheet(id);
            
            var year = timesheet.start_date.Year;
            var month = timesheet.start_date.Month;
            ViewBag.month = month;
            ViewBag.year = year;
            var timeSheetDays = DateUtil.indexMonth(year, month);
            ViewBag.timeSheetDays = timeSheetDays;
            ViewBag.first_week = timeSheetDays.FirstOrDefault().weekOfMonth;
            ViewBag.last_week=timeSheetDays.LastOrDefault().weekOfMonth;
            ViewBag.timesheet_id = id;
            var timesheet_items = _timesheetService.GetTimesheets(month, year, timesheet.emp_number);
            var time_items = new List<TimesheetItem>();
            foreach(var item in timesheet_items)
            {
                time_items.Add(new TimesheetItem(item));
            }
            ViewBag.timesheet_items =  time_items;
            return View();
        }

        [HttpPost]
        [PermissionFilter(permission = "timesheet.posttimesheet")]
        public ActionResult PostTimesheet(FormCollection form_collection)
        {
         
            var list = form_collection.AllKeys.AsQueryable();
            var timesheet_id = Convert.ToInt32(Request["timesheet_id"]);
            var timesheet = _timesheetService.GetTimesheet(timesheet_id);
            var time_sheets = new List<admin_hrm_timesheet_item>();
            foreach (var key in form_collection.AllKeys)
            {
                var value = Request[key];
                if (value == string.Empty) continue;
                //skip all the request data that starts with alphabets
                if (key.StartsWith("p") || key.StartsWith("a") || key.StartsWith("c") || key.StartsWith("t"))
                {
                    continue;
                }
                var item = new admin_hrm_timesheet_item();
                
                var key_split = key.Split('_');
                if (key_split.Count() < 3) continue;
                item.date = DateTime.Parse(key_split[1]);
                var activity_key = "activity_" + key_split[0] + "_" + key_split[2];

                item.activity_id =Convert.ToInt32(Request[activity_key]);
                item.project_id = Convert.ToInt32(Request["project_" + key_split[0] + "_" + key_split[2]]);
                item.duration = Convert.ToInt32(value);
                item.timesheet_id = timesheet_id;
                item.emp_number = timesheet.emp_number;
                item.comment = Request["comment_" + key];

                time_sheets.Add(item);
            }
            _timesheetService.AddTimesheetItem(time_sheets);


            //add log for the action 
            var log = new admin_hrm_timesheet_action_log
            {
                action = "SAVED",
                date_time = DateTime.Now,
                performed_by = timesheet.emp_number,
                timesheet_id = timesheet.timesheet_id
            };
            _timesheetService.AddTimesheetLog(log);

            var id = timesheet_id;
            if (User.IsInRole("sub"))
            {
                return RedirectToAction("MyTimesheet", new { id });
            }
            else if (User.IsInRole("sup") || User.IsInRole("admin"))
            {
                return RedirectToAction("ViewTimesheet", new { id=id });
            }
            else
            {
                return RedirectToAction("NotFound", "Home");
            }
           
           
        }

        [PermissionFilter(permission = "timesheet.viewtimesheet")]
        public ActionResult ViewTimesheet(int? page,string search,int? id)
        {

            if (id.HasValue)
            {
                var timesheet = _timesheetService.GetTimesheet(id.Value);

                ViewBag.id = id.Value;
                ViewBag.name = string.Format("{0} {1}", timesheet.admin_hrm_employee.emp_lastname, timesheet.admin_hrm_employee.emp_firstname) + " | " + timesheet.start_date.ToString("MMMM - yyyy");
                ViewBag.status = timesheet.state;
            }
                
            var user_id= User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            var subs = employee.admin_hrm_emp_reportto.Select(e => e.erep_sub_emp_number).ToList();
            
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<admin_hrm_timesheet> timesheets = null;

            if (search== null || search==string.Empty || search.ToLower()=="all")
                timesheets = _timesheetService.GetTimesheets().Where(e=>subs.Contains(e.emp_number) && e.state !="NOT SUBMITTED" && e.state == "SUBMITTED").OrderByDescending(e=>e.timesheet_id).ToPagedList(pageIndex,pageSize);
            else 
                timesheets = _timesheetService.GetTimesheets().Where(e=>e.state==search.ToUpper() && subs.Contains(e.emp_number)).OrderByDescending(e => e.timesheet_id).ToPagedList(pageIndex, pageSize);

            return View(timesheets);
        }

        [PermissionFilter(permission = "timesheet.mytimesheet")]
        public ActionResult MyTimesheet(int? id)
        {
          
            if (id.HasValue)
            {
                var timesheet = _timesheetService.GetTimesheet(id.Value);
                ViewBag.timesheet = timesheet;
                ViewBag.month = timesheet.start_date.Month;
                ViewBag.year = timesheet.start_date.Year;
                return View();
            }
            else
            {
                var user_id = User.Identity.GetUserId();
                var employee = _employeeService.GetEmployeeByUserId(user_id);

                var timesheet = _timesheetService.GetLastestTimesheet(employee.emp_number);
                ViewBag.timesheet = timesheet;
                if (timesheet != null)
                {
                    ViewBag.month = timesheet.start_date.Month;
                    ViewBag.year = timesheet.start_date.Year;
                }
                else
                {
                    ViewBag.month = DateTime.Now.Month;
                    ViewBag.year = DateTime.Now.Year;
                }
                
                return View();
            }
            
        }

        [HttpPost]
        [PermissionFilter(permission = "timesheet.mytimesheet")]
        public ActionResult MyTimesheet(FormCollection form_collection)
        {
           
            var year = Convert.ToInt32(form_collection["year"]);
            var month = Convert.ToInt32(form_collection["month"]);


            var start_date = new DateTime(year, month, 1);
            var end_date = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            ViewBag.month = month;
            ViewBag.year = year;
            var timesheet = _timesheetService.GetTimesheetByDates(start_date, end_date,employee.emp_number);
            ViewBag.timesheet = timesheet;

            return View();
        }

        [HttpPost]
        [PermissionFilter(permission = "timesheet.newtimesheet")]
        public ActionResult NewTimesheet(int month, int year)
        {
           
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            //check for existing timesheets
            var start_date = new DateTime(year, month, 1);
            var tsheet = _timesheetService.GetTimesheets().FirstOrDefault(e => e.emp_number == employee.emp_number && e.start_date==start_date);

            if (tsheet != null)
            {
                return RedirectToAction("Edit", new { id =tsheet.timesheet_id});
            }

           
            var timesheet = new admin_hrm_timesheet();
            timesheet.start_date = start_date;
            timesheet.end_date = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            timesheet.emp_number = employee.emp_number;
            timesheet.state = "NOT SUBMITTED";


          
            _timesheetService.AddTimesheet(timesheet);
            var id = timesheet.timesheet_id;
            return RedirectToAction("Edit", new { id });
        }

        [PermissionFilter(permission = "timesheet.submit")]
        public ActionResult Submit(int id)
        {

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);


            var timesheet = _timesheetService.GetTimesheet(id);
            timesheet.state = "SUBMITTED";
            _timesheetService.UpdateTimesheet(timesheet);

            //add log for the action 
            var log = new admin_hrm_timesheet_action_log();
            log.action = "SUBMITTED";
            log.date_time = DateTime.Now;
            log.performed_by = employee.emp_number;
            log.timesheet_id = timesheet.timesheet_id;
            _timesheetService.AddTimesheetLog(log);




            var email_body = string.Format("{0} {1} submitted timesheet for the month of {2}. ", employee.emp_lastname, employee.emp_firstname,timesheet.start_date.ToString("MMMM") );

            string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            Domain += "/timesheet/viewtimesheet/?id=" + id;

            email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

            var email = new Email
            {

                body = email_body,
                subject = "Timesheet submission - Workspace"
            };
            email.to = new List<string> { employee.admin_hrm_emp_reportto1.FirstOrDefault().admin_hrm_employee.emp_work_email };
            email.IsHtml = true;

            //NotificationUtil.SendEmail(email);

            return RedirectToAction("MyTimesheet", new { id });
        }

        public ActionResult FillTimesheet(int frm_emp)
        {
            return View();
            //var year = Convert.ToInt32(form_collection["year"]);
            //var month = Convert.ToInt32(form_collection["month"]);

            int year = 2018;
            var month = 0;
            //var start_date = new DateTime(year, month, 1);
            //var end_date = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            var fileContents = System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/convertcsv.json"));
            var array = JArray.Parse(fileContents);
            //var user_id = User.Identity.GetUserId();

            //var emps = new int[] { 7, 13, 43, 18, 1079, 1084, 1096, 3091, 4114, 4227, 4226, 4172, 4225, 4214, 4207 };
            foreach (var ar in array)
            {
                var to_emp = Convert.ToInt32(ar["emp"]);
                month = Convert.ToInt32(ar["month"]);
                var project_id = Convert.ToInt32(ar["project"]);
                try
                {
                    var employee = _employeeService.GetEmployee(to_emp);

                    if (employee == null)
                        continue;

                    var start_date = new DateTime(year, month, 1);
                    var timesheet = new admin_hrm_timesheet();
                    timesheet.start_date = start_date;
                    timesheet.end_date = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    timesheet.emp_number = employee.emp_number;
                    timesheet.state = "NOT SUBMITTED";
                    if (_timesheetService.GetEmpMonthTimesheet(month, year, to_emp) == null)
                        _timesheetService.AddTimesheet(timesheet);
                    else
                        timesheet = _timesheetService.GetEmpMonthTimesheet(month, year, to_emp);
                    var id = timesheet.timesheet_id;

                    if (timesheet.state == "APPROVED") continue;

                    //var from_timesheets = _timesheetService.GetTimesheets(month, year, frm_emp).ToList();
                    //var timesheet_items = new List<admin_hrm_timesheet_item>();
                    //foreach (var t_item in from_timesheets)
                    //{
                    //    t_item.emp_number = to_emp;
                    //    t_item.project_id = project_id;
                    //    timesheet_items.Add(t_item);
                    //}
                    //_timesheetService.AddTimesheetItem(timesheet_items);

                    var log = new admin_hrm_timesheet_action_log();
                    //{
                    //    action = "SAVED",
                    //    date_time = DateTime.Now,
                    //    performed_by = timesheet.emp_number,
                    //    timesheet_id = timesheet.timesheet_id
                    //};
                    //_timesheetService.AddTimesheetLog(log);


                    ////var timesheet = _timesheetService.GetTimesheet(id);
                    //timesheet.state = "SUBMITTED";
                    //_timesheetService.UpdateTimesheet(timesheet);

                    ////add log for the action 
                    //log = new admin_hrm_timesheet_action_log();
                    //log.action = "SUBMITTED";
                    //log.date_time = DateTime.Now;
                    //log.performed_by = employee.emp_number;
                    //log.timesheet_id = timesheet.timesheet_id;
                    //_timesheetService.AddTimesheetLog(log);

                    // var timesheet = _timesheetService.GetTimesheet(id);
                    timesheet.state = "APPROVED";
                    _timesheetService.UpdateTimesheet(timesheet);

                    //add log for the action 
                    log = new admin_hrm_timesheet_action_log();
                    log.action = "APPROVED";
                    log.date_time = DateTime.Now;
                    log.performed_by = employee.admin_hrm_emp_reportto1.FirstOrDefault().admin_hrm_employee.emp_number;
                    log.timesheet_id = timesheet.timesheet_id;
                    _timesheetService.AddTimesheetLog(log);
                }
                catch (Exception ex)
                {

                }

            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Report(int? month,int? year)
        {
            var month_value = DateTime.Now.Month - 1;
            var year_value = DateTime.Now.Year;
            if (month.HasValue) month_value = month.Value;
            if (year.HasValue) year_value = year.Value;

            var command = new SqlCommand();
            command.CommandText = "sp_get_timesheet_report";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@month", month_value);
            command.Parameters.AddWithValue("@year", year_value);

            var ds = DataUtil.GetDataSet(command);

            ViewBag.table = ds.Tables[0];
            ViewBag.month = month_value;
            ViewBag.year = year_value;

            return View();
        }

        [PermissionFilter(permission = "timesheet.approve")]
        public ActionResult Approve(int id)
        {

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            var timesheet = _timesheetService.GetTimesheet(id);
            timesheet.state = "APPROVED";
            _timesheetService.UpdateTimesheet(timesheet);

            //add log for the action 
            var log = new admin_hrm_timesheet_action_log();
            log.action = "APPROVED";
            log.date_time = DateTime.Now;
            log.performed_by = employee.emp_number;
            log.timesheet_id = timesheet.timesheet_id;
            _timesheetService.AddTimesheetLog(log);


            return RedirectToAction("ViewTimesheet", new {id= id });
        }

        public ActionResult Cancel(int id)
        {

            var timesheet = _timesheetService.GetTimesheet(id);
            timesheet.state = "NOT SUBMITTED";
            _timesheetService.UpdateTimesheet(timesheet);

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            //add log for the action 
            var log = new admin_hrm_timesheet_action_log();
            log.action = "CANCELLED";
            log.date_time = DateTime.Now;
            log.performed_by = employee.emp_number;
            log.timesheet_id = timesheet.timesheet_id;
            _timesheetService.AddTimesheetLog(log);


            return RedirectToAction("ViewTimesheet", new { id = id });
        }

        public ActionResult Audit(int? year,int? month,string search)
        {
            var yr = DateTime.Now.Year;
            var mnth = DateTime.Now.Month;
            if (year.HasValue && month.HasValue)
            {
                yr = year.Value;
                mnth = month.Value;
            }
            ViewBag.employees = _employeeService.GetEmployees();
            ViewBag.audits = _timesheetService.GetMonthlyAudit(yr, mnth);
            ViewBag.search = search;
            ViewBag.month = mnth;
            ViewBag.year = yr;

            return View();
        }

        public ActionResult Index(int? month, int? year, int? page, string search = null)
        {
            var month_value = DateTime.Now.Month - 1;
            var year_value = DateTime.Now.Year;
            if (month.HasValue)
                month_value = (int)month;
            if (year.HasValue)
                year_value = (int) year;

            ViewBag.month = month_value;
            ViewBag.year = year_value;
            var allTimesheets = _timesheetService.GetAllTimesheet()
                .Where(m => m.start_date.Year == year_value && m.end_date.Month == month_value).Select(m => m.admin_hrm_employee);

            var allTimesheet = _timesheetService.GetAllTimesheetItem()
                .Where(m => m.admin_hrm_timesheet.start_date.Year == year_value && m.admin_hrm_timesheet.end_date.Month == month_value);
            var timesheet = new List<TimeSheetO>();
           
            var AllEmp = _employeeService.GetEmployees();

            foreach (var obj in allTimesheets)
            {   
                var emp_no = obj.emp_number;
                var projects = allTimesheet.Where(m => m.emp_number == emp_no)
                    .GroupBy(x => x.project_id)
                    .Select(e => new TimesheetProjects { Duration = e.Sum(m => (int)m.duration), ProjectId = e.Key})
                    .ToList();
                timesheet.Add(new TimeSheetO {
                    EmployeeName = obj.emp_firstname + " " + obj.emp_lastname,
                    EmpNumber = emp_no,
                    Designation = obj.admin_hrm_emp_job_record.admin_hrm_lkup_job_title.job_title,
                    Projects = projects
                });
            }

            ViewBag.TimeSheetO = timesheet;
            ViewBag.TimsheetProjects = allTimesheet.Select(m => m.pm_project).Distinct()
                .Select(m => new TimesheetProjectNames { ProjectId = m.id, ProjectName = m.name}).ToList();

            return View();
        }
    }
}