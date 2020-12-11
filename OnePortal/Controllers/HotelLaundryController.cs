using System;
using System.Linq;
using System.Threading.Tasks;
//using HRM.DAL.IService;
using System.Web.Mvc;
//using HRM.DAL.Models;
using PagedList;
using OnePortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnePortal.Helper;
//using HRM.DAL.Util;
using System.Collections.Generic;
using System.Web;
using Microsoft.Owin.Security;
using OnePortal.Models.ViewModels;
using Newtonsoft.Json;
using Hotel.Dal.Models;
using Hotel.Dal.IService;
using HRM.DAL.IService;
using HRM.DAL.Util;
using System.Web.UI.WebControls;
//using HRM.DAL.IService;

namespace OnePortal.Controllers
{
    public class HotelLaundryController : Controller
    {

        private HotelDBEntities db = new HotelDBEntities();
        // GET: HotelLaundry

        //IReservationService _reservationService;
       // IGuestService _guestService;
       // IRoomService _roomService;
       // ILookupService _lookupService;
        IBookingService _bookingService;
        IHLaundryService _hlaundryService;
        IHGuestService _hGuestService;
        ILookupService _lookupService;
        IEmployeeService _employeeService;

        public HotelLaundryController(IHLaundryService hlaundryService, IBookingService bookingService, ILookupService lookupService, IEmployeeService employeeService, IHGuestService guestService, IRoomService roomService)
        {
            _bookingService = bookingService;
            _hlaundryService = hlaundryService;
            _hGuestService = guestService;
            _lookupService = lookupService;
            _employeeService = employeeService;
            // _employeeService = employeeService;
        }



        [HttpPost]
        //[PermissionFilter(permission = "employee.create")]
        public async Task<ActionResult> Create(admin_hrm_employee employee, Hotel_guest guests, collection_register register, delivery deliver, pricing price, billing bill, FormCollection collection, LaundryVModel laundry)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            /*
             * Added By Sanmi
             * To Check if a customer exists before creating
             * This check is done by email comparism
             */
            var existing_guest = _hGuestService.GetGuest(guests.Id);
            var deacivated_guest = _hGuestService.GetDeactivatedGuest(guests.Last_name);
            if (existing_guest != null && deacivated_guest != null)
            {
                ViewBag.message = "The guest has already been added or was deactivated, Please check and make sure that the email address is correct and has not been added before";
                return View();
            }

           // var Emo = laundry;

           // var collections = new collection_register
           // {
           //     Guest_name = laundryobj.First_name + "" + laundryobj.Last_name,
           //     cloth_quantity = laundry.Quantity,
           //     price = laundry.price,
           //     collection_date = DateTime.Now,
           //     proposed_delivery_date = laundry.delivery_date,
           //     delivery_personel = laundryobj.Personel,
           //     Description = laundry.Description,
           //     Emp_name = laundryobj.Personel,



           // };

           //collections.id = _hlaundryService.AddEntry(collections);


           // var entryonj = new entry_information
           // {
           //     quantity = entry.quantity,
           //     collection_id = laundry.Id,
           //     Total_amount = entry.Total_amount,

           // };

           // entryonj.id = _hlaundryService.AddCollection(entryonj);

            //var employee = _employeeService.GetEmployee(Convert.ToInt32(emp_number));
            //guests.First_name = .EmailAddress;

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



            var guest_number = _hGuestService.AddGuest(guests);
            if (guest_number > 0)
            {
                guests.Id = guest_number;
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
            //DataSeed emp = new DataSeed();
            //ViewBag.Name = name;
            var names = laundry;
            ViewBag.Laundry = laundry;
            return View(laundry);
        }

        //[HttpPost]
        //public ActionResult SubmitData(DataSeed emp)
        //{
        //    TempData["Emp"] = emp;
        //    return Redirect("/HotelLaundry/Create");
        //}

        public ActionResult Create(LaundryVModel laundry)
        {
            
            //entryonj.id = _hlaundryService.AddCollection(entryonj);


            ViewBag.Laundry = laundry;
            return View(laundry);
        }


        public ActionResult Make(LaundryViewModel laundryobj)
        {
            //entry_information entry = new entry_information();


                var collection = new collection_register
            {
                Guest_name = laundryobj.First_name + "" + laundryobj.Last_name,
                quantity = laundryobj.GrandQuantity,
                price = laundryobj.Grandprice,
                collection_date = DateTime.Now,
                proposed_delivery_date = laundryobj.delivery_date,
                delivery_personel = laundryobj.Personel,
                Description = laundryobj.Description,
                //Emp_name = laundryobj.Personel,
                guest_room_number = laundryobj.Room_number,

                };

            collection.id = _hlaundryService.AddEntry(collection);
            
            var clothes = new List<entry_information>();
            var clothe = new List<RoomViewModel>();
           


            for (var i = 1; i <= laundryobj.quantity.Count; i++)
            {

                clothes.Add(new entry_information()
                {
                    Guest_name = laundryobj.First_name + " " + laundryobj.Last_name,
                    collection_id = laundryobj.CollectionId[i],
                    price_par_clothe_entry = laundryobj.price[i],
                    Total_amount = laundryobj.totalprice[i].ToString(),
                    quantity_par_clothe_entry = laundryobj.quantity[i],
                    quantity = laundryobj.totalquantity[i]
                });

            }
            _hlaundryService.AddCol(clothes);
           



            return RedirectToAction("IndexAll");
        }


        public class DataSeed
        {
            public string Room_number { get; set; }
            public string First_name
            {
                get;
                set;
            }
            public string Last_name
            {
                get;
                set;
            }
           

        }

        //[PermissionFilter(permission = "employee.index")]
        public ActionResult IndexAll(int? page, string search = null)
        {
            int pageIndex = 1;
            int pageSize = 15;
            pageIndex = page.HasValue ? page.Value : 1;
            var userId = User.Identity.GetUserId();
            //set the return message 
            ViewBag.message = TempData["message"];
            if (search != null)
            {
                //_complainService.GetAllComplain().Where(e => e.UserId.Contains(search) || e.Type.Contains(search) || e.Priority.Contains(search) || e.AspNetUser.UserName.Contains(search) || e.Resolved.Contains(search)).OrderBy(e => e.UpdateDate).ToPagedList(pageIndex, pageSize);
                ViewBag.search = search;
                return View(_hlaundryService.GetAllEntry().OrderBy(e => e.collection_date).ToPagedList(pageIndex, pageSize));
            }
            return View(_hlaundryService.GetAllEntry().OrderBy(e => e.collection_date).ToPagedList(pageIndex, pageSize));

            //ViewBag.guest = _hGuestService.GetAllGuest();
            //ViewBag.entry = _hlaundryService.GetAllEntry();
            // return View();
        }

        public void Getgeust()
        {
            ViewBag.guest = _hGuestService.GetAllGuest();
            ViewBag.entry = _hlaundryService.GetAllEntry();

        }

        public ActionResult Test()
        {
            DataSeed emp = new DataSeed();
            return View(emp);
            
        }

       public async Task<ActionResult> IndexReservation(int? year, int? month, int? category, int? category_id, string search)
            {
                var yr = DateTime.Now.Year;
                var mnth = DateTime.Now.Month;
                int cat = 1;
                int cate = 1;

                if (category_id.HasValue) cate = category_id.Value;

                if (category.HasValue)
                {
                    cat = category.Value;

                }

                if (year.HasValue && month.HasValue)
                {
                    yr = year.Value;
                    mnth = month.Value;

                }



                //ViewBag.audits = _inventoryService.GetMonthlyAudit(yr, mnth);
                ViewBag.search = search;
                ViewBag.category = cat;
                ViewBag.month = mnth;
                ViewBag.year = yr;
                ViewBag.prcategory = cate;


            //DataSeed emp = TempData["Emp"] as DataSeed;
            //DataSeed emp = new DataSeed();
            //return View();


            var hotel_reservation = db.Hotel_reservation.AsQueryable();
            ViewBag.reservation = hotel_reservation;
               return View(hotel_reservation);
            }



        public ActionResult Index(int? page, string search = null)
        {
            return View(_hlaundryService.GetAllEntry().OrderBy(e => e.collection_date));

        }

       

        public ActionResult IndexPrice(int? year, int? month, string search)
        {
            var yr = DateTime.Now.Year;
            var mnth = DateTime.Now.Month;

            if (year.HasValue && month.HasValue)
            {
                yr = year.Value;
                mnth = month.Value;

            }


            ViewBag.price = _hlaundryService.GetAllPrice().OrderBy(e => e.created_date).Where(e => e.Deleted == 0);
            ViewBag.search = search;
            ViewBag.month = mnth;
            ViewBag.year = yr;
            ViewBag.requestData2 = _hlaundryService.GetAllPrice();

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

                prices.id = _hlaundryService.AddPricing(prices);

                //TempData["message"] = "Your complain has been saved and email sent to the appropriate persons. Thank You";

                TempData["alert_type"] = "alert-success";
                TempData["message"] = "Successfully add new Item and price";
            }
            catch (Exception e)
            {
                TempData["alert_type"] = "alert-danger";
                TempData["message"] = "Encountered error, check the values entered";
               
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
                prices.id = _hlaundryService.AddPricing(prices);

                TempData["alert_type"] = "alert-success";
                TempData["message"] = "Successfully add new Price";

            }

            catch (Exception e)
            {
                TempData["message"] = "There was an error saving the items price, Please try again";
               
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
            ViewBag.price = _hlaundryService.GetPrice(id);
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePrice(pricing price)
        {
            _hlaundryService.UpdatePrice(price);
            ViewBag.message = "The Price of the item was successfully updated";
            try
            {
                _hlaundryService.UpdatePricee(price);
                ViewBag.message = "The Price of the item was successfully updated";
                // TempData["alert_type"] = "alert-success";
                //TempData["message"] = "The Price of the item was successfully updated";
            }
            catch (Exception e)
            {
               
                //TempData["alert_type"] = "alert-danger";
                //TempData["message"] = "There was an error updating Price";

                ViewBag.message = "There was an error updating the Price";
            }

            ViewBag.price = _hlaundryService.GetPrice(price.id);

            //return RedirectToAction("IndexPrice");
            return View();
        }


        public string DeletePrice(int id)
        {
            int deleted = _hlaundryService.DeletePrice(id);
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
            int deleted = _hlaundryService.DeleteEntry(id);
            if (deleted > 0)
                return "The Entry was deleted successfully";
            return "There was an error deleting the Entry";
        }
    }
}

