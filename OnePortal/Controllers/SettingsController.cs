using HRM.DAL.IService;
using HRM.DAL.Models;
using HRM.DAL.Util;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace OnePortal.Controllers
{
    [Authorize(Roles = "admin")]
    public class SettingsController : Controller
    {
        ISettingsService _settingsService;
        
        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET: Settings
        public ActionResult ManagePermissions(int? page)
        {
            int pageSize = 50;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<AspNetPermission> permissions = _settingsService.GetPermissions().OrderByDescending(e => e.Id).ToPagedList(pageIndex, pageSize);
            return View(permissions);
           
        }

        public ActionResult AddPermissionsToRole(FormCollection collection)
        {
            var role = collection["role"];
            var permissions = collection["permissions"].ToString().Split(',').ToList();
            var permission_ids = new List<int>();
            foreach(var permission in permissions)
            {
                permission_ids.Add(Convert.ToInt32(permission));
            }
            _settingsService.AddPermissionsToRole(permission_ids, role);
            return RedirectToAction("ManagePermissions");
        }

        public ActionResult IndexJobTitle()
        {
            ViewBag.message = TempData["message"];
            ViewBag.alert_type = TempData["alert_type"];
            ViewBag.requestData = _settingsService.GetAllJobTitle();

            return View();
        }

        public ActionResult CreateJobTitle(admin_hrm_lkup_job_title jobTitle)
        {
            if (ModelState.IsValid)
            {
                jobTitle.is_deleted = 0;
                _settingsService.AddJobTitle(jobTitle);

                TempData["alert_type"] = "alert-success";
                TempData["message"] = "Successfully add new Job";
            }
            else
            {
                TempData["alert_type"] = "alert-danger";
                TempData["message"] = "Encountered error, check the values entered";
            }

            return RedirectToAction("IndexJobTitle");
        }

        [HttpGet]
        public string UpdateJobTitle(int jobTitleId)
        {
            var fetchedjobtitle = _settingsService.GetAllJobTitle().Select(m => new
            {
                id=m.id,title=m.job_title,description=m.job_description, deleted=m.is_deleted, note=m.note
            }).FirstOrDefault(e => e.id == jobTitleId && e.deleted == 0);
            if (fetchedjobtitle == null)
            {
                return "not found";
            }

            return JsonConvert.SerializeObject(fetchedjobtitle);
        }

        [HttpPost]
        public ActionResult UpdateJobTitle(admin_hrm_lkup_job_title jobTitle)
        {
            try
            {
                _settingsService.UpdateJobTitle(jobTitle);
                TempData["alert_type"] = "alert-success";
                TempData["message"] = "The job title was successfully updated";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                TempData["alert_type"] = "alert-danger";
                TempData["message"] = "There was an error updating job title";
            }
            return RedirectToAction("IndexJobTitle");
        }

        public string DeleteJobTitle(int id)
        {
            int deleted = _settingsService.DeleteJobTitle(id);
            if (deleted > 0)
                return "The job title was deleted successfully";
            return "There was an error deleting the job title";
        }

        public ActionResult IndexSubunit(int? page)
        {
            //    int pageIndex = 1;
            //    int pageSize = 15;

            //    pageIndex = page.HasValue ? page.Value : 1;
            //    ViewBag.message = TempData["message"];
            ViewBag.message = TempData["message"];
            ViewBag.alert_type = TempData["alert_type"];
            ViewBag.requestData = _settingsService.GetAllSubunit();
            //return View(_settingsService.GetAllSubunit().OrderBy(e => e.id).ToPagedList(pageIndex, pageSize));
            return View();
        }

        public ActionResult CreateSubunit(admin_hrm_lkup_subunit subunit)
        {
            if (ModelState.IsValid)
            {
                _settingsService.AddSubunit(subunit);

                TempData["AlertType"] = "alert-success";
                TempData["AlertMessage"] = "Successfully add new subunit";

            }
            else
            {
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = "Encountered error";
            }

            return RedirectToAction("IndexSubunit");
        }

        [HttpGet]
        public string UpdateSubunit(int subunitId)
        {
            var fetchedsubunit = _settingsService.GetAllSubunit().Select(m => new
            {
                id = m.id,
                name = m.name,
                level = m.level,
                lft = m.lft,
                rgt = m.rgt
            }).FirstOrDefault(e => e.id == subunitId);

            //if (subunitId == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            //admin_hrm_lkup_subunit fetchedsubunit = _settingsService.GetAllSubunit().FirstOrDefault(e => e.id == subunitId);

            if (fetchedsubunit == null)
            {
                return "not found";
            }

            ViewBag.subunit = fetchedsubunit;

            return JsonConvert.SerializeObject(fetchedsubunit);

        }

        [HttpPost]
        public ActionResult UpdateSubunit(admin_hrm_lkup_subunit subunit)
        {
            try
            {
                _settingsService.UpdateSubunit(subunit);
                ViewBag.message = "The sub-unit was successfully updated";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                ViewBag.message = "There was an error updating sub-unit";
            }
            ViewBag.subunit = _settingsService.GetSubunit(subunit.id);
            return RedirectToAction("IndexSubunit");
        }

        public string DeleteSubunit(int id)
        {
            int deleted = _settingsService.DeleteSubunit(id);
            if (deleted > 0)
                return "The sub-unit was deleted successfully";
            return "There was an error deleting the sub-unit";
        }



        public ActionResult IndexEmploymentStatus(int? page)
        {
            //int pageIndex = 1;
            //int pageSize = 15;

            //pageIndex = page.HasValue ? page.Value : 1;
            //ViewBag.message = TempData["message"];
            //return View(_settingsService.GetAllEmploymentStatus().OrderBy(e => e.id).ToPagedList(pageIndex, pageSize));
            ViewBag.message = TempData["message"];
            ViewBag.alert_type = TempData["alert_type"];
            ViewBag.requestData = _settingsService.GetAllEmploymentStatus();
            return View();
        }

        public ActionResult CreateEmploymentStatus(admin_hrm_lkup_employment_status employmentStatus)
        {
            if (ModelState.IsValid)
            {
                _settingsService.AddEmploymentStatus(employmentStatus);

                TempData["AlertType"] = "alert-success";
                TempData["AlertMessage"] = "Successfully add new Job";

            }
            else
            {
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = "Encountered error";
            }

            return RedirectToAction("IndexEmploymentStatus");
        }

        [HttpGet]
        public string UpdateEmploymentStatus(int employmentStatusId)
        {
           // if (employmentStatusId == null)
           // {
           //     return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
           // }

           //admin_hrm_lkup_employment_status fetchedemploymentStatus = _settingsService.GetAllEmploymentStatus().FirstOrDefault(e => e.id == employmentStatusId);

           // if (fetchedemploymentStatus == null)
           // {
           //     return HttpNotFound();
           // }

           // ViewBag.employmentStatus = fetchedemploymentStatus;

           // return View();



            var fetchedemploymentStatus = _settingsService.GetAllEmploymentStatus().Select(m => new
            {
                id = m.id,
                title = m.name,
               
            }).FirstOrDefault(e => e.id == employmentStatusId);
            if (fetchedemploymentStatus == null)
            {
                return "not found";
            }

            return JsonConvert.SerializeObject(fetchedemploymentStatus);

        }

        [HttpPost]
        public ActionResult UpdateEmploymentStatus(admin_hrm_lkup_employment_status employmentStatus)
        {
            try
            {
                _settingsService.UpdateEmploymentStatus(employmentStatus);
                ViewBag.message = "The employment Status was successfully updated";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                ViewBag.message = "There was an error updating employment Status";
            }
            ViewBag.employmentStatus = _settingsService.GetEmploymentStatus(employmentStatus.id);
            return RedirectToAction("IndexEmploymentStatus");
        }

        public string DeleteEmploymentStatus(int id)
        {
            int deleted = _settingsService.DeleteEmploymentStatus(id);
            if (deleted > 0)
                return "The employment Status was deleted successfully";
            return "There was an error deleting the employment Status";
        }



        public ActionResult IndexLocation(int? page)
        {
            //int pageIndex = 1;
            //int pageSize = 15;

            //pageIndex = page.HasValue ? page.Value : 1;
            //ViewBag.message = TempData["message"];
            //return View(_settingsService.GetAllLocation().OrderBy(e => e.id).ToPagedList(pageIndex, pageSize));

            ViewBag.message = TempData["message"];
            ViewBag.alert_type = TempData["alert_type"];
            ViewBag.requestData = _settingsService.GetAllLocation();
            return View();
        }

        public ActionResult CreateLocation(admin_hrm_location location)
        {
            if (ModelState.IsValid)
            {
                _settingsService.AddLocation(location);

                TempData["AlertType"] = "alert-success";
                TempData["AlertMessage"] = "Successfully add new location";

            }
            else
            {
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = "Encountered error";
            }

            return RedirectToAction("IndexLocation");
        }

        [HttpGet]
        public string UpdateLocation(int locationId)
        {
            //if (locationId == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            //admin_hrm_location fetchedLocation = _settingsService.GetAllLocation().FirstOrDefault(e => e.id == locationId);

            //if (fetchedLocation == null)
            //{
            //    return HttpNotFound();
            //}

            //ViewBag.location = fetchedLocation;

            //return View();

            var fetchedLocation = _settingsService.GetAllLocation().Select(m => new
            {
                id = m.id,
                name = m.name,
                phone = m.phone,
                province = m.province,
                timezone = m.time_zone,
                zipcode = m.zip_code,
                 note = m.notes
            }).FirstOrDefault(e => e.id == locationId);
            if (fetchedLocation == null)
            {
                return "not found";
            }

            return JsonConvert.SerializeObject(fetchedLocation);
        }

        [HttpPost]
        public ActionResult UpdateLocation(admin_hrm_location location)
        {
            try
            {
                _settingsService.UpdateLocation(location);
                ViewBag.message = "The location was successfully updated";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                ViewBag.message = "There was an error updating location";
            }
            ViewBag.location = _settingsService.GetLocation(location.id);
            return RedirectToAction("IndexLocation");
        }

        public string DeleteLocation(int id)
        {
            int deleted = _settingsService.DeleteLocation(id);
            if (deleted > 0)
                return "The location was deleted successfully";
            return "There was an error deleting the location";
        }



        public ActionResult IndexAppSettings(int? page)
        {
            //int pageIndex = 1;
            //int pageSize = 15;

            //pageIndex = page.HasValue ? page.Value : 1;
            //ViewBag.message = TempData["message"];
            //return View(_settingsService.GetAllAppSettings().OrderBy(e => e.id).ToPagedList(pageIndex, pageSize));
            ViewBag.message = TempData["message"];
            ViewBag.alert_type = TempData["alert_type"];
            ViewBag.requestData = _settingsService.GetAllAppSettings();
            return View();
        }

        public ActionResult CreateAppSettings(ws_app_settings appsettings)
        {
            if (ModelState.IsValid)
            {
                _settingsService.AddAppSettings(appsettings);

                TempData["AlertType"] = "alert-success";
                TempData["AlertMessage"] = "Successfully add new app settings";

            }
            else
            {
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = "Encountered error";
            }

            return RedirectToAction("IndexAppSettings");
        }

        [HttpGet]
        public string UpdateAppSettings(int appsettingsId)
        {
        //    if (appsettingsId == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    ws_app_settings fetchedappsettings = _settingsService.GetAllAppSettings().FirstOrDefault(e => e.id == appsettingsId);

        //    if (fetchedappsettings == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    ViewBag.appsettings = fetchedappsettings;

        //    return View();

         var fetchedappsettings = _settingsService.GetAllAppSettings().Select(m => new
         {
             id = m.id,
             title = m.key,
             description = m.value,
            
         }).FirstOrDefault(e => e.id == appsettingsId);
            if (fetchedappsettings == null)
            {
                return "not found";
            }

            return JsonConvert.SerializeObject(fetchedappsettings);
        }

        [HttpPost]
        public ActionResult UpdateAppSettings(ws_app_settings appsettings)
        {
            try
            {
                _settingsService.UpdateAppSettings(appsettings);
                ViewBag.message = "The app settings was successfully updated";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                ViewBag.message = "There was an error updating app settings";
            }
            ViewBag.appsettings = _settingsService.GetAppSettings(appsettings.id);
            return RedirectToAction("IndexAppSettings");
        }

        public string DeleteAppSettings(int id)
        {
            int deleted = _settingsService.DeleteAppSettings(id);
            if (deleted > 0)
                return "The app settings was deleted successfully";
            return "There was an error deleting the app settings";
        }



        public ActionResult IndexLookup(int? page, ws_lookup lookup)
        {
        //    int pageIndex = 1;
        //    int pageSize = 15;

        //    pageIndex = page.HasValue ? page.Value : 1;
        //    ViewBag.message = TempData["message"];
        //    return View(_settingsService.GetAllLookup().OrderBy(e => e.id).ToPagedList(pageIndex, pageSize));

          ViewBag.message = TempData["message"];
            ViewBag.alert_type = TempData["alert_type"];
            ViewBag.requestData = _settingsService.GetAllLookup();
            return View();
    }

        public ActionResult CreateLookup(ws_lookup lookup)
        {    
            if (ModelState.IsValid)
            {
                _settingsService.AddLookUp(lookup);

                TempData["AlertType"] = "alert-success";
                TempData["AlertMessage"] = "Successfully add new lookup";

            }
            else
            {
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = "Encountered error";
            }

            return RedirectToAction("IndexLookup");
        }

        [HttpGet]
        public string UpdateLookup(int lookupId)
        {
            // if (lookupId == null)
            // {
            //     return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            // }

            //ws_lookup fetchedlookup = _settingsService.GetAllLookup().FirstOrDefault(e => e.id == lookupId);

            // if (fetchedlookup == null)
            // {
            //     return HttpNotFound();
            // }

            // ViewBag.lookup = fetchedlookup;

            // return View();

            var fetchedlookup = _settingsService.GetAllLookup().Select(m => new
            {
                id = m.id,
                name = m.lookup_name,
               orderd = m.orderd,
                deleted = m.deleted,
                category = m.category
            }).FirstOrDefault(e => e.id == lookupId);
            if (fetchedlookup == null)
            {
                return "not found";
            }

            return JsonConvert.SerializeObject(fetchedlookup);
        }

        [HttpPost]
        public ActionResult UpdateLookup(ws_lookup lookup)
        {
            try
            {
                _settingsService.UpdateLookUp(lookup);
                ViewBag.message = "The lookup was successfully updated";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                ViewBag.message = "There was an error updating lookup";
            }
            ViewBag.lookup = _settingsService.GetLookup(lookup.id);
            return RedirectToAction("IndexLookup");
        }

        public string DeleteJobLookup(int id)
        {
            int deleted = _settingsService.DeleteLookUp(id);
            if (deleted > 0)
                return "The lookup was deleted successfully";
            return "There was an error deleting lookup";
        }


        public ActionResult IndexSample ()
        {

            return View();
        }

    }
}