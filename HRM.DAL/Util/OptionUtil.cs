using HRM.DAL.Models;
using HRM.DAL.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HRM.DAL.Util
{
    public static class OptionUtil
    {
        readonly static oneportalEntities _context = new oneportalEntities();

        public static IQueryable<admin_hrm_activity> GetActivities()
        {
            return _context.admin_hrm_activity.AsQueryable();
        }

        public static IQueryable<pm_project> GetProjects()
        {
            return _context.pm_project.AsQueryable();
        }

        public static IQueryable<product_category> GetCategory()
        {
            return _context.product_category.AsQueryable();

        }


        //public static IQueryable<product> GetCategory()
        //{
        //    return _context.products.AsQueryable();

        //}

        public static string GetTimeshetItemTitle(int project_id, int activity_id)
        {
           
            var project = _context.pm_project.FirstOrDefault(e => e.id == project_id).name;
            var activity = _context.admin_hrm_activity.FirstOrDefault(e => e.id == activity_id).activity_name;

            return project + " " + activity;
        }

        public static IQueryable<admin_hrm_leave_type> GetLeaveTypes()
        {
            return _context.admin_hrm_leave_type.Where(e => e.deleted == 0);
        }


        public static admin_hrm_employee GetEmployee(string user_id)
        {
            return _context.admin_hrm_employee.FirstOrDefault(e => e.user_id == user_id);
        }

        public static List<string> GetPermissions(string user_id)
        {
            var permissions = new List<string>();
             var roles = _context.AspNetUsers.FirstOrDefault(e => e.Id == user_id).AspNetRoles.ToList();
            foreach(var role in roles)
            {
                permissions.AddRange(role.AspNetRolePermissions.Select(e => e.AspNetPermission.Permission).ToList());
            }

            return permissions;
        }

        public static string GetLeaveStatus(List<admin_hrm_leave> leaves)
        {
            var status = leaves.Select(e => e.status).Distinct();
            var list = new List<string>();
            foreach(var st in status)
            {
                var stat = _context.admin_hrm_leave_status.FirstOrDefault(e => e.status == st);
                if (stat != null)
                {
                    list.Add(stat.name + "(" + leaves.Count(e => e.status == st) + ")");
                }

            }

            return String.Join(",",list);
        }

        public static string GetLeaveStatusName(int status_id)
        {
            var status = _context.admin_hrm_leave_status.FirstOrDefault(e => e.status == status_id);
            return status != null ? status.name : "";
        }

        public static string GetLeaveStatus(int status)
        {
            return _context.admin_hrm_leave_status.FirstOrDefault(e => e.status == status).name;
        }
       

        public static List<TimesheetCategory> GetCategories()
        {
            var lnk_categories = _context.admin_hrm_timesheet_lk_project_activity;
            var categories = new List<TimesheetCategory>();
            foreach(var lk_category in lnk_categories)
            {
                categories.Add(new TimesheetCategory(lk_category));
            }

            return categories;
        }

        public static string HolidayOrLeave(DateTime date,int emp_number)
        {
            var result = "";
            var leave = _context.admin_hrm_leave.Where(e => e.date == date && e.emp_number == emp_number && e.status.Value==3);
            if (leave.Any()) result = "L";

            var holidays = _context.admin_hrm_holiday.Where(e => e.date == date);
            if (holidays.Any()) result = "H";
            return result;
        }

        public static bool isHoliday(DateTime date)
        {
            return _context.admin_hrm_holiday.Where(e => e.date == date).Any();
        }
        

        public static IQueryable<pm_funder> GetFunders()
        {
            return _context.pm_funder.AsQueryable();
        }

        public static IQueryable<ws_lookup> GetLookupOptions(string category)
        {
            return _context.ws_lookup.Where(e => e.category == category);
        }

        public static IQueryable<Guest> GetGuestsOptions()
        {
            return _context.Guests.Where(e => e.Active == 1);
        }

        public static IQueryable<product_category> GetCategoryOptions()
        {
            return _context.product_category.Where(e => e.Deleted == 0);
        }

        public static IQueryable<pricing> GetPricingOptions()
        {
            return _context.pricings.Where(e => e.Deleted == 0);
        }

        public static IQueryable<admin_hrm_employee> GetEmpOptions()
        {
            return _context.admin_hrm_employee.Where(e => e.deleted == 0);
        }

        //public static IQueryable<pricing> GetPricingOptions()
        //{
        //    return _context.pricings.AsQueryable();
        //}

        //public IQueryable<Guest> GetGuests()
        //{
        //    return _context.Guests.Where(e => e.Deleted == 0).AsQueryable();
        //}


        public static IQueryable<admin_hrm_lkup_job_title> GetJobTitles()
        {
            return _context.admin_hrm_lkup_job_title.Where(m => m.is_deleted == 0).AsQueryable();
        }
        

        public static IQueryable<admin_hrm_lkup_employment_status> GetEmploymentStatus()
        {
            return _context.admin_hrm_lkup_employment_status.AsQueryable();
        }

        public static IQueryable<admin_hrm_lkup_subunit> GetSubunits()
        {
            return _context.admin_hrm_lkup_subunit.AsQueryable();
        }

        public static IQueryable<admin_hrm_education> GetEducation()
        {
            return _context.admin_hrm_education.AsQueryable();
        }


        public static IQueryable<pm_budget_item> GetBudgetItems()
        {
            return _context.pm_budget_item.AsQueryable();
        }

        public static IQueryable<admin_hrm_location> GetLocations()
        {
            return _context.admin_hrm_location.AsQueryable();
        }

        public static List<MonthItem> GetMonths()
        {
            var months = new List<MonthItem>();
            months.Add(new MonthItem { Name = "January", Value = "1" });
            months.Add(new MonthItem { Name = "February", Value = "2" });
            months.Add(new MonthItem { Name = "March", Value = "3" });
            months.Add(new MonthItem { Name = "April", Value = "4" });
            months.Add(new MonthItem { Name = "May", Value = "5" });
            months.Add(new MonthItem { Name = "June", Value = "6" });
            months.Add(new MonthItem { Name = "July", Value = "7" });
            months.Add(new MonthItem { Name = "August", Value = "8" });
            months.Add(new MonthItem { Name = "September", Value = "9" });
            months.Add(new MonthItem { Name = "October", Value = "10" });
            months.Add(new MonthItem { Name = "November", Value = "11" });
            months.Add(new MonthItem { Name = "December", Value = "12" });

            return months;
        }


        public static List<CategoryItem> GetProductCategory()
        {
            var category = new List<CategoryItem>();
            category.Add(new CategoryItem { Name = "Appliance", Value = "1" });
           

            return category;
        }

        /*
         * Added by Johnbosco
         */
        /*
         * This function gets the units for procurement from the DB
         */
        public static IQueryable<ws_lookup> GetProcuremenetUnits()
        {
            return _context.ws_lookup.Where(m => m.category == "procurement_unit").AsQueryable();
        }

        public static string GetEmployeeNameById(string userId)
        {
            var employee = _context.AspNetUsers.First(m => m.Id == userId).admin_hrm_employee.First();
            return employee.emp_lastname + " " + employee.emp_firstname;
        }

        public static string GetEmployeeEmailById(string userId)
        {
            var employee = _context.AspNetUsers.First(m => m.Id == userId).admin_hrm_employee.First();
            return employee.emp_work_email;
        }
    }
}
