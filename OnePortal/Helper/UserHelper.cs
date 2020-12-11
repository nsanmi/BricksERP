using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using OnePortal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using HRM.DAL.Util;

namespace OnePortal.Helper
{
    public static class UserHelper
    {
        public static async Task ImpersonateUserAsync(string userName)
        {
            ApplicationDbContext ctx = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));

            var context = System.Web.HttpContext.Current;

            var originalUsername = context.User.Identity.Name;

            var impersonatedUser = await userManager.FindByNameAsync(userName);

            var impersonatedIdentity = await userManager.CreateIdentityAsync(impersonatedUser, DefaultAuthenticationTypes.ApplicationCookie);
            impersonatedIdentity.AddClaim(new System.Security.Claims.Claim("UserImpersonation", "true"));
            impersonatedIdentity.AddClaim(new System.Security.Claims.Claim("OriginalUsername", originalUsername));

            var authenticationManager = context.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, impersonatedIdentity);
        }

        public static bool IsImpersonating(this IPrincipal principal)
        {
            if (principal == null)
            {
                return false;
            }

            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                return false;
            }


            return claimsPrincipal.HasClaim("UserImpersonation", "true");
        }

        public static String GetOriginalUsername(this IPrincipal principal)
        {
            if (principal == null)
            {
                return String.Empty;
            }

            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                return String.Empty;
            }

            if (!claimsPrincipal.IsImpersonating())
            {
                return String.Empty;
            }

            var originalUsernameClaim = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == "OriginalUsername");

            if (originalUsernameClaim == null)
            {
                return String.Empty;
            }

            return originalUsernameClaim.Value;
        }

        public static async Task RevertImpersonationAsync()
        {
            ApplicationDbContext ctx = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));

            var context = HttpContext.Current;

            if (!HttpContext.Current.User.IsImpersonating())
            {
                throw new Exception("Unable to remove impersonation because there is no impersonation");
            }


            var originalUsername = HttpContext.Current.User.GetOriginalUsername();

            var originalUser = await userManager.FindByNameAsync(originalUsername);

            var impersonatedIdentity = await userManager.CreateIdentityAsync(originalUser, DefaultAuthenticationTypes.ApplicationCookie);
            var authenticationManager = context.GetOwinContext().Authentication;

            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, impersonatedIdentity);
        }

        public static string GetEmployeeNameById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return "";
            return OptionUtil.GetEmployeeNameById(userId);
        }

        public static string GetEmployeeEmail(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return "";
            return OptionUtil.GetEmployeeEmailById(userId);
        }
    }
}