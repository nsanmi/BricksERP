using HRM.DAL.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WorkFlow.DAL.Data;

namespace OnePortal.Helper
{
    public static class TransactionHelper
    {
        public static string GetResponsiblePerson(XElement document, bpm_workflow workflow)
        {
            var result = new List<string>();
            var employeeService = new EmployeeService();

            var employee = employeeService.GetEmployee(workflow.created_by);
            var current_step = document.Element("next_step").Value;
            var step_value = Convert.ToInt32(current_step);
            var step_element = document.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == current_step);
            //get the current step
            if (step_value > 1)
            {
                var position = step_element.Element("position");
                switch (position.Attribute("pos_type").Value)
                {
                    case "sup":
                        //get the supervisor of the person that applied for the request
                        var requestor_no = workflow.admin_hrm_employee.emp_number;
                        var supervisors = employee.admin_hrm_emp_reportto1.Select(e => e.erep_sup_emp_number).ToArray();
                        foreach (var sup in supervisors)
                        {
                            var empl = employeeService.GetEmployee(sup);
                            result.Add(String.Format("{0} {1}", empl.emp_lastname, empl.emp_firstname));
                        }

                       
                        break;
                    case "jti":
                        var job_title_id = Convert.ToInt32(position.Value);

                        var employees = employeeService.GetEmployees().Where(e => e.admin_hrm_emp_job_record.job_title_id == job_title_id);
                        foreach (var empl in employees)
                        {
                           result.Add(String.Format("{0} {1},", empl.emp_lastname, empl.emp_firstname));
                        }

                        break;
                    case "dir":
                        //get the subunit of the requestor
                        var directorate = employeeService.GetDirectorate(workflow.created_by);
                        var emps = employeeService.GetEmployees().Where(e => e.admin_hrm_emp_job_record.job_title_id == directorate.head);

                        foreach (var empl in emps)
                        {
                            result.Add(String.Format("{0} {1}", empl.emp_lastname, empl.emp_firstname));
                        }


                        break;
                    case "emp":
                        //get the subunit of the requestor
                        result.Add(String.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname));

                        break;
                    default:
                        break;
                }


            }

            return string.Join(",",result);
        }
    }
}