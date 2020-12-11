using HRM.DAL.Models;
using HRM.DAL.Util;
using OnePortal.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnePortal.Controllers
{
    public class OptionApiController : ApiController
    {
        
        public Dictionary<int,string> GetActivities()
        {
            var activities = OptionUtil.GetActivities();
            var a_list = new Dictionary<int, string>();
            foreach(var activity in activities)
            {
                a_list.Add(activity.id, activity.activity_name);
            }
            return a_list;
        }

        public Dictionary<int, string> GetProjects()
        {
            var projects= OptionUtil.GetProjects();
            var dict_projects = new Dictionary<int, string>();

            foreach(var project in projects)
            {
                dict_projects.Add(project.id, project.name);
            }

            return dict_projects;
        }

    }
}
