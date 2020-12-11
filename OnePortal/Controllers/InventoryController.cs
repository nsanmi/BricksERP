using HRM.DAL.IService;
using Microsoft.AspNet.Identity;
using PagedList;
using HRM.DAL.IService;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using HRM.DAL.Models;
using HRM.DAL.Util;
using System.Collections.Generic;
using PagedList;
using System.Linq;
using OnePortal.Helper;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using OnePortal.Models;
using System.Threading.Tasks;
using OnePortal.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace OnePortal.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
            IInventoryService _inventoryService;
            ILookupService _lookupService;
            IEmployeeService _employeeService;
            public InventoryController(  IInventoryService inventoryService, ILookupService lookupService, IEmployeeService employeeService)
            {
            
               _inventoryService = inventoryService;
               _lookupService = lookupService;
                _employeeService = employeeService;
            }

        

            // GET: Inventory
         
        //public ActionResult Index(int? page, string search = null)
        //{
        //    int pageIndex = 1;
        //    int pageSize = 15;
        //    pageIndex = page.HasValue ? page.Value : 1;
        //    var userId = User.Identity.GetUserId();
        //    //set the return message 
        //    ViewBag.message = TempData["message"];
        //    if (search != null)
        //    {
        //        //_complainService.GetAllComplain().Where(e => e.UserId.Contains(search) || e.Type.Contains(search) || e.Priority.Contains(search) || e.AspNetUser.UserName.Contains(search) || e.Resolved.Contains(search)).OrderBy(e => e.UpdateDate).ToPagedList(pageIndex, pageSize);
        //        ViewBag.search = search;
        //        return View(_inventoryService.GetAllProduct().Where(m => m.AspNetUser.Id == userId && m.Deleted == 0).Where(e => e.Type.Contains(search) || e.Priority.Contains(search)).OrderBy(e => e.Resolved).ToPagedList(pageIndex, pageSize));
        //    }
        //    return View(_complainService.GetAllComplain().Where(m => m.AspNetUser.Id == userId && m.Deleted == 0).OrderBy(e => e.Resolved).ToPagedList(pageIndex, pageSize));
        //}

        public ActionResult Index(int? page, string search = null)
        {
            int pageIndex = 1;
            int pageSize = 15;
            pageIndex = page.HasValue ? page.Value : 1;
            if (search != null)
            {
                ViewBag.search = search;
                return View(_inventoryService.GetAllProduct().Where(m => m.Deleted == 0).Where(e => e.product_name.Contains(search) || e.product_type.Contains(search)).OrderBy(e => e.product_status).ToPagedList(pageIndex, pageSize));
            }
            return View(_inventoryService.GetAllProduct().Where(e => e.Deleted == 0).OrderByDescending(e => e.created_date).ToPagedList(pageIndex, pageSize));
        }

        public ActionResult IndexAll(int? year, int? month,int? category, int? category_id, string search)
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

          
            ViewBag.employees = _employeeService.GetEmployees();
            ViewBag.product = _inventoryService.GetAllProduct().OrderBy(e => e.product_status);
            //ViewBag.audits = _inventoryService.GetMonthlyAudit(yr, mnth);
            ViewBag.search = search;
            ViewBag.category = cat;
            ViewBag.month = mnth;
            ViewBag.year = yr;
            ViewBag.prcategory = cate;
            ViewBag.requestData = _inventoryService.GetAllProductCategory();
            ViewBag.requestData2 = _inventoryService.GetAllProduct();

            return View();
        }

        public ActionResult Create()
        {

            return View();
        }


        // MAKE: Complain
        [HttpPost]
        public ActionResult Make(product prodct)
        {
            var user_id = User.Identity.GetUserId();
            try
            {
                var product = new product
                {
                    product_name = prodct.product_name,
                    product_description = prodct.product_description,
                    product_status = prodct.product_status,
                    product_quality = prodct.product_quality,
                    quantity = prodct.quantity,
                    created_date = DateTime.Now,
                    
                    //Priority = comp.Priority,
                    //Type = comp.Type,
                    //UserId = user_id,
                    //Comment = comp.Comment,
                    //CreateDate = DateTime.Now,
                    //Deleted = 0,
                    //Resolved = "No"
                };

                product.product_id = _inventoryService.AddProduct(product);

                string directory = Server.MapPath("~/Documents/Complain/" + prodct.product_type) + "\\";
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }

                for (int i = 1; i < Request.Files.Count; i++)
                {
                    var uniqueName = string.Format("{0:dd-MM-yyyy_hh_mm_ss}", DateTime.Now);
                    var names = Request.Files[i].FileName.Split('\\');
                    var filePath = directory + uniqueName + "_" + names[names.Length - 1];
                    Request.Files[i].SaveAs(filePath);
                    _inventoryService.AddProductFiles(new product_files
                    {
                        product_id = product.product_id,
                        Filename = "Documents/Product/" + prodct.product_type + "/" + uniqueName + "_" + names[names.Length - 1]
                    });
                }
                //send mail to compalin admin

                //Get the url of the complain
                string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                                (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                Domain += "/complain/update?complainId=" + product.product_id;
                var email_body = string.Format("A complain of <strong>{0}</strong> priority has been raised on <strong>{1}</strong>.<br/>", product.product_status,
                    prodct.product_type);
                email_body += "the description is <br/>";
                email_body += string.Format("<i>{0}</i>.<br/>", product.product_description);
                email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

                var notification_email = new Email
                {
                    body = email_body,
                    subject = "Product - Action required - BricksERP"
                };
                notification_email.IsHtml = true;
                notification_email.body = email_body;
#if DEBUG
                //notification_email.to = new List<string>{ UserHelper.GetEmployeeEmail(user_id) };

                notification_email.to = _inventoryService.GetProductAdmin();
#else
                //notification_email.to = _complainService.GetComplainAdmin();
#endif

                NotificationUtil.SendEmailComplain(notification_email, UserHelper.GetEmployeeEmail(user_id));

                TempData["message"] =
                    "Your complain has been saved and email sent to the appropriate persons. Thank You";
            }
            catch (Exception e)
            {
                TempData["message"] = "There was an error saving the complain, Please try again";
                Utils.LogError(e);
            }


            return RedirectToAction("Index");
        }


        public ActionResult IndexCategory()
        {

            ViewBag.message = TempData["message"];
            ViewBag.alert_type = TempData["alert_type"];
            ViewBag.requestData = _inventoryService.GetAllProductCategory();

            return View();
        }

        public ActionResult CreateCategory(product_category productcategory)
        {
            try
            {
                var productcat = new product_category
                {
                    name = productcategory.name,
                    description = productcategory.description,
                    Deleted = productcategory.Deleted = 0,
                    date_created = DateTime.Now

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

                productcat.category_id = _inventoryService.AddProductCategory(productcat);

                
                TempData["alert_type"] = "alert-success";
               TempData["message"] = "Successfully add new Product Category";
            }
            catch (Exception e)
            {
                TempData["alert_type"] = "alert-danger";
                TempData["message"] = "Encountered error, check the values entered";
                Utils.LogError(e);
            }
            

            return RedirectToAction("IndexCategory");
        }


        [HttpGet]
        public ActionResult Update(int product_id)
        {
            ViewBag.product = _inventoryService.GetProducts(product_id);

            return View();
        }

        [HttpPost]
        public ActionResult Update(product prod)
        {
            try
            {
                _inventoryService.UpdateProduct(prod);
                ViewBag.message = "The product was successfully updated";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                ViewBag.message = "There was an error updating the product";
            }
            ViewBag.product = _inventoryService.GetProducts(prod.product_id);
            //return View();
            return RedirectToAction("IndexAll");
        }

        public string DeleteProduct(int id)
        {
            int deleted = _inventoryService.DeleteProduct(id);
            if (deleted > 0)
                return "The Product was deleted successfully";
            return "There was an error deleting the Product";
        }

        [HttpGet]
        public ActionResult UpdateCat(int category_id)
        {
            ViewBag.category = _inventoryService.GetProductCategory(category_id);

            return View();
        }

        [HttpPost]
        public ActionResult UpdateCat(product_category cat)
        {
            try
            {
                _inventoryService.UpdateProductCategory(cat);
                ViewBag.message = "The product category was successfully updated";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                ViewBag.message = "There was an error updating the product";
            }
            ViewBag.category = _inventoryService.GetProductCategory(cat.category_id);
            //return View();
            return RedirectToAction("IndexCategory");
        }


        [HttpGet]
        public string UpdateCategory(int categoryId)
        {
            var fetchedProductCategory = _inventoryService.GetAllProductCategory().Select(m => new
            {
                id = m.category_id,
                Name = m.name,
                Description = m.description,
                UpdatedDate = m.date_updated,
                deleted = m.Deleted,
              
            }).FirstOrDefault(e => e.id == categoryId && e.deleted == 0);
            if (fetchedProductCategory == null)
            {
                return "not found";
            }

            return JsonConvert.SerializeObject(fetchedProductCategory);
        }

        [HttpPost]
        public ActionResult UpdateCategory(product_category productcategory)
        {
            try
            {
                _inventoryService.UpdateProductCategory(productcategory);
                TempData["alert_type"] = "alert-success";
                TempData["message"] = "The Product Category was successfully updated";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                TempData["alert_type"] = "alert-danger";
                TempData["message"] = "There was an error updating Product Category";
            }
            return RedirectToAction("IndexCategory");
        }

        public string DeleteCategory(int id)
        {
            int deleted = _inventoryService.DeleteProductCategory(id);
            if (deleted > 0)
                return "The Product Category was deleted successfully";
            return "There was an error deleting the Product Category";
        }



        public ActionResult AddCategory(product_category cat)
        {
            var user_id = User.Identity.GetUserId();
            try
            {
                var category = new product_category
                {

                    name = cat.name,
                    date_created = DateTime.Now,
                    description = cat.description,
                    Deleted = 0,


                    //Priority = comp.Priority,
                    //Type = comp.Type,
                    //UserId = user_id,
                    //Comment = comp.Comment,
                    //CreateDate = DateTime.Now,
                    //Deleted = 0,
                    //Resolved = "No"
                };

                category.category_id = _inventoryService.AddProductCategory(category);
            }

            catch (Exception e)
            {
                TempData["message"] = "There was an error saving the product category, Please try again";
                Utils.LogError(e);
            }
        

                return View();
            
        }

    }
}