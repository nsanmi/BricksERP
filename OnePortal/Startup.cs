using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using OnePortal.Models;
using Owin;
using System;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(OnePortal.Startup))]
namespace OnePortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            ConfigureAuth(app);
            createRolesandUsers();

           
        }

        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool   
                var role = new IdentityRole();
                role.Name = "admin";
                roleManager.Create(role);

            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("sup"))
            {
                var role = new IdentityRole();
                role.Name = "sup";
                roleManager.Create(role);

            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("sub"))
            {
                var role = new IdentityRole();
                role.Name = "sub";
                roleManager.Create(role);

            }
        }


       


    }
}
