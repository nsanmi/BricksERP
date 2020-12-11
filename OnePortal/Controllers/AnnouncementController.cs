using HRM.DAL.IService;
using HRM.DAL.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnePortal.Controllers
{
    public class AnnouncementController : ApiController
    {
        IAnnouncementService _announcementService;
        IEmployeeService _employeeService;

        public AnnouncementController(IAnnouncementService announcementService, IEmployeeService employeeService)
        {
            _announcementService = announcementService;
            _employeeService = employeeService;
        }

        public void Post(HttpRequestMessage message)
        {
            var announcement = new admin_announcement();
            var user_id = User.Identity.GetUserId();
            announcement.created_at = DateTime.Now;

            var post_text = message.Content.ReadAsStringAsync().Result;
            var obj = JObject.Parse(post_text);

            var employee = _employeeService.GetEmployeeByUserId(user_id);
            announcement.message = WebUtility.HtmlEncode(obj["message"].ToString());
            announcement.created_by = employee.emp_number;
            _announcementService.AddAnnouncement(announcement);
        }
    }
}
