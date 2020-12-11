using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Hotel.Dal.Models;
using HRM.DAL.Models;
using Microsoft.AspNet.Identity;
using HRM.DAL.Util;
using OnePortal.Helper;
using Hotel.Dal.IService;
using HRM.DAL.IService;

namespace OnePortal.Controllers
{
    public class Hotel_guestController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();

        IHGuestService _hGuestService;
        IEmployeeService _employeeService;

        public Hotel_guestController(IHGuestService hguestService, IEmployeeService employeeService)
        {

            _hGuestService = hguestService;
            _employeeService = employeeService;

        }
        // GET: Hotel_guest
        public async Task<ActionResult> Index()
        {
            return View(await db.Hotel_guest.ToListAsync());
        }

        // GET: Hotel_guest/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_guest hotel_guest = await db.Hotel_guest.FindAsync(id);
            if (hotel_guest == null)
            {
                return HttpNotFound();
            }
            return View(hotel_guest);
        }

        // GET: Hotel_guest/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hotel_guest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,First_name,Last_name,Other_names,Address,State,Country,Nextofkin_fullname,Email,Password,Nextofkin_fulladdress,Nextofkin_phonenumber,status,deleted")] Hotel_guest hotel_guest)
        {
            if (ModelState.IsValid)
            {
                db.Hotel_guest.Add(hotel_guest);
                await db.SaveChangesAsync();
                ViewBag.message = "The guest was successfully updated";
                return RedirectToAction("Index");

            }
            else
            {

                ViewBag.message = "There was an error updating the guest detail";
            }

            return View(hotel_guest);
        }

        // GET: Hotel_guest/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_guest hotel_guest = await db.Hotel_guest.FindAsync(id);
            if (hotel_guest == null)
            {
                return HttpNotFound();
            }
            return View(hotel_guest);
        }

        // POST: Hotel_guest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,First_name,Last_name,Other_names,Address,State,Country,Nextofkin_fullname,Email,Password,Nextofkin_fulladdress,Nextofkin_phonenumber,status,deleted")] Hotel_guest hotel_guest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_guest).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ViewBag.message = "The guest was successfully updated";
                return RedirectToAction("Index");
            }
            else
            {

                ViewBag.message = "There was an error updating the guest detail";
            }

            return View(hotel_guest);
        }

        // GET: Hotel_guest/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_guest hotel_guest = await db.Hotel_guest.FindAsync(id);
            if (hotel_guest == null)
            {
                return HttpNotFound();
            }
            return View(hotel_guest);
        }

        // POST: Hotel_guest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Hotel_guest hotel_guest = await db.Hotel_guest.FindAsync(id);
            db.Hotel_guest.Remove(hotel_guest);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

//        public ActionResult RequestDiscount(int guestId, int emp_number, string guestname)
//        {

//            var request = "Request for discount";
//            Guest guest = new Guest();
//            var user_id = User.Identity.GetUserId();

//            try
//            {
//                //send mail to compalin admin

//                //Get the url of the guest
//                string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
//                            (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
//            Domain += "/Hotel_guest/MDDiscount?guestId=" + guest.GuestID;
//            var email_body = string.Format("A Request for booking<strong>{0}</strong>  discount of 30% has been raised on behalf of<strong>{1}</strong>.<br/>", guest.FirstName + guest.LastName
//               );
//            email_body += "the description is <br/>";
//            email_body += string.Format("<i>{0}</i>.<br/>", request);
//            email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

//            var notification_email = new Email
//            {
//                body = email_body,
//                subject = "Discount - Action required - BricksERP"
//            };
//            notification_email.IsHtml = true;
//            notification_email.body = email_body;
//#if DEBUG
//            //notification_email.to = new List<string>{ UserHelper.GetEmployeeEmail(user_id) };

//            notification_email.to = _hGuestService.GetBookingAdmin();
//#else
//                //notification_email.to = _complainService.GetComplainAdmin();
//#endif

//            NotificationUtil.SendEmailComplain(notification_email, UserHelper.GetEmployeeEmail(user_id));

//            TempData["message"] =
//                "Your request has been saved and email sent to the appropriate persons. Thank You";
//        }
//            catch (Exception e)
//            {
//                TempData["message"] = "There was an error sending the discount request, Please try again";
//                Utils.LogError(e);
//            }

//            return View();
//        }
//        public ActionResult UpdateDiscount()
//        {
//            return View();
//        }

//        public ActionResult GiveDiscount(int guestId, int percentage)
//        {
//            //ViewBag.complain = _hGuestService.GetGuest(guestId);


//            var user_id = User.Identity.GetUserId();
//            var employee = _employeeService.GetEmployeeByUserId(user_id);

//            ViewBag.employee = _employeeService.GetEmployee(employee.emp_number);

//            //var guest_record = new Hotel_guest();

//            var guest_old = db.Hotel_guest.First(e => e.Id == guestId);

//            guest_old.DiscountapprovedBy = employee.emp_firstname + employee.emp_lastname + employee.emp_middle_name;
          
//            guest_old.DiscountPercentage = percentage;
//            guest_old.LastDiscountPercentage = percentage;

//            guest_old.EmailOfDiscountapprovedPerson = employee.emp_work_email;
//            guest_old.emp_number = employee.emp_number;
//            guest_old.AddedDate = DateTime.Now;
//            guest_old.UpdatedDate = DateTime.Now;


//            if (guest_old.DateOfLastDiscountUpdate == null)
//            {
//                guest_old.DateOfLastDiscountUpdate = DateTime.Now;
//            }
//            else
//            {
//                guest_old.DateOfLastDiscountUpdate = guest_old.DateOfLastDiscountUpdate;
//            }

//            //db.Entry(guest_old).State = EntityState.Modified;

//            //db.SaveChanges();


//            return RedirectToAction("CompanyPages", "GuestProfile", new { area = "Admin" });
//            //return RedirectToAction("CompanyPages/GuestProfile");
//        }

//        public ActionResult MDDiscount(int guestId, int percentage)
//        {
//            ViewBag.complain = _hGuestService.GetGuest(guestId);


//            var user_id = User.Identity.GetUserId();
//            var employee = _employeeService.GetEmployeeByUserId(user_id);

//            ViewBag.employee = _employeeService.GetEmployee(employee.emp_number);

//            //var guest_record = new Hotel_guest();
          



//            var guest_old = db.Hotel_guest.First(e => e.Id == guestId);

//            guest_old.DiscountapprovedBy = employee.emp_firstname + employee.emp_lastname + employee.emp_middle_name;
//            if (employee.job_title_code== 5077)
//            {
//                guest_old.DiscountPercentage = percentage;
//                guest_old.LastDiscountPercentage = percentage;
               
//            }
//            else if (employee.job_title_code == 5081)
//            {

//            }
//            else if (employee.job_title_code == 4037)
//            {

//            }



//            guest_old.DiscountPercentage = percentage;
//            guest_old.LastDiscountPercentage = percentage;

//            guest_old.EmailOfDiscountapprovedPerson = employee.emp_work_email;
//            guest_old.emp_number = employee.emp_number;
//            guest_old.AddedDate = DateTime.Now;
//            guest_old.UpdatedDate = DateTime.Now;


//            if (guest_old.DateOfLastDiscountUpdate == null)
//            {
//                guest_old.DateOfLastDiscountUpdate = DateTime.Now;
//            }
//            else
//            {
//                guest_old.DateOfLastDiscountUpdate = guest_old.DateOfLastDiscountUpdate;
//            }

//            db.Entry(guest_old).State = EntityState.Modified;

//            db.SaveChanges();


//            return RedirectToAction("GuestProfile");
//        }

//        public ActionResult ManagersDiscount(int guestId, int percentage)
//        {
//            return RedirectToAction("GuestProfile");
//        }
        public ActionResult GuestProfile()
        {

            return View();
        }

    }
}
