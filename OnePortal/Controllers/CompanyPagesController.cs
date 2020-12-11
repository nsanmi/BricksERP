using Hotel.Dal.IService;
using Hotel.Dal.Models;
using HRM.DAL.IService;
using HRM.DAL.Models;
using HRM.DAL.Util;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OnePortal.Helper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnePortal.Controllers
{
    public class CompanyPagesController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();

        IEmployeeService _employeeService;
        IHGuestService _hGuestService;

        public CompanyPagesController(IEmployeeService employeeService, IHGuestService hguestService)
        {
            _employeeService = employeeService;
            _hGuestService = hguestService;
        }


        // GET: CompanyPages
        public ActionResult Index()
        {
            List<Room> items = null;
            using (StreamReader r = new StreamReader(HttpContext.Server.MapPath("~/Data/file.json")))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Room>>(json);
            }
            ViewBag.Room = items;


            return View();
        }

        public ActionResult Dashboard()
        {

            return View();
        }


        public ActionResult EmpProfile()
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            ViewBag.employee = _employeeService.GetEmployee(employee.emp_number);

            return View();
        }
        public ActionResult OurTeam()
        {

            return View();
        }

        public async Task<ActionResult> Guest()
        {
            return View(await db.Hotel_guest.ToListAsync());
        }
        public ActionResult Guests()
        {

            return View();
        }

        public ActionResult GuestProfile(int guestId, Hotel_guest guest)
        {
            var guestprofile = _hGuestService.GetGuest(guestId);
            var b = guestprofile;

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            ViewBag.User = employee.job_title_code;

            ViewBag.employee = _employeeService.GetEmployee(employee.emp_number);
            ViewBag.profile = _hGuestService.GetGuest(guestId);


            return View();
        }

        public ActionResult Success()
        {

            return View();
        }



        public ActionResult Workers()
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

        public ActionResult RequestDiscount(int guestId, int emp_number, string guestname)
        {

            var request = "Request for discount";
            Guest guest = new Guest();
            var user_id = User.Identity.GetUserId();

            try
            {
                //send mail to compalin admin

                //Get the url of the guest
                string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                            (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                Domain += "/Hotel_guest/MDDiscount?guestId=" + guest.GuestID;
                var email_body = string.Format("A Request for booking<strong>{0}</strong>  discount of 30% has been raised on behalf of<strong>{1}</strong>.<br/>", guestname
                   );
                email_body += "the description is <br/>";
                email_body += string.Format("<i>{0}</i>.<br/>", request);
                email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

                var notification_email = new Email
                {
                    body = email_body,
                    subject = "Discount - Action required - BricksERP"
                };
                notification_email.IsHtml = true;
                notification_email.body = email_body;
#if DEBUG
                //notification_email.to = new List<string>{ UserHelper.GetEmployeeEmail(user_id) };

                notification_email.to = _hGuestService.GetBookingAdmin();
#else
                //notification_email.to = _complainService.GetComplainAdmin();
#endif

                NotificationUtil.SendEmailComplain(notification_email, UserHelper.GetEmployeeEmail(user_id));

                TempData["message"] =
                    "Your request has been saved and email sent to the appropriate persons. Thank You";
            }
            catch (Exception e)
            {
                TempData["message"] = "There was an error sending the discount request, Please try again";
                Utils.LogError(e);
            }

            //return View();
            //return RedirectToAction("GuestProfile");
            return RedirectToAction("GuestProfile", new { guestId = guestId });
        }
        public ActionResult UpdateDiscount()
        {
            return View();
        }

        public ActionResult GiveDiscount(int guestId, int percentage)
        {
            //ViewBag.complain = _hGuestService.GetGuest(guestId);


            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);

            ViewBag.employee = _employeeService.GetEmployee(employee.emp_number);

            //var guest_record = new Hotel_guest();

            var guest_old = db.Hotel_guest.First(e => e.Id == guestId);

            guest_old.DiscountapprovedBy = employee.emp_firstname + employee.emp_lastname + employee.emp_middle_name;

            guest_old.DiscountPercentage = percentage;
            guest_old.LastDiscountPercentage = percentage;

            guest_old.EmailOfDiscountapprovedPerson = employee.emp_work_email;
            guest_old.emp_number = employee.emp_number;
            guest_old.AddedDate = DateTime.Now;
            guest_old.UpdatedDate = DateTime.Now;


            if (guest_old.DateOfLastDiscountUpdate == null)
            {
                guest_old.DateOfLastDiscountUpdate = DateTime.Now;
            }
            else
            {
                guest_old.DateOfLastDiscountUpdate = guest_old.DateOfLastDiscountUpdate;
            }

            //db.Entry(guest_old).State = EntityState.Modified;

            //db.SaveChanges();


            //return RedirectToAction("CompanyPages", "GuestProfile", new { area = "Admin" });

            return RedirectToAction("GuestProfile", new { guestId = guestId });
        }

    }
}