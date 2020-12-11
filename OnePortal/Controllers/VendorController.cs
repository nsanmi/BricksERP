using HRM.DAL.IService;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRM.DAL.Util;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.IService;

namespace OnePortal.Controllers
{
    [Authorize]
    public class VendorController : Controller
    {
        IVendorService _vendorService;
        IEmployeeService _employeeService;
        public VendorController(IVendorService vendorService,IEmployeeService employeeService)
        {
            _vendorService = vendorService;
            _employeeService = employeeService;
        }

        // GET: Vendor
        public ActionResult Vendors(int? page,string search=null)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            if (search != null)
            {
                IPagedList<bpm_vendor> vendors = _vendorService.GetVendors().Where(e => e.deleted == 0 && (e.company_name.Contains(search) || e.name.Contains(search) || e.phone.Contains(search))).OrderBy(e => e.created_at).ToPagedList(pageIndex, pageSize);
                return View(vendors);
            }
            else
            {
                IPagedList<bpm_vendor> vendors = _vendorService.GetVendors().Where(m => m.deleted == 0).OrderBy(e => e.created_at).ToPagedList(pageIndex, pageSize);
                return View(vendors);
            }
        }

        [HttpPost]
        public ActionResult Vendors(bpm_vendor vendor)
        {
            var form = Request.Form;
            vendor.State = 1;
            vendor.Lga = 1;
            _vendorService.AddVendor(vendor);
            var categories = form["category"].ToString().Split(',');
            var vendor_categories = new List<bpm_lnk_vendor_category>();
            foreach(var c in categories)
            {
                vendor_categories.Add(new bpm_lnk_vendor_category {category_id=Convert.ToInt32(c),vendor_id=vendor.id });
            }
            _vendorService.AddCategory(vendor_categories);
            return RedirectToAction("Vendors");
        }

        public ActionResult Manage(int id)
        {
            ViewBag.message = TempData["message"];
            ViewBag.V_message = TempData["V_message"];

            return View(_vendorService.GetVendor(id));
        }

        [HttpPost]
        public ActionResult Manage(bpm_vendor vendor)
        {
            var form = Request.Form;

            try
            {
                _vendorService.UpdateVendor(vendor);
                var categories = form["category"]?.Split(',');
                var vendor_categories = new List<bpm_lnk_vendor_category>();
                if (categories != null)
                {
                    foreach (var c in categories)
                    {
                        vendor_categories.Add(new bpm_lnk_vendor_category
                        {
                            category_id = Convert.ToInt32(c),
                            vendor_id = vendor.id
                        });
                    }

                    _vendorService.AddCategory(vendor_categories);

                }
                TempData["V_message"] = "Vendor updated successfully";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                TempData["V_message"] = "Error ocuured, try again";
            }

            return RedirectToAction("Manage", new { id = vendor.id });
        }

        [HttpPost]
        public ActionResult UploadDocument(bpm_vendor_document document)
        {

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);


            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                var directory = "~/Documents/Vendors/" + document.vendor_id;
                if (!Directory.Exists(Server.MapPath(directory)))
                {
                    Directory.CreateDirectory(Server.MapPath(directory));
                }

                var file_path = "~/Documents/Vendors/" + document.vendor_id + "/" + file.FileName;
                document.file_path = file_path;
                document.created_at = DateTime.Now;
                document.created_by = employee.emp_number;
                try
                {
                    _vendorService.AddDocument(document);
                    TempData["message"] = "Vendor's document was saved";
                }
                catch (Exception e)
                {
                    Utils.LogError(e);
                    TempData["message"] = "Error Occured, Retry";
                }
            }

            return RedirectToAction("Manage", new { id = document.vendor_id });
        }

        /*
         * Added by Johnbosco
         * For more on Managing of Vendors
         */
        [HttpPost]
        public string DeleteVendor(int id)
        {
            return _vendorService.DeleteVendor(id) ? "Vendor deleted successfully" : "Something went wrong, Please try again";
        }
    }
}