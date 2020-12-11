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

using OnePortal.Models.ViewModels;
using System.IO;

namespace OnePortal.Controllers
{

    [Authorize]
    public class SuggestionController : Controller
    {

        ISuggestionService _suggestionService;
        IEmployeeService _employeeService;


        public SuggestionController(ISuggestionService suggestionService, IEmployeeService employeeService)
        {

            _suggestionService = suggestionService;
            _employeeService = employeeService;
        }

        // GET: Suggestion
        public ActionResult Index(int? page, string search = null)
        {


            int pageIndex = 1;
            int pageSize = 15;
            pageIndex = page.HasValue ? page.Value : 1;
            var userId = User.Identity.GetUserId();


            if (search != null)
            {
                //_complainService.GetAllComplain().Where(e => e.UserId.Contains(search) || e.Type.Contains(search) || e.Priority.Contains(search) || e.AspNetUser.UserName.Contains(search) || e.Resolved.Contains(search)).OrderBy(e => e.UpdateDate).ToPagedList(pageIndex, pageSize);
                ViewBag.search = search;
                return View(_suggestionService.GetAllSuggestion().Where(m => m.user_id == userId && m.deleted == 0).Where(e => e.title.Contains(search) || e.comment.Contains(search)).OrderByDescending(e => e.date).ToPagedList(pageIndex, pageSize));
            }
            return View(_suggestionService.GetAllSuggestion().Where(m => m.deleted == 0).OrderByDescending(e => e.date).ToPagedList(pageIndex, pageSize));

        }

        public string Delete(int id)
        {
            int deleted = _suggestionService.DeleteSuggestion(id);
            if (deleted > 0)
                return "The suggestion was deleted successfully";
            return "There was an error deleting the suggestion";
        }


        public ActionResult Make()
        {
            //set the return message 
            ViewBag.message = TempData["message"];
            return View();
        }
        
        [HttpPost]
        public ActionResult Make(admin_hrm_suggestion suggest)
        {
            var user_id = User.Identity.GetUserId();

            //suggest.emp_number =89;
            //suggest


            suggest.date = DateTime.Now;
            suggest.deleted = 0;
            admin_hrm_suggestion_files suggestionFile = new admin_hrm_suggestion_files();
            suggestionFile = new admin_hrm_suggestion_files { filename = "hello.pdf" };


            suggest.admin_hrm_suggestion_files.Add(suggestionFile);

            try
            {
                //var suggestion = new admin_hrm_suggestion
                //{
                //    date = DateTime.Now,
                //    deleted = 0,
                //    user_id = user_id,
                //    title = suggest.title,
                //    comment = suggest.comment
                //};



                suggest.suggestion_id = _suggestionService.AddSuggestion(suggest);

                string directory = Server.MapPath("~/Documents/Suggestion/" + suggest.title) + "\\";
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
                    _suggestionService.AddSuggestionFiles(new admin_hrm_suggestion_files
                    {
                        suggestion_id = suggest.suggestion_id,
                        filename = "Documents/Suggestion/" + suggest.title + "/" + uniqueName + "_" + names[names.Length - 1]
                    });
                }

                //send mail to compalin admin

                //Get the url of the complain
                /*string Domain = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                                (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                Domain += "/suggestion/update?suggestionId=" + suggest.suggestion_id;
                var email_body = string.Format("A suggestion has been raised on <strong>{0}</strong>.<br/>",
                    suggest.title);
                email_body += "the description is <br/>";
                email_body += string.Format("<i>{0}</i>.<br/>", suggest.comment);
                email_body += string.Format(" <a href='{0}'>Click here </a> to view", Domain);

                var notification_email = new Email
                {
                    body = email_body,
                    subject = "Suggestion - Action required"
                };
                notification_email.IsHtml = true;
                notification_email.body = email_body;
                notification_email.to = _suggestionService.GetSuggestionAdmin();
                NotificationUtil.SendEmailComplain(notification_email, UserHelper.GetEmployeeEmail(user_id));*/

                TempData["message"] = "Your suggestion has been sent to management. Thank You";
            }
            catch (Exception e)
            {
                TempData["message"] = "There was an error sending the suggestion, Please try again";
                Utils.LogError(e);
            }

            //set the return message 
            ViewBag.message = TempData["message"];
            return View();

        }

        [HttpGet]
        public ActionResult Update(int suggestionId)
        {
            ViewBag.suggestion = _suggestionService.GetSuggestion(suggestionId);

            return View();
        }

        //[HttpPost]
        //public ActionResult Update(admin_hrm_suggestion suggestion)
        //{
        //    try
        //    {
        //        _suggestionService.UpdateSuggestion(suggestion);
        //        ViewBag.message = "The suggestion was successfully updated";
        //    }
        //    catch (Exception e)
        //    {
        //        Utils.LogError(e);
        //        ViewBag.message = "There was an error updating the complain";
        //    }
        //    ViewBag.suggestion = _suggestionService.GetSuggestion(suggestion.suggestion_id);
        //    return View();
        //}


        public string Deletee(int id)
        {

            int deleted = _suggestionService.DeleteSuggestion(id);
            if (deleted > 0)
                return "The suggestion was deleted successfully";
            return "There was an error deleting the suggestion";

        }
    }
}