using HRM.DAL.Util;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnePortal.Controllers;
using OnePortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnePortal.Helper
{
    public class PermissionHelper
    {
        public static bool CanAccess(string permission,string username)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var user = UserManager.FindByName(username);
            var permissions = OptionUtil.GetPermissions(user.Id);
            if (permissions.Contains(permission))
            {
                return true;
            }
            return false;

        }
    }

    public class PermissionFilter : ActionFilterAttribute, IActionFilter
    {
        public string permission { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user_id = HttpContext.Current.User.Identity.GetUserId();
            var permissions = OptionUtil.GetPermissions(user_id);
            if (!permissions.Contains(permission))
            {

                filterContext.Result = new RedirectToRouteResult(
            new RouteValueDictionary {{ "Controller", "Home" },
                                      { "Action", "AccessDenied" } });

            }
            base.OnActionExecuting(filterContext);
        

        }
    }
}