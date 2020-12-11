using HRM.DAL.Models;
using System;
using System.Data.Entity;
using System.Linq;
using HRM.DAL.IService;

namespace HRM.DAL.Service
{
    public class EmployeeService: IEmployeeService
    {
        readonly oneportalEntities _context =new oneportalEntities();

        public int AddEmployee(admin_hrm_employee employee)
        {
            var id = 0;
            employee.added_date = DateTime.Now;
            _context.admin_hrm_employee.Add(employee);
            _context.SaveChanges();
            
            id = employee.emp_number;
            return id;
        }

        //get all the subordinates of an employee
        public IQueryable<admin_hrm_emp_reportto> GetSubordinates(int emp_number)
        {
            return _context.admin_hrm_emp_reportto.Where(e => e.erep_sup_emp_number == emp_number);
        }

        //get all the supervisors of an employee
        public IQueryable<admin_hrm_emp_reportto> GetSupervisors(int emp_number)
        {
            return _context.admin_hrm_emp_reportto.Where(e => e.erep_sub_emp_number == emp_number);
        }

        public void AddJobRecord(admin_hrm_emp_job_record job_Record)
        {
            _context.admin_hrm_emp_job_record.Add(job_Record);
            _context.SaveChanges();
        }

        public IQueryable<admin_hrm_employee> GetUsers()
        {
            return _context.admin_hrm_employee.Where(e => e.user_id != null && e.deleted==0);
        }

        public IQueryable<admin_hrm_employee> GetEmployees()
        {
            return _context.admin_hrm_employee.Where(e=>e.deleted==0).AsQueryable();
        }

        public admin_hrm_employee GetEmployee(int emp_number)
        {
            return _context.admin_hrm_employee.FirstOrDefault(e => e.emp_number == emp_number);
        }

        public admin_hrm_employee GetEmployeeByUserId(string user_id)
        {
            return _context.admin_hrm_employee.FirstOrDefault(e => e.user_id == user_id && e.deleted == 0);
        }

        public void UpdateEmployee(admin_hrm_employee employee)
        {
            var old_emp = _context.admin_hrm_employee.FirstOrDefault(e => e.emp_number == employee.emp_number);
            if(old_emp != null){
                _context.Entry(old_emp).CurrentValues.SetValues(employee);
                _context.SaveChanges();
            }
        }

        public void UpdateEmployeeRecord(admin_hrm_emp_job_record record)
        {
            var old = _context.admin_hrm_emp_job_record.FirstOrDefault(e => e.employee_number == record.employee_number);
            _context.Entry(old).CurrentValues.SetValues(record);

            _context.SaveChanges();
        }

        public void DeleteEmployee(int id)
        {
            _context.admin_hrm_employee.Remove(GetEmployee(id));
            _context.SaveChanges();
        }

        public void AddEmergencyContact(admin_hrm_emp_emergency_contacts contacts)
        {
            contacts.eec_seqno = _context.admin_hrm_emp_emergency_contacts.Count(e => e.emp_number == contacts.emp_number) + 1;
            _context.admin_hrm_emp_emergency_contacts.Add(contacts);
            _context.SaveChanges();
        }

        public void DeleteEmergencyContact(int emp_number,int seqno)
        {
            _context.admin_hrm_emp_emergency_contacts.Remove(_context.admin_hrm_emp_emergency_contacts.FirstOrDefault(e => e.emp_number == emp_number && e.eec_seqno == seqno));
            _context.SaveChanges();
        }

        public void AddDependent(admin_hrm_emp_dependents dependents)
        {
            dependents.ed_seqno = _context.admin_hrm_emp_dependents.Count(e => e.emp_number == dependents.emp_number) + 1;
            _context.admin_hrm_emp_dependents.Add(dependents);
            _context.SaveChanges();
        }

        public void DeleteDependent(int emp_number, int seqno)
        {
            _context.admin_hrm_emp_dependents.Remove(_context.admin_hrm_emp_dependents.FirstOrDefault(e => e.emp_number == emp_number && e.ed_seqno == seqno));
            _context.SaveChanges();
        }

        public void AddEducation(admin_hrm_emp_education education)
        {
            _context.admin_hrm_emp_education.Add(education);
            _context.SaveChanges();
        }

        public void DeleteEducation(int id)
        {
            _context.admin_hrm_emp_education.Remove(_context.admin_hrm_emp_education.FirstOrDefault(e => e.id == id));
            _context.SaveChanges();
        }

        public void AddMembership(admin_hrm_emp_member_detail member_Detail)
        {
            _context.admin_hrm_emp_member_detail.Add(member_Detail);
            _context.SaveChanges();
        }

        public void DeleteMembership(int id)
        {
            //_context.admin_hrm_emp_member_detail.Remove(_context.admin_hrm_emp_member_detail.FirstOrDefault(e=>e.))
        }

        public void AddDocument(admin_hrm_uploaded_document document,int emp_number)
        {
            _context.admin_hrm_uploaded_document.Add(document);

            var emp_file = new admin_hrm_emp_file
            {
                emp_number = emp_number,
                file_id = document.file_id
            };

            _context.admin_hrm_emp_file.Add(emp_file);

            _context.SaveChanges();
        }

        public void UpdateUsername(admin_hrm_employee employee)
        {
            var emp = _context.admin_hrm_employee.FirstOrDefault(e => e.emp_number == employee.emp_number);
            if (emp != null)
            {
                //check the username of the person
                var user = _context.AspNetUsers.FirstOrDefault(e => e.Id == emp.user_id);
                if (user != null)
                {
                    //check if there is a change in username
                    if (employee.emp_work_email != user.UserName)
                    {
                        var old = user;
                        user.UserName = employee.emp_work_email;
                        user.Email = employee.emp_work_email;

                        _context.Entry(old).CurrentValues.SetValues(user);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public admin_hrm_uploaded_document GetDocument(Guid document_id)
        {
            return _context.admin_hrm_uploaded_document.FirstOrDefault(e => e.file_id == document_id);
        }

        public void AddReportTo(admin_hrm_emp_reportto reportto)
        {
            //remove supervisee from old supervisor
            var old_reportTo = _context.admin_hrm_emp_reportto.FirstOrDefault(e => e.erep_sub_emp_number == reportto.erep_sub_emp_number);
            if (old_reportTo != null)
            {
                _context.admin_hrm_emp_reportto.Remove(old_reportTo);
            }
            _context.admin_hrm_emp_reportto.Add(reportto);
            _context.SaveChanges();
        }

        public admin_hrm_directorate GetDirectorate(int emp_number)
        {
            var employee = _context.admin_hrm_employee.FirstOrDefault(e => e.emp_number == emp_number);
            var subunit_id = employee.admin_hrm_emp_job_record.subunit_id;
            return _context.admin_hrm_lk_directorate_subunit.First(e => e.subunit_id == subunit_id).admin_hrm_directorate;
        }

        public void AddLocation(admin_hrm_emp_locations locations)
        {
            _context.admin_hrm_emp_locations.Add(locations);
            _context.SaveChanges();
        }

        public admin_hrm_employee GetEmployee(string email)
        {
            return _context.admin_hrm_employee.FirstOrDefault(m => m.emp_work_email == email && m.deleted == 0);
        }

        public admin_hrm_employee GetDeactivatedEmployee(string email)
        {
            return _context.admin_hrm_employee.FirstOrDefault(m => m.emp_work_email == email);
        }

        public void DeactivateEmployee(int emp_number)
        {
            var old_emp = _context.admin_hrm_employee.FirstOrDefault(e => e.emp_number == emp_number);
            if (old_emp != null)
            {
                old_emp.active = 0;
                old_emp.deleted = 1;
                _context.Entry(old_emp).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
    }
}
