using System;
using System.Linq;
using System.Threading.Tasks;
using HRM.DAL.IService;
using System.Web.Mvc;
using HRM.DAL.Models;

using PagedList;using OnePortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnePortal.Helper;
using HRM.DAL.Util;
using System.Collections.Generic;
using System.Web;
using Microsoft.Owin.Security;

namespace OnePortal.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [PermissionFilter(permission = "employee.create")]
        public async Task<ActionResult> Create(admin_hrm_employee employee, admin_hrm_emp_job_record job_Record, FormCollection collection)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            /*
             * Added By Johnbosco
             * To Check if an employee exists before creating
             * This check is done by email comparism
             */
            var existing_employee = _employeeService.GetEmployee(employee.emp_work_email);
            var deacivated_employee = _employeeService.GetDeactivatedEmployee(employee.emp_work_email);
            if (existing_employee != null && deacivated_employee != null)
            {
                ViewBag.message = "The employee has already been added or was deactivated, Please check and make sure that the email address is correct and has not been added before";
                return View();
            }

            //var employee = _employeeService.GetEmployee(Convert.ToInt32(emp_number));
            employee.job_title_code = job_Record.job_title_id;
            employee.emp_status = job_Record.employment_status_id;
            var emp_number = _employeeService.AddEmployee(employee);
            if (emp_number > 0)
            {
                employee.emp_number = emp_number;
                job_Record.employee_number = employee.emp_number;

                //add the job_record of the employee
                _employeeService.AddJobRecord(job_Record);

                //add supervisor
                var supervisor = Convert.ToInt32(collection["supervisor"]);
                var reportto = new admin_hrm_emp_reportto
                {
                    erep_sup_emp_number = supervisor,
                    erep_sub_emp_number = emp_number,
                    erep_reporting_mode = 1
                };
                _employeeService.AddReportTo(reportto);

                //add the location of the employee
                _employeeService.AddLocation(new admin_hrm_emp_locations { location_id = job_Record.location_id.Value, emp_number = emp_number });
                //create an account for the user

                var user = new ApplicationUser { UserName = employee.emp_work_email, Email = employee.emp_work_email, RegistrationDate = DateTime.Now };
                var pass = Utils.GeneratePassword(3, 3, 3) + "!";
                var result = await UserManager.CreateAsync(user, pass);
                if (result.Succeeded)
                {
                    employee.user_id = user.Id;
                    _employeeService.UpdateEmployee(employee);
                    UserManager.AddToRole(user.Id, collection["role"].ToString());

                    var template = EmailUtil.GetTemplate("account_setup");
                    string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    template = template.Replace("{name}", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname));
                    template = template.Replace("{workspace_name}", "WorkSpace");
                    template = template.Replace("{pass}", pass);
                    template = template.Replace("{url}", Domain);

                    var email = new Email
                    {
                        body = template,
                        subject = "Project - Brinkspoint ERP"
                    };

                    email.to = new List<string> { employee.emp_work_email };
                    email.IsHtml = true;

                    NotificationUtil.SendEmail(email);
                    //string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

                    NotificationUtil.SendNotifications(new int[] { employee.emp_number }, template, Domain);
                    ViewBag.message = "The employee was created successfully, email was sent, consider sending the employee a confirmatory email with employee password. " +
                                      "Employee password is " + pass + " Also urge employee to change their password upon login";
                }
                //return View("Edit",emp_number);
            }
            return View();
        }

        [PermissionFilter(permission = "employee.index")]
        public ActionResult Index()
        {
            ViewBag.employees = _employeeService.GetEmployees();
            return View();
        }

        [PermissionFilter(permission = "employee.emp")]
        public ActionResult Emp(int id)
        {
            ViewBag.employee = _employeeService.GetEmployee(id);
            return View();
        }

        [PermissionFilter(permission = "employee.emprecord")]
        public ActionResult EmpRecord()
        {

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            ViewBag.employee = _employeeService.GetEmployee(employee.emp_number);
            return View("MyRecord");
        }

        [HttpPost]
        [PermissionFilter(permission = "employee.emprecord")]
        public ActionResult UpdateEmpRecord(admin_hrm_employee employee)
        {
            _employeeService.UpdateUsername(employee);
            var collection = Request.Form;
            var joined_date = collection["joined_date"];
            _employeeService.UpdateEmployee(employee);

            var job_record = new admin_hrm_emp_job_record();
            job_record.employee_number = employee.emp_number;
            job_record.job_title_id = employee.job_title_code;
            job_record.joined_date = employee.joined_date;

            if (collection["probation_end_date"] != null && collection["probation_end_date"] != "")
                job_record.probation_end_date = Convert.ToDateTime(collection["probation_end_date"]);

            if (collection["date_of_permanency"] != null && collection["date_of_permanency"] != "")
                job_record.date_of_permanency = Convert.ToDateTime(collection["date_of_permanency"]);

            job_record.subunit_id = Convert.ToInt32(collection["emp_subunit"]);
            job_record.employment_status_id = Convert.ToInt32(collection["emp_status"]);
            _employeeService.UpdateEmployeeRecord(job_record);
            //return RedirectToAction("MyRecord");
            return Redirect(Request.UrlReferrer.ToString());
        }

        [PermissionFilter(permission = "employee.manageusers")]
        [Authorize(Roles = "procurement")]
        public ActionResult ManageUsers(int? page,string search=null)
        {
            int pageSize = 50;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<admin_hrm_employee> employees = _employeeService.GetUsers().OrderBy(e => e.emp_work_email).ToPagedList(pageIndex, pageSize);
            if (search != null)
            {
                employees = _employeeService.GetUsers().Where(e=>e.emp_lastname.Contains(search) || e.emp_firstname.Contains(search) || e.emp_middle_name.Contains(search) || e.AspNetUser.UserName.Contains(search) || e.emp_work_email.Contains(search)).OrderBy(e => e.emp_work_email).ToPagedList(pageIndex, pageSize);
            }
            return View(employees);
        }

        [HttpPost]
        [PermissionFilter(permission = "employee.manageusers")]
        public async Task<ActionResult> ManageUsers(FormCollection collection)
        {

            var emp_number = collection["employee"];
            var username = collection["username"];
            var role = collection["role"];
            var password = collection["password"];


            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var employee = _employeeService.GetEmployee(Convert.ToInt32(emp_number));

            var user = new ApplicationUser { UserName = username, Email = employee.emp_work_email,RegistrationDate=DateTime.Now };
            
            var result=await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                employee.user_id = user.Id;
                _employeeService.UpdateEmployee(employee);
                UserManager.AddToRole(user.Id, role);


                var template = EmailUtil.GetTemplate("account_setup_manual");

                template = template.Replace("{name}", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname));
                template = template.Replace("{workspace_name}", "WorkSpace");
                template = template.Replace("{password}", password);

                var email = new Email
                {
                    body = template,
                    subject = "Project - Workspace"
                };


                email.to = new List<string> { employee.emp_work_email };
                email.IsHtml = true;

                NotificationUtil.SendEmail(email);
                string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

                NotificationUtil.SendNotifications(new int[] { employee.emp_number }, template, Domain);

                //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public ActionResult LinkUserToAccount(FormCollection collection)
        {

            var emp_number = collection["employee"];
            var username = collection["username"];
           

            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var employee = _employeeService.GetEmployee(Convert.ToInt32(emp_number));

            var user = UserManager.FindByName(username);
            employee.user_id = user.Id;
            _employeeService.UpdateEmployee(employee);

         
            return RedirectToAction("ManageUsers");
        }

        public async Task<ActionResult> Impersonate(string username)
        {
            await UserHelper.ImpersonateUserAsync(username);

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> RevertImpersonate()
        {
            await UserHelper.RevertImpersonationAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [PermissionFilter(permission = "employee.addrole")]
        public ActionResult AddRole(FormCollection collection)
        {
            var user_id = collection["user_id"];
            var role = collection["new_role"];
           

            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            UserManager.AddToRole(user_id, role);

         
            return RedirectToAction("ManageUsers");
        }

        [PermissionFilter(permission = "employee.edit")]
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [PermissionFilter(permission = "employee.edit")]
        public ActionResult Edit(admin_hrm_employee employee)
        {
            return View();
        }

        [HttpPost]
        [PermissionFilter(permission = "employee.addemergencycontact")]
        public ActionResult AddEmergencyContact(admin_hrm_emp_emergency_contacts contacts)
        {
            _employeeService.AddEmergencyContact(contacts);
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        [PermissionFilter(permission = "employee.adddependent")]
        public ActionResult AddDependent(admin_hrm_emp_dependents dependents)
        {
            _employeeService.AddDependent(dependents);
            return Redirect(Request.UrlReferrer.ToString());
        }
        [HttpPost]
        [PermissionFilter(permission = "employee.addeducation")]
        public ActionResult AddEducation(admin_hrm_emp_education education)
        {
            _employeeService.AddEducation(education);
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        [PermissionFilter(permission = "employee.reportto")]
        public ActionResult ReportTo(FormCollection collection)
        {
            var emp_number = Convert.ToInt32(collection["emp_number"]);
            var employee = Convert.ToInt32(collection["employee"]);
            var reportto = new admin_hrm_emp_reportto();
            if (collection["reporting_type"].ToString()== "Supervisee")
            {
                reportto.erep_sup_emp_number = emp_number;
                reportto.erep_sub_emp_number = employee;
                reportto.erep_reporting_mode = 1;
            }
            else
            {
                reportto.erep_sup_emp_number = employee;
                reportto.erep_sub_emp_number = emp_number;
                reportto.erep_reporting_mode = 1;
            }
            _employeeService.AddReportTo(reportto);
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult AddMembership(admin_hrm_emp_member_detail member_Detail)
        {
            _employeeService.AddMembership(member_Detail);
            return Redirect(Request.UrlReferrer.ToString());
        }
      
        public ActionResult DeleteDependent(int id)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);


            _employeeService.DeleteDependent(employee.emp_number, id);
            return Redirect(Request.UrlReferrer.ToString());
        }
      
        public ActionResult DeleteContact(int id)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            _employeeService.DeleteEmergencyContact(employee.emp_number, id);

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult DeleteEducation(int id)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);


            _employeeService.DeleteEducation(id);

            return Redirect(Request.UrlReferrer.ToString());
        }


        public ActionResult EmpProfile()
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            ViewBag.employee = _employeeService.GetEmployee(employee.emp_number);
            
            return View();
        }

        public async Task<ActionResult> Bash(FormCollection collection)
        {
            //var emp_number = collection["employee"];
            //var username = collection["username"];
            //var role = collection["role"];
            //var password = collection["password"];

            //ApplicationDbContext context = new ApplicationDbContext();
            //var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //var employee = _employeeService.GetEmployee(Convert.ToInt32(emp_number));

            //var user = new ApplicationUser { UserName = username, Email = employee.emp_work_email, RegistrationDate = DateTime.Now };

            //var result = await UserManager.CreateAsync(user, password);
            //if (result.Succeeded)
            //{
            //    employee.user_id = user.Id;
            //    _employeeService.UpdateEmployee(employee);
            //    UserManager.AddToRole(user.Id, role);

            //    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            //}
            return RedirectToAction("ManageUsers");
        }
    }
}