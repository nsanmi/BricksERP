using HRM.DAL.IService;
using OnePortal.Models.ViewModels;
using System.Web.Http;
using HRM.DAL.Models;
using System.Linq;
using HRM.DAL.Models.ViewModel;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Hotel.Dal.Models.ViewModel;
using Hotel.Dal.IService;

namespace OnePortal.Controllers
{
    public class EmployeeApiController : ApiController
    {
        IEmployeeService _employeeService;
        IGuestBookingService _guestBookingService;

        public EmployeeApiController(IEmployeeService employeeService , IGuestBookingService guestBookingService)
        {
            _employeeService = employeeService;
            _guestBookingService = guestBookingService;
        }

        public List<SearchResult> getsearch(string id)
        {
            var result = new List<SearchResult>();
            var employees = _employeeService.GetEmployees().Where(e => e.emp_firstname.Contains(id) || e.emp_lastname.Contains(id) || e.emp_middle_name.Contains(id));
            foreach(var x in employees)
            {
                result.Add(new SearchResult { id = x.emp_number, name = string.Format("{0} {1} {2}", x.emp_lastname, x.emp_firstname, x.emp_middle_name) });
            }

            return result;
        }


        public List<SearchBookingResult> getguest(string id)
        {
            var result = new List<SearchBookingResult>();
            var guest = _guestBookingService.GetGuests().Where(e => e.First_name.Contains(id) || e.Last_name.Contains(id) || e.Email.Contains(id) || e.Phone_number.Contains(id));
            foreach (var x in guest)
            {
                result.Add(new SearchBookingResult { id = x.Id, name = string.Format("{0} {1} {2}", x.Last_name, x.First_name, x.Other_names) });
            }

            return result;
        }

        public Employee GetEmployee(int id)
        {
            return new Employee(_employeeService.GetEmployee(id));
        }

        public int UploadImg()
        {
            var user_id = User.Identity.GetUserId();
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var postedFile = httpRequest.Files[0];
                string ext = Path.GetExtension(postedFile.FileName).Substring(1);

                if (ext.ToUpper() == "PNG" || ext.ToUpper() == "JPG" || ext.ToUpper() == "GIF")
                {

                    var filePath = HttpContext.Current.Server.MapPath("~/Content/images/profile_images/" + user_id + "." + ext);
                    postedFile.SaveAs(filePath);
                }

                return 1;
            }

            return 0;
        }

        public Organogram GetOrganogram()
        {
            //var employees = ;
            //get the country director
            var countryDirector = _employeeService.GetEmployees().First(m => m.job_title_code == 2 && m.deleted == 0);
            string profilePic = "/Content/images/profile_images/" + countryDirector.user_id + ".jpg";
            if (!File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(profilePic)))
                profilePic = "/Content/images/a4.jpg"; 
            Organogram organogram = new Organogram
            {
                name = countryDirector.emp_lastname + " " + countryDirector.emp_firstname + " " +
                       countryDirector.emp_middle_name,
                imageUrl = profilePic,
                area = "Coporate",
                profileUrl = "",
                office = "",
                tags = "",
                isLoggedUser = User.Identity.GetUserId() == countryDirector.user_id,
                unit = new Unit { type = "", value = ""},
                positionName = "",
                children = new List<Organogram>(),
                empNumber = countryDirector.emp_number
            };
            var sups = countryDirector.admin_hrm_emp_reportto;
            foreach (var employee in sups)
            {
                var emp = employee.admin_hrm_employee1;
                organogram.children.Add(buildOrganogram(emp));
            }

            return organogram;
        }

        private Organogram buildOrganogram(admin_hrm_employee employee)
        {
            string profilePic = "/Content/images/profile_images/" + employee.user_id + ".jpg";
            if (!File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(profilePic)))
                profilePic = "/Content/images/a4.jpg";
            return new Organogram
            {
                name = employee.emp_lastname + " " + employee.emp_firstname + " " +
                       employee.emp_middle_name,
                imageUrl = profilePic,
                area = "Coporate",
                profileUrl = "",
                office = "",
                tags = "",
                isLoggedUser = User.Identity.GetUserId() == employee.user_id,
                unit = new Unit { type = "", value = "" },
                positionName = "",
                children = new List<Organogram>(),
                empNumber = employee.emp_number
            };
        }

        public List<Organogram> GetChildOrganograms(int empNumber)
        {
            var supervisor = _employeeService.GetEmployee(empNumber);
            List<Organogram> organogram = new List<Organogram>();
            var sups = supervisor.admin_hrm_emp_reportto;
            foreach (var employee in sups)
            {
                var emp = employee.admin_hrm_employee1;
                organogram.Add(buildOrganogram(emp));
            }

            return organogram;
        }

        public Organogram GetAllOrganogram()
        {
            //var employees = ;
            //get the country director
            var countryDirector = _employeeService.GetEmployees().First(m => m.job_title_code == 2 && m.deleted == 0);
            
            return GetAllChildOrganogram(countryDirector);
        }

        public Organogram GetAllChildOrganogram(admin_hrm_employee supervisor)
        {
            //get the supervisor profile picture
            string profilePic = "/Content/images/profile_images/" + supervisor.user_id + ".jpg";
            if (!File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(profilePic)))
                profilePic = "/Content/images/a4.jpg";
            //build the organogram
            Organogram organogram = new Organogram
            {
                name = supervisor.emp_lastname + " " + supervisor.emp_firstname,
                imageUrl = profilePic,
                area = "Coporate",
                profileUrl = "",
                office = "MGIC Office",
                tags = supervisor.emp_nick_name + " ",
                isLoggedUser = User.Identity.GetUserId() == supervisor.user_id,
                unit = new Unit { type = "", value = "" },
                positionName = supervisor.admin_hrm_emp_job_record.admin_hrm_lkup_job_title.job_title,
                children = new List<Organogram>(),
                empNumber = supervisor.emp_number
            };
            var sups = supervisor.admin_hrm_emp_reportto;
            foreach (var employee in sups)
            {
                var emp = employee.admin_hrm_employee1;
                organogram.children.Add(GetAllChildOrganogram(emp));
            }

            return organogram;
        }
    }
}
