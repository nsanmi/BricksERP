using HRM.DAL.IService;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using HRM.DAL.Models;
using System.Collections.Generic;
using System.IO;
using PagedList;
using System.Linq;
using OnePortal.Helper;
using HRM.DAL.Util;
using OnePortal.Models.ViewModels;

namespace OnePortal.Controllers
{
    [Authorize]
    public class LeaveController : Controller
    {

        readonly static oneportalEntities _context = new oneportalEntities();

        ILeaveService _leaveService;
        IEmployeeService _employeeService;
        public LeaveController(ILeaveService leaveService, IEmployeeService employeeService)
        {
            _leaveService = leaveService;
            _employeeService = employeeService;
        }
        // GET: Leave

        [PermissionFilter(permission = "leave.apply")]
        public ActionResult Apply()
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            ViewBag.entitlements = _leaveService.GetEmployeeEntitlements(employee.emp_number);
            ViewBag.message = TempData["message"];
            return View();
        }

        [HttpPost]
        [PermissionFilter(permission = "leave.apply")]
        public ActionResult Apply(FormCollection collection)
        {
            //check dates
            if (string.IsNullOrEmpty(collection["start_date"]) || string.IsNullOrEmpty(collection["end_date"]) ||
                string.IsNullOrEmpty(collection["leave_type"]))
            {
                TempData["message"] = "Please select appropriate dates.";
                return RedirectToAction("Apply");
            }
            var start_date = Convert.ToDateTime(collection["start_date"]);
            var end_date = Convert.ToDateTime(collection["end_date"]);
            var leave_type = Convert.ToInt32(collection["leave_type"]);
           
            //check dates
            if (start_date > end_date)
            {
                //ViewBag.message = "";
                TempData["message"] = "Please check and select appropriate dates.";
                return RedirectToAction("Apply");
            }

            var days = 0;
            //calculate the number of days
            for (var day = start_date; day <= end_date; day = day.AddDays(1))
            {
                if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday && !OptionUtil.isHoliday(day))
                {
                    days += 1;
                }
            }
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            var entitlement = employee.admin_hrm_leave_entitlement.LastOrDefault(e => e.leave_type_id == leave_type && (e.from_date <= DateTime.Now && e.to_date >= DateTime.Now));
            //check entitlements
            if (entitlement == null ){
                //ViewBag.message = "";
                TempData["message"] = "You are currently not entitled to this type of leave.";
                return RedirectToAction("Apply");
            }
           
            if ((entitlement.no_of_days - entitlement.days_used) < days)
            {
                ViewBag.message = "";
                TempData["message"] = "The number of days you selected is more than your entitlements please check and reapply.";
                return RedirectToAction("Apply");
            }

            var partial_days = Convert.ToInt32(collection["sel_partial_days"]);
            var comment = collection["comment"];
            if (partial_days == 1)
            {
                var request = new admin_hrm_leave_request()
                {
                    leave_type_id = leave_type,
                    leave_start_date = start_date,
                    date_applied = DateTime.Now,
                    emp_number = employee.emp_number
                };
                _leaveService.AddLeaveRequest(request);

                if (comment != null || comment != "")
                {
                    var request_comment = new admin_hrm_leave_request_comment()
                    {
                        leave_request_id = request.id,
                        created = DateTime.Now,
                        created_by_name = employee.emp_firstname,
                        created_by_emp_number = employee.emp_number,
                        comments = comment
                    };

                   _leaveService.AddRequestComment(request_comment);
                }

                var email_body = "<table border='1'><tr><td>Date(s)</td><td>Duration(Hour)</td></tr>";
                var leaves = new List<admin_hrm_leave>();
                for (var day = start_date; day <= end_date; day = day.AddDays(1))
                {
                    if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday && !OptionUtil.isHoliday(day))
                    {
                        leaves.Add(new admin_hrm_leave()
                        {
                            leave_request_id = request.id,
                            date = day,
                            emp_number = employee.emp_number,
                            length_days = 1,
                            length_hours = 8,
                            status = 1,
                            leave_type_id = leave_type
                        });
                        email_body += "<tr><td>" + day.ToString("yyyy-MM-dd") + "</td><td>8</td></tr>";
                    }
                }
                email_body += "</table><br/>";
                _leaveService.AddLeave(leaves);

                var leave_type_name = OptionUtil.GetLeaveTypes().FirstOrDefault(e => e.id == leave_type).name;
                email_body += string.Format("{0} {1} {2} applied for {3} day(s) {4} leave", employee.emp_lastname, employee.emp_firstname, employee.emp_middle_name, leaves.Count(), leave_type_name);
                var email = new Email
                {
                    body = email_body,
                    subject = "Leave Application - Workspace"
                };
                email.to = new List<string> { employee.admin_hrm_emp_reportto1.FirstOrDefault().admin_hrm_employee.emp_work_email };
                //email.to = new List<string> { employee.emp_work_email };
                email.IsHtml = true;
             
                NotificationUtil.SendEmail(email);
            }
            
            //ViewBag.message = "Your leave application was processed successfully.";
            TempData["message"] = "Your leave application was processed successfully. Wait for your Supervisor approval";
            return RedirectToAction("Apply");
        }

        [PermissionFilter(permission = "leave.myleave")]
        public ActionResult MyLeave(int? page, string search)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var leaves = _leaveService.GetEmployeeLeaveRequest(employee.emp_number).OrderByDescending(e => e.leave_start_date).ToPagedList(pageIndex, pageSize);
            ViewBag.messages = TempData["messages"];

            return View(leaves);

        }

        [PermissionFilter(permission = "leave.myentitlements")]
        public ActionResult MyEntitlements()
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            ViewBag.entitlements = _leaveService.GetEmployeeEntitlements(employee.emp_number);

            return View();
        }

        [PermissionFilter(permission = "leave.leavelist")]
        public ActionResult LeaveList(int? page,int[] category,string search, DateTime? start_date, DateTime? end_date)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var leaves = _leaveService.GetPendingApproval(employee.emp_number).OrderByDescending(e => e.leave_start_date).ToPagedList(pageIndex, pageSize);
            if (category != null)
            {
                if (category.Any())
                {
                    leaves = _leaveService.GetSubLeaves(employee.emp_number, category).OrderByDescending(e => e.leave_start_date).ToPagedList(pageIndex, pageSize);
                }
            }
            

            return View(leaves);
        }

        [HttpPost]
        [PermissionFilter(permission = "leave.leavelist")]
        public ActionResult LeaveList(FormCollection collection)
        {
            foreach (var key in collection.AllKeys)
            {
                var value = Request[key];
                if (value == string.Empty || value == "-2") continue;

                var request = _leaveService.GetRequest(Convert.ToInt32(key));
                var leaves = _leaveService.GetRequestLeaves(request.id);

                var email_body = "<table border='1'><tr><td>Date(s)</td><td>Duration(Hour)</td><td>Status</td></tr>";

                foreach (var leave in leaves)
                {
                    leave.status = Convert.ToInt32(value);
                    //_leaveService.UpdateLeave(leave);
                    email_body += "<tr><td>" + leave.date.Value.ToString("yyyy-MM-dd") + "</td><td>8</td><td>TAKEN</td></tr>";
                }
                _leaveService.UpdateLeave(leaves.ToList());
                email_body += "</table><br/>";
                var employee = request.admin_hrm_employee;

                email_body += string.Format("{0} {1} {2} the {3} day(s) {4} leave you applied for was {5}", employee.emp_lastname, employee.emp_firstname, employee.emp_middle_name, request.admin_hrm_leave.LastOrDefault().date.Value.Subtract(request.leave_start_date).Days, request.admin_hrm_leave.FirstOrDefault().admin_hrm_leave_type.name, OptionUtil.GetLeaveStatus(Convert.ToInt32(value)));
                var email = new Email
                {
                    
                    body = email_body,
                    subject = "Leave Application - Workspace"
                };

                email.to=new List<string> { employee.emp_work_email };
                

                NotificationUtil.SendEmail(email);
            }
            string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            var url = Request.Url.Authority;
            return RedirectToAction("LeaveList");
        }

        [PermissionFilter(permission = "leave.assignleave")]
        public ActionResult AssignLeave()
        {
            return View();
        }

        [HttpPost]
        [PermissionFilter(permission = "leave.assignleave")]
        public ActionResult AssignLeave(FormCollection collection)
        {
            var start_date = Convert.ToDateTime(collection["start_date"]);
            var end_date = Convert.ToDateTime(collection["end_date"]);
            var leave_type = Convert.ToInt32(collection["leave_type"]);

            var employee = _employeeService.GetEmployee(Convert.ToInt32(collection["employee"].ToString()));

            var partial_days = Convert.ToInt32(collection["sel_partial_days"]);
            var comment = collection["comment"];
            if (partial_days == 1)
            {

                var request = new admin_hrm_leave_request()
                {
                    leave_type_id = leave_type,
                    leave_start_date = start_date,
                    date_applied = DateTime.Now,
                    emp_number = employee.emp_number
                };
                _leaveService.AddLeaveRequest(request);

                if (comment != null || comment != "")
                {
                    var request_comment = new admin_hrm_leave_request_comment()
                    {
                        leave_request_id = request.id,
                        created = DateTime.Now,
                        created_by_name = employee.emp_firstname,
                        created_by_emp_number = employee.emp_number,
                        comments = comment

                    };

                    _leaveService.AddRequestComment(request_comment);
                }

                var leaves = new List<admin_hrm_leave>();
                for (var day = start_date; day <= end_date; day = day.AddDays(1))
                {
                    leaves.Add(new admin_hrm_leave()
                    {
                        leave_request_id = request.id,
                        date = day,
                        emp_number = employee.emp_number,
                        length_days = 1,
                        length_hours = 8,
                        status = 1,
                        leave_type_id = leave_type
                    });
                }
                _leaveService.AddLeave(leaves);
            }

            return RedirectToAction("AssignLeave");
            
        }
        [HttpPost]
        public ActionResult ChangeStatus()
        {
            return RedirectToAction("LeaveList");
        }

        [PermissionFilter(permission = "leave.leavedetails")]
        public ActionResult LeaveDetails(int request_id)
        {
            var canEdit = 0;
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            var request = _leaveService.GetRequest(request_id);
            var sup=request.admin_hrm_employee.admin_hrm_emp_reportto1.FirstOrDefault()?.admin_hrm_employee.emp_number;
            if (employee.emp_number == sup)
            {
                canEdit = 1;
            }
            ViewBag.canEdit = canEdit;
            ViewBag.request = request;
            return View();
        }

        [HttpPost]
        [PermissionFilter(permission = "leave.leavedetails")]
        public ActionResult LeaveDetails(FormCollection collection)
        {
            var leaves = new List<admin_hrm_leave>();
            var email_body = "<table border='1'><tr><td>Date(s)</td><td>Duration(Hour)</td><td>Status</td></tr>";
            admin_hrm_employee employee = null;
            foreach (var key in collection.AllKeys)
            {   
                var value = Request[key];
                if (value == string.Empty || value == "-2") continue;

                var leave = _leaveService.GetLeave(Convert.ToInt32(key));
                
                leave.status = Convert.ToInt32(value);
                leaves.Add(leave);
               
                //if the leave is approved, subtract from the entitlements
                if (Convert.ToInt32(value) == 3)
                {
                    var entitlement = _leaveService.GetEntitlement(leave.leave_type_id, leave.emp_number);
                    entitlement.days_used = entitlement.days_used + 1;
                    _leaveService.UpdateEntitlement(entitlement);

                    email_body += "<tr><td>" + leave.date.Value.ToString("yyyy-MM-dd") + "</td><td>8</td><td>TAKEN</td></tr>";
                }
                else
                {
                    email_body += "<tr><td>" + leave.date.Value.ToString("yyyy-MM-dd") + "</td><td>8</td><td>"+OptionUtil.GetLeaveStatusName(leave.status.Value)+"</td></tr>";
                }
                employee = leave.admin_hrm_employee; 

            }
            email_body += "</table>";


            _leaveService.UpdateLeave(leaves);

            if (employee != null)
            {
                var email = new Email
                {
                    body = email_body,
                    subject = "Leave Application - Workspace"
                };

                email.to = new List<string> { employee.emp_work_email };
                email.IsHtml = true;

                NotificationUtil.SendEmail(email);
            }

          

            string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

            return RedirectToAction("LeaveList");
        }

        [PermissionFilter(permission = "leave.holiday")]
        public ActionResult Holiday(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var holidays = _leaveService.GetHolidays().OrderByDescending(e=>e.id).ToPagedList(pageIndex, pageSize); 
            return View(holidays);
        }

        [HttpPost]
        [PermissionFilter(permission = "leave.holiday")]
        public ActionResult Holiday(FormCollection collection)
        {
            var start_date = Convert.ToDateTime(collection["start_date"]);
            var end_date = Convert.ToDateTime(collection["end_date"]);
            var holiday = new admin_hrm_holiday
            {
                description = collection["description"].ToString(),
                length = end_date.Subtract(start_date).Days,
                date = start_date,
                recurring = Convert.ToInt32(collection["repeats"])
            };

            _leaveService.AddHoliday(holiday);

           
            return RedirectToAction("Holiday");
        }

        public ActionResult Entitlements()
        {
            //foreach(var employee in _employeeService.GetEmployees())
            //{
            //    var days = 25;
            //    var old = _leaveService.GetEntitlement(1, employee.emp_number);
            //    if (old != null)
            //    {
            //        if ((old.no_of_days - old.days_used) >= 10)
            //        {
            //            days += 10;
            //        }
            //        else
            //        {
            //            days += (int)(old.no_of_days - old.days_used);
            //        }
            //    }
            //    var entitlement = new admin_hrm_leave_entitlement { emp_number = employee.emp_number, no_of_days = days, days_used = 0, from_date = new DateTime(2017, 10, 1), to_date = new DateTime(2018, 9, 30), leave_type_id = 1, entitlement_type = 1, created_by_name = "System Admin" };

            //    _leaveService.AddEntitlement(entitlement);
            //    entitlement.leave_type_id = 2;
            //    entitlement.no_of_days = 12;
            //    _leaveService.AddEntitlement(entitlement);
            //}
            return View();
        }

        public ActionResult OnLeave()
        {
            return View();
        }

        //public ActionResult LeaveUser()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public ActionResult HRIndex(int status)
        //{
        //    ViewBag.leave = _leaveService.GetLeaveUser(status);

        //    return View();
        //}

        public ActionResult HRIndex(int? page)
        {
            int pageIndex = 1;
            int pageSize = 50;
            pageIndex = page.HasValue ? page.Value : 1;
            var LeaveStatus = _leaveService.GetApproved();
            var allLeave = _leaveService.GetAllLeave().OrderByDescending(m => m.leave_start_date).ToList();
            var leaves = new List<LeaveR>();
            
            foreach (var _ in LeaveStatus)
            { 
                if (_.admin_hrm_leave.Count > 0)
                {
                    var startDate = _.admin_hrm_leave.First().date;
                    var endDate = _.admin_hrm_leave.Last().date;
                    var OnLeave = DateTime.Now >= startDate && DateTime.Now <= endDate;
                    var app = LeaveStatus;
                    if (OnLeave == true)
                    {
                        leaves.Add(new LeaveR
                        {
                            EmpNumber = _.emp_number,
                            EmployeeName = _.admin_hrm_employee.emp_firstname + " " + _.admin_hrm_employee.emp_lastname,
                            StartDate = _.admin_hrm_leave.First().date?.ToString("d"),
                            EndDate = _.admin_hrm_leave.Last().date?.ToString("d"),
                            NumberOfDaysTaken = _.admin_hrm_leave.Count(m => m.status == 3),
                            NumberOfDaysCanceled = _.admin_hrm_leave.Count(m => m.status == 0),
                            NumberOfDaysRejected = _.admin_hrm_leave.Count(m => m.status == -1),
                            NumberOfDaysPendingApproval = _.admin_hrm_leave.Count(m => m.status == 1),
                            NumberOfDaysScheduled = _.admin_hrm_leave.Count(m => m.status == 2),
                            OnLeave = DateTime.Now >= startDate && DateTime.Now <= endDate
                            
                        });
                    }
                }
            }
            
          return View(leaves.OrderBy(m => m.StartDate).ToPagedList(pageIndex, pageSize));

        }

        public ActionResult LeaveUser(int? month, int? year, int? page)
        {
            var month_value = DateTime.Now.Month;
            var year_value = DateTime.Now.Year;
            if (month.HasValue)
                month_value = (int)month;
            if (year.HasValue)
                year_value = (int)year;
            ViewBag.month = month_value;
            ViewBag.year = year_value;

            int pageIndex = 1;
            int pageSize = 50;
            pageIndex = page.HasValue ? page.Value : 1;
            var allLeave = _leaveService.GetAllLeave().OrderByDescending(m => m.leave_start_date).ToList();

            var leaves = new List<LeaveR>();

            foreach (var _ in allLeave)
            {
                
                    if (_.admin_hrm_leave.Count > 0)
                    {  
                        var startDate = _.admin_hrm_leave.First().date;
                        var endDate = _.admin_hrm_leave.Last().date;
                        var OnLeave = DateTime.Now >= startDate && DateTime.Now <= endDate;

                      
                            leaves.Add(new LeaveR
                            {
                                EmpNumber = _.emp_number,
                                EmployeeName = _.admin_hrm_employee.emp_firstname + " " + _.admin_hrm_employee.emp_lastname,
                                StartDate = _.admin_hrm_leave.First().date?.ToString("d"),
                                EndDate = _.admin_hrm_leave.Last().date?.ToString("d"),
                                NumberOfDaysTaken = _.admin_hrm_leave.Count(m => m.status == 3),
                                NumberOfDaysCanceled = _.admin_hrm_leave.Count(m => m.status == 0),
                                NumberOfDaysRejected = _.admin_hrm_leave.Count(m => m.status == -1),
                                NumberOfDaysPendingApproval = _.admin_hrm_leave.Count(m => m.status == 1),
                                NumberOfDaysScheduled = _.admin_hrm_leave.Count(m => m.status == 2),
                                OnLeave = DateTime.Now >= startDate && DateTime.Now <= endDate
                            });
                        
                    }
            }
            ViewBag.leave = leaves;
            return View();
        }
            public ActionResult Cancel(int id)
        {

            //var status = leaves.Select(e => e.status).Distinct();
            //var list = new List<string>();
            //foreach (var st in status)
            //{
            //    var stat = _context.admin_hrm_leave_status.FirstOrDefault(e => e.status == st);
            //    if (stat != null)
            //    {
            //        list.Add(stat.name + "(" + leaves.Count(e => e.status == st) + ")");
            //    }

            //}

            int cancel = _leaveService.CancelLeave(id);

            if (cancel == 0) {
                TempData["messages"] = "Your leave was canceled successfully";
            }
            else { 
                TempData["messages"] = "There was an error canceling leave request";

            }

            return RedirectToAction("MyLeave");

           
        }
    }
}