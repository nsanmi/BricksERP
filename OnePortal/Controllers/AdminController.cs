using HRM.DAL.IService;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using HRM.DAL.Util;
using OnePortal.Models.ViewModels;
using Newtonsoft.Json;

namespace OnePortal.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        IEmployeeService _employeeService;
        IAnnouncementService _announcementService;
        ILeaveService _leaveService;

        public AdminController(IEmployeeService employeeService, IAnnouncementService announcementService, ILeaveService leaveService)
        {
            _employeeService = employeeService;
            _announcementService = announcementService;
            _leaveService = leaveService;
        }
        // GET: Admin
        public ActionResult Noticeboard(int? page)
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);
            //var emp= employee.admin_hrm_emp_reportto1.FirstOrDefault().admin_hrm_employee1.emp_work_email;

            int pageSize = 5;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var announcements = _announcementService.GetAnnouncements().OrderByDescending(e => e.created_at).ToPagedList(pageIndex, pageSize);
            ViewBag.leaves = _leaveService.GetApproved();
            //get employees who have birthday this month
            ViewBag.month_celebrants = _employeeService.GetEmployees().Where(e => e.emp_birthday.Value.Month == DateTime.Now.Month).OrderByDescending(e=>e.emp_birthday.Value);
            //JsonTextReader reader = null;
            //try
            //{
            //    reader = new JsonTextReader(new StreamReader("C:\\Users\\NELSON.O\\source\\repos\\oneportal\\OnePortal\\Models\\coreValues.json"));

            //}
            //catch(Exception e)
            //{

            //}
            List<CoreValues> items = null;
            
            using (StreamReader r = new StreamReader(HttpContext.Server.MapPath("~/App_Data/coreValues.json")))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<CoreValues>>(json);
            }
            ViewBag.coreValues = items;

            return View(announcements);
        }

        public ActionResult DeletePost(int id)
        {
            _announcementService.DeleteAnnouncement(id);
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Organogram()
        {
            return View();
        }

        public ActionResult Organisation(string href)
        {
            ViewBag.href = href;
            return View();
        }

        public ActionResult Policies()
        {
            DirectoryInfo d = new DirectoryInfo(@"C:\Policies");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles(); //Getting Text files
            var policies = new List<Policy>();
            foreach (FileInfo file in Files)
            {
                policies.Add(new Policy
                {
                    FilePath = file.Name,
                    DisplayName = file.Name,
                    CreateDate = "4/8/2019"
                });
            }

            ViewBag.policies = policies;
            return View();
        }

        public ActionResult Dashboard()
        {
           
            return View();
        }

        public ActionResult OurTeam()
        {

            return View();
        }

        public ActionResult Activity()
        {

            return View();
        }

        public ActionResult NoteView()
        {

            return View();
        }

        public ActionResult FileManager()
        {

            return View();
        }

        public ActionResult IssueTrack()
        {

            return View();
        }

        public ActionResult Calander()
        {

            return View();
        }
    }
}