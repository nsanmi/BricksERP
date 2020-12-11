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


namespace OnePortal.Controllers
{
    [Authorize]
    public class ComplainController : Controller
    {
        IComplainService _complainService;
        ILookupService _lookupService;
        IEmployeeService _employeeService;
        public ComplainController(IComplainService complainService, ILookupService lookupService, IEmployeeService employeeService)
        {
           
           _complainService = complainService;
           _lookupService = lookupService;
            _employeeService = employeeService;
        }

        // MAKE: Complain
        [HttpPost]
        public ActionResult Make(Complain comp)
        {
            var user_id = User.Identity.GetUserId();
            try
            {
                var complaint = new ws_complain
                {
                    Priority = comp.Priority,
                    Type = comp.Type,
                    UserId = user_id,
                    Comment = comp.Comment,
                    CreateDate = DateTime.Now,
                    Deleted = 0,
                    Resolved = "No"
                };

                complaint.Id = _complainService.AddComplain(complaint);

                string directory = Server.MapPath("~/Documents/Complain/" + comp.Type) + "\\";
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
                    _complainService.AddComplainFiles(new ws_complain_files
                    {
                        ComplainId = complaint.Id,
                        Filename = "Documents/Complain/" + comp.Type + "/" + uniqueName + "_" + names[names.Length - 1]
                    });
                }
                //send mail to compalin admin

                //Get the url of the complain
                string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                                (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                Domain += "/complain/update?complainId=" + complaint.Id;
                var email_body = string.Format("A complain of <strong>{0}</strong> priority has been raised on <strong>{1}</strong>.<br/>", complaint.Priority,
                    complaint.Type);
                email_body += "the description is <br/>";
                email_body += string.Format("<i>{0}</i>.<br/>", complaint.Comment);
                email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

                var notification_email = new Email
                {
                    body = email_body,
                    subject = "Complain - Action required - BricksERP"
                };
                notification_email.IsHtml = true;
                notification_email.body = email_body;
#if DEBUG
                //notification_email.to = new List<string>{ UserHelper.GetEmployeeEmail(user_id) };

                notification_email.to = _complainService.GetComplainAdmin();
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

        [HttpGet]
        public ActionResult Update(int complainId)
        {
            ViewBag.complain = _complainService.GetComplain(complainId); 

            return View();
        }

        [HttpPost]
        public ActionResult Update(ws_complain complain)
        {
            try
            {
                _complainService.UpdateComplain(complain);
                ViewBag.message = "The complain was successfully updated";
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                ViewBag.message = "There was an error updating the complain";
            }
            ViewBag.complain = _complainService.GetComplain(complain.Id);
            return View();
        }

        public string Delete(int id)
        {
            int deleted = _complainService.DeleteComplain(id);
            if(deleted > 0)
                return "The comment was deleted successfully";
            return "There was an error deleting the comment";
        }

        public string Resolve(int id)
        {
            var complaint = new ws_complain();
            
            var make = complaint;
            int resolve = _complainService.ResolvedComplain(id, User.Identity.GetUserId());
            var email = _complainService.GetComplain(id).AspNetUser.admin_hrm_employee.First().emp_work_email;


            var email_body = string.Format("The complain you reported on BricksERP has been resolved.<br/>", complaint.Type);
            //email_body += "The description is <br/>";
            //email_body += string.Format("<i>{0}</i>.<br/>", complaint.Comment);
           

            var notification_email = new Email
            {
                body = email_body,
                subject = "Complain - Complain resolved - BricksERP"
            };
            notification_email.IsHtml = true;
            notification_email.body = email_body;


            ////Get the complainer's email from db with the id
            ////copy the resolver
            ////then send the mail
            //foreach(var a in _complainService.ResolvedNotification())
            //{

            //}
            List<string> email_recipient = new List<string> { email};
            notification_email.to = email_recipient;
            NotificationUtil.SendEmailTest(notification_email);
             //TempData["message"] =
            //    "Your complain has been resolved and email sent to the appropraite persons. Thank You";
               if (resolve > 0)
                return "The complain was resolved successfully";
            return "There was an error confirming resolved";
          }

        public ActionResult Index(int? page, string search = null)
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
                return View(_complainService.GetAllComplain().Where(m => m.AspNetUser.Id == userId && m.Deleted == 0).Where(e => e.Type.Contains(search) || e.Priority.Contains(search)).OrderBy(e => e.Resolved).ToPagedList(pageIndex, pageSize));
            }
            return View(_complainService.GetAllComplain().Where(m=>m.AspNetUser.Id == userId && m.Deleted == 0).OrderBy(e => e.Resolved).ToPagedList(pageIndex, pageSize));
        }

        public ActionResult AdminIndex(int? page, string search = null)
        {
            int pageIndex = 1;
            int pageSize = 15;
            pageIndex = page.HasValue ? page.Value : 1;
            if (search != null)
            {
                ViewBag.search = search;
                return View(_complainService.GetAllComplain().Where(m => m.Deleted == 0).Where(e => e.Type.Contains(search) || e.Priority.Contains(search)).OrderBy(e => e.Resolved).ToPagedList(pageIndex, pageSize));
            }
            return View(_complainService.GetAllComplain().Where(e => e.Deleted == 0).OrderByDescending(e => e.CreateDate).ToPagedList(pageIndex, pageSize));
        }

        public ActionResult ManageComplain()
        {
            return View();
        }

        public ActionResult MakeComplain()
        {
            return View();
        }

        public ActionResult MakeCom()
        {
            return View();
        }

    }
}