using System;
using System.Linq;
using System.Threading.Tasks;
using HRM.DAL.IService;
using System.Web.Mvc;
using HRM.DAL.Models;

using PagedList;
using OnePortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnePortal.Helper;
using HRM.DAL.Util;
using System.Collections.Generic;
using System.Web;
using Microsoft.Owin.Security;
using OnePortal.Models.ViewModels;
using Newtonsoft.Json;

namespace OnePortal.Controllers
{
    public class LaundryController : Controller
    {
        IReservationService _reservationService;
        IGuestService _guestService;
        IRoomService _roomService;
        ILookupService _lookupService;
        IBookingsService _bookingService;
        ILaundryService _laundryService;
        IEmployeeService _employeeService;

        public LaundryController(ILaundryService laundryService, IBookingsService bookingService, ILookupService lookupService, IEmployeeService employeeService, IGuestService guestService, IRoomService roomService, IReservationService reservationService)
        {
            _bookingService = bookingService;
            _laundryService = laundryService;
            _guestService = guestService;
            _roomService = roomService;
            _reservationService = reservationService;
            _lookupService = lookupService;
            _employeeService = employeeService;
            // _employeeService = employeeService;
        }


        // GET: Laundary
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost]
        [PermissionFilter(permission = "employee.create")]
        public async Task<ActionResult> Create(admin_hrm_employee employee, Guest guests, collection_register register, delivery deliver, pricing price, billing bill, FormCollection collection, User_Email email)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            /*
             * Added By Sanmi
             * To Check if an customer exists before creating
             * This check is done by email comparism
             */
            var existing_guest = _bookingService.GetGuest(guests.GuestID);
            var deacivated_guest = _bookingService.GetDeactivatedGuest(guests.LastName);
            if (existing_guest != null && deacivated_guest != null)
            {
                ViewBag.message = "The guest has already been added or was deactivated, Please check and make sure that the email address is correct and has not been added before";
                return View();
            }

            //var employee = _employeeService.GetEmployee(Convert.ToInt32(emp_number));
            guests.FirstName = email.EmailAddress;

            //var laundryobj = new Laundry
            //{
            //    ServiceType = register.ServiceType,
            //    GuestEmail = email.EmailAddress,
            //    Description = register.Description,
            //    CollectionDate = register.collection_date,
            //    DeliveryDate = register.delivery_date,
            //    GuestName = guests.FirstName + "" + guests.LastName,
            //    Quantity = register.quantity,
            //    Price = price.washing_price,
            //    Deleted = register.Deleted.ToString(),
            //    EmployeeName = employee.emp_firstname + "" + employee.emp_lastname,

            //};

            var laundryobj = new collection_register
            {
                ServiceType = register.ServiceType,
                price = register.price

            };

            laundryobj.id = _laundryService.AddEntry(laundryobj);



            var guest_number = _bookingService.AddGuest(guests);
            if (guest_number > 0)
            {
                guests.GuestID = guest_number;
                //job_Record.employee_number = employee.emp_number;

                //add the job_record of the employee
                //_employeeService.AddJobRecord(job_Record);

                //add supervisor
                //var supervisor = Convert.ToInt32(collection["supervisor"]);
                //var reportto = new admin_hrm_emp_reportto
                //{
                //    erep_sup_emp_number = supervisor,
                //    erep_sub_emp_number = emp_number,
                //    erep_reporting_mode = 1
                //};
                //_employeeService.AddReportTo(reportto);

                //add the location of the employee
                //_employeeService.AddLocation(new admin_hrm_emp_locations { location_id = job_Record.location_id.Value, emp_number = emp_number });
                //create an account for the user

                //var user = new ApplicationUser { UserName = employee.emp_work_email, Email = employee.emp_work_email, RegistrationDate = DateTime.Now };
                //var pass = Utils.GeneratePassword(3, 3, 3) + "!";
                //var result = await UserManager.CreateAsync(user, pass);
                //if (result.Succeeded)
                //{
                //    employee.user_id = user.Id;
                //    _employeeService.UpdateEmployee(employee);
                //    UserManager.AddToRole(user.Id, collection["role"].ToString());

                //    var template = EmailUtil.GetTemplate("account_setup");
                //    string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                //    template = template.Replace("{name}", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname));
                //    template = template.Replace("{workspace_name}", "WorkSpace");
                //    template = template.Replace("{pass}", pass);
                //    template = template.Replace("{url}", Domain);

                //    var email = new Email
                //    {
                //        body = template,
                //        subject = "Project - Brinkspoint ERP"
                //    };

                //    email.to = new List<string> { employee.emp_work_email };
                //    email.IsHtml = true;

                //    NotificationUtil.SendEmail(email);
                //    //string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

                //    NotificationUtil.SendNotifications(new int[] { employee.emp_number }, template, Domain);
                //    ViewBag.message = "The employee was created successfully, email was sent, consider sending the employee a confirmatory email with employee password. " +
                //                      "Employee password is " + pass + " Also urge employee to change their password upon login";
                //}
                //return View("Edit",emp_number);
            }
            return View();
        }

        [PermissionFilter(permission = "employee.index")]
        public ActionResult IndexAll()
        {
            ViewBag.guest = _bookingService.GetAllGuest();
            ViewBag.entry = _laundryService.GetAllEntry();
            return View();
        }

        public void Getgeust()
        {
            ViewBag.guest = _bookingService.GetAllGuest();
            ViewBag.entry = _laundryService.GetAllEntry();
           
        }

        public ActionResult Index(int? page, string search = null)
        {
            return View(_laundryService.GetAllEntry().OrderBy(e => e.collection_date));

        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult IndexPrice(int? year, int? month,  string search)
        {
            var yr = DateTime.Now.Year;
            var mnth = DateTime.Now.Month;
         
            if (year.HasValue && month.HasValue)
            {
                yr = year.Value;
                mnth = month.Value;

            }


            ViewBag.price = _laundryService.GetAllPrice().OrderBy(e => e.created_date).Where(e => e.Deleted == 0);
            ViewBag.search = search;
            ViewBag.month = mnth;
            ViewBag.year = yr;
            ViewBag.requestData2 = _laundryService.GetAllPrice();

            return View();
        }

        public ActionResult CreatePrice(pricing price)
        {
            try
            {
                var prices = new pricing
                {
                    ClothName = price.ClothName,
                    Description = price.Description,
                    Deleted = price.Deleted = 0,
                    created_date = DateTime.Now

                    //product_description = prodct.product_description,
                    //    product_status = prodct.product_status,
                    //    product_quality = prodct.product_quality,
                    //    quantity = prodct.quantity,
                    //    created_date = DateTime.Now,

                    //Priority = comp.Priority,
                    //Type = comp.Type,
                    //UserId = user_id,
                    //Comment = comp.Comment,
                    //CreateDate = DateTime.Now,
                    //Deleted = 0,
                    //Resolved = "No"

                };

                prices.id = _laundryService.AddPricing(prices);

                //TempData["message"] = "Your complain has been saved and email sent to the appropriate persons. Thank You";

                TempData["alert_type"] = "alert-success";
                TempData["message"] = "Successfully add new Item and price";
            }
            catch (Exception e)
            {
                TempData["alert_type"] = "alert-danger";
                TempData["message"] = "Encountered error, check the values entered";
                Utils.LogError(e);
            }




            //if (ModelState.IsValid)
            //{
            //    productcategory.Deleted = 0;
            //    productcategory.date_updated = DateTime.Now;
            //    _inventoryService.AddProductCategory(productcategory);

            //    TempData["alert_type"] = "alert-success";
            //    TempData["message"] = "Successfully add new Product Category";
            //}
            //else
            //{
            //    TempData["alert_type"] = "alert-danger";
            //    TempData["message"] = "Encountered error, check the values entered";
            //}

            return RedirectToAction("IndexPrice");
        }


        public ActionResult AddPrice(pricing price)
        {
            var user_id = User.Identity.GetUserId();
            try
            {
                var prices = new pricing
                {

                    ClothName = price.ClothName,
                    washing_price = price.washing_price,
                    clotheType = price.clotheType,
                    created_date = DateTime.Now,
                    Description = price.Description,
                    Deleted = 0,
                    
                };
                prices.id = _laundryService.AddPricing(prices);

                TempData["alert_type"] = "alert-success";
                TempData["message"] = "Successfully add new Price";

            }

            catch (Exception e)
            {
                TempData["message"] = "There was an error saving the items price, Please try again";
                Utils.LogError(e);
            }


            // return View();
            return RedirectToAction("IndexPrice");

        }

        //[HttpGet]
        //public string UpdatePrice(int Id)
        //{
        //    ViewBag.price = _laundryService.GetPrice(Id);


        //    //var fetchedPrice = _laundryService.GetAllPrice().Select(m => new
        //    //{
        //    //    id = m.id,
        //    //    Name = m.clotheType,
        //    //    description = m.Description,
        //    //    UpdatedDate = DateTime.Now,
        //    //    Type = m.clotheType,

        //    //    deleted = m.Deleted,

        //    //}).FirstOrDefault(e => e.id == categoryId && e.deleted == 0);
        //    //if (fetchedPrice == null)
        //    //{
        //    //    return "not found";
        //    //}

        //    //return JsonConvert.SerializeObject(fetchedPrice);

        //    return View();
        //}

       

        [HttpGet]
        public ActionResult UpdatePrice(int id)
        {  //var c = _laundryService.GetPrice(id);
            ViewBag.price = _laundryService.GetPrice(id);
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePrice(pricing price)
        {
            _laundryService.UpdatePrice(price);
            ViewBag.message = "The Price of the item was successfully updated";
            try
            {
                _laundryService.UpdatePricee(price);
                ViewBag.message = "The Price of the item was successfully updated";
                // TempData["alert_type"] = "alert-success";
                //TempData["message"] = "The Price of the item was successfully updated";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                //TempData["alert_type"] = "alert-danger";
                //TempData["message"] = "There was an error updating Price";

                ViewBag.message = "There was an error updating the Price";
            }

            ViewBag.price = _laundryService.GetPrice(price.id);

            //return RedirectToAction("IndexPrice");
            return View();
        }
      

        public string DeletePrice(int id)
        {
            int deleted = _laundryService.DeletePrice(id);
            if (deleted > 0)
                return "The Item Price was deleted successfully";
            return "There was an error deleting the Price";
        }



        public ActionResult IndexEntry()
        {
            return View();
        }

        public ActionResult AddEntry(collection_register collection, entry_information entry)
        {
           

            var user_id = User.Identity.GetUserId();
            try
            {
                //var prices = new pricing
                //{

                //    ClothName = price.ClothName,
                //    clotheType = price.clotheType,
                //    created_date = DateTime.Now,
                //    Description = price.Description,
                //    Deleted = 0,

                //};

            }

            catch (Exception e)
            {
                TempData["message"] = "There was an error saving the items price, Please try again";
                Utils.LogError(e);
            }


            return View();

        }

        
        public ActionResult UpdateEntry()
        {

            return RedirectToAction("IndexEntry");
        }


        public ActionResult IndexAudit()
        {
            return View();
        }

        public string DeleteEntry(int id)
        {
            int deleted = _laundryService.DeleteEntry(id);
            if (deleted > 0)
                return "The Entry was deleted successfully";
            return "There was an error deleting the Entry";
        }
    }
}