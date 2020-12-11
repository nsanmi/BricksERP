using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnePortal.Models.ViewModels
{
    public class Employee
    {
        public Employee(admin_hrm_employee employee)
        {
            emp_number = employee.emp_number;
            employee_id = employee.employee_id;
            emp_firstname = employee.emp_firstname;
            emp_lastname = employee.emp_lastname;

            if (employee.emp_birthday.HasValue)
                emp_birthday = employee.emp_birthday.Value.ToString("yyyy-MM-dd");


            emp_marital_status = employee.emp_marital_status;
            emp_gender = employee.emp_gender;
            emp_hm_telephone = employee.emp_hm_telephone;
            emp_work_email = employee.emp_work_email;
            emp_address = employee.emp_street1 + " " + employee.emp_street2 + " " +employee.city_code+" "+employee.coun_code;
            emp_status = employee.admin_hrm_lkup_employment_status.name;
            emp_job_title = employee.admin_hrm_emp_job_record.admin_hrm_lkup_job_title.job_title;

            if (employee.admin_hrm_emp_locations.LastOrDefault() != null)
            {
                emp_location = employee.admin_hrm_emp_locations.LastOrDefault().admin_hrm_location.name;
            }
           
            if (employee.admin_hrm_emp_reportto1.Any())
            {
                emp_supervisors = new List<Person>();
                foreach (var sup in employee.admin_hrm_emp_reportto1)
                {
                    emp_supervisors.Add(new Person {person_name = string.Format("{0} {1}", sup.admin_hrm_employee.emp_lastname, sup.admin_hrm_employee.emp_firstname) });
                }
            }

            if (employee.admin_hrm_emp_reportto.Any())
            {
                emp_subs = new List<Person>();
                foreach (var sub in employee.admin_hrm_emp_reportto)
                {
                    admin_hrm_employee emp = sub.admin_hrm_employee1;
                    var person = new Person { person_name = string.Format("{0} {1}", emp.emp_lastname, emp.emp_firstname), person_jobtitle = emp.admin_hrm_emp_job_record.admin_hrm_lkup_job_title.job_title };
                    if (emp.admin_hrm_emp_locations.Any())
                    {
                        person.person_location = emp.admin_hrm_emp_locations.LastOrDefault().admin_hrm_location.name;
                    }
                    emp_subs.Add(person);
                }
            }

            if (employee.joined_date.HasValue)
            {
                joined_date =CalculateDuration(employee.joined_date.Value) + " " + employee.joined_date.Value.ToString("yyyy-MM-dd");
            }
            emp_about = employee.emp_about;
        }

        public string emp_about { get; set; }
        public string emp_location { get; set; }
        public string emp_address { get; set; }
        public int emp_number { get; set; }
        public string employee_id { get; set; }
        public string emp_lastname { get; set; }
        public string emp_firstname { get; set; }
        public string emp_middle_name { get; set; }
        public string emp_nick_name { get; set; }
        public string ethnic_race_code { get; set; }
        public string emp_birthday { get; set; }
        public Nullable<int> nation_code { get; set; }
        public Nullable<byte> emp_gender { get; set; }
        public string emp_marital_status { get; set; }
        public string emp_ssn_num { get; set; }
        public string emp_sin_num { get; set; }
        public string emp_other_id { get; set; }
        public string emp_dri_lice_num { get; set; }
        public Nullable<System.DateTime> emp_dri_lice_exp_date { get; set; }
        public string emp_military_service { get; set; }
        public string emp_status { get; set; }
        public string emp_job_title { get; set; }
        public Nullable<int> eeo_cat_code { get; set; }
        public Nullable<int> work_station { get; set; }
        public string emp_street1 { get; set; }
        public string emp_street2 { get; set; }
        public string city_code { get; set; }
        public string coun_code { get; set; }
        public string provin_code { get; set; }
        public string emp_zipcode { get; set; }
        public string emp_hm_telephone { get; set; }
        public string emp_mobile { get; set; }
        public string emp_work_telephone { get; set; }
        public string emp_work_email { get; set; }
        public string sal_grd_code { get; set; }
        public string joined_date { get; set; }
        public string emp_oth_email { get; set; }
        public Nullable<int> termination_id { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
        public Nullable<System.DateTime> added_date { get; set; }

        public List<Person> emp_supervisors { set; get; }
        public List<Person> emp_subs { set; get; }


        static string CalculateDuration(DateTime join_date)
        {
            DateTime Now = DateTime.Now;
            int Years = new DateTime(DateTime.Now.Subtract(join_date).Ticks).Year - 1;
            DateTime PastYearDate = join_date.AddYears(Years);
            int Months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (PastYearDate.AddMonths(i) == Now)
                {
                    Months = i;
                    break;
                }
                else if (PastYearDate.AddMonths(i) >= Now)
                {
                    Months = i - 1;
                    break;
                }
            }
            int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;
            int Hours = Now.Subtract(PastYearDate).Hours;
            int Minutes = Now.Subtract(PastYearDate).Minutes;
            int Seconds = Now.Subtract(PastYearDate).Seconds;
            return String.Format(" <strong style='color:orange'>{0} Year(s) {1} Month(s)</strong>",
            Years, Months);
        }
    }

    public class Person
    {
        public string person_name { set; get; }
        public string person_location { set; get; }
        public string person_jobtitle { set; get; }
    }

    public class Organogram
    {
        public string name { get; set; }
        public string imageUrl { get; set; }
        public string area { get; set; }
        public string profileUrl { get; set; }
        public string office { get; set; }
        public string tags { get; set; }
        public bool isLoggedUser { get; set; }
        public Unit unit { get; set; }
        public string positionName { get; set; }
        public List<Organogram> children { get; set; }
        public int empNumber { get; set; }
    }

    public class Unit
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class LeaveR
    {
        public string EmployeeName { get; set; }
        public int EmpNumber { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int NumberOfDaysRejected { get; set; }
        public int NumberOfDaysTaken { get; set; }
        public int NumberOfDaysScheduled { get; set; }
        public int NumberOfDaysCanceled { get; set; }
        public int NumberOfDaysPendingApproval { get; set; }
        public string Status { get; set; }
        public bool OnLeave { get; set; }

        public string Taken { get; set; }
        public string EScheduled { get; set; }
        public int Pending { get; set; }
        public string Approved { get; set; }
        public bool Rejected { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }

    }

    public class TimeSheetO
    {
        public string EmployeeName { get; set; }
        public int EmpNumber { get; set; }
        public string Designation { get; set; }
        public List<TimesheetProjects> Projects { get; set; }
    }

    public class TimesheetProjects
    {
        public int ProjectId { get; set; }
        public int Duration { get; set; }
    }
    public class TimesheetProjectNames
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}