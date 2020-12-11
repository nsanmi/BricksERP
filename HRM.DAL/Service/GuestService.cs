using HRM.DAL.IService;
using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Service
{
    public class GuestService : IGuestService
    {
        readonly oneportalEntities _context = new oneportalEntities();


            public int AddGuest(Guest guest)
            {
                var id = 0;
                //guest.added_date = DateTime.Now;
                _context.Guests.Add(guest);
                _context.SaveChanges();

                id = guest.GuestID;
                return id;
            }

            //get all the subordinates of an employee
            //public IQueryable<admin_hrm_emp_reportto> GetSubordinates(int emp_number)
            //{
            //    return _context.admin_hrm_emp_reportto.Where(e => e.erep_sup_emp_number == emp_number);
            //}

            ////get all the supervisors of an employee
            //public IQueryable<admin_hrm_emp_reportto> GetSupervisors(int emp_number)
            //{
            //    return _context.admin_hrm_emp_reportto.Where(e => e.erep_sub_emp_number == emp_number);
            //}
            

            public IQueryable<Guest> GetGuests()
            {
                return _context.Guests.Where(e => e.Deleted == 0).AsQueryable();
            }

            public Guest GetGuest(int guest_id)
            {
                return _context.Guests.FirstOrDefault(e => e.GuestID == guest_id);
            }

            public Guest GetGuestByUserId(string user_id)
            {
                return _context.Guests.FirstOrDefault(e => e.GuestID.ToString() == user_id && e.Deleted == 0);
            }

            public void UpdateGuest(Guest guest)
            {
                var old_guest = _context.Guests.FirstOrDefault(e => e.GuestID == guest.GuestID);
                if (old_guest != null)
                {
                    _context.Entry(old_guest).CurrentValues.SetValues(guest);
                    _context.SaveChanges();
                }
            }

          
            public void DeleteGuest(int id)
            {
                _context.Guests.Remove(GetGuest(id));
                _context.SaveChanges();
            }

            //public void AddEmergencyContact(admin_hrm_emp_emergency_contacts contacts)
            //{
            //    contacts.eec_seqno = _context.admin_hrm_emp_emergency_contacts.Count(e => e.emp_number == contacts.emp_number) + 1;
            //    _context.admin_hrm_emp_emergency_contacts.Add(contacts);
            //    _context.SaveChanges();
            //}

            //public void DeleteEmergencyContact(int emp_number, int seqno)
            //{
            //    _context.admin_hrm_emp_emergency_contacts.Remove(_context.admin_hrm_emp_emergency_contacts.FirstOrDefault(e => e.emp_number == emp_number && e.eec_seqno == seqno));
            //    _context.SaveChanges();
            //}
            

            //public void AddDocument(admin_hrm_uploaded_document document, int emp_number)
            //{
            //    _context.admin_hrm_uploaded_document.Add(document);

            //    var emp_file = new admin_hrm_emp_file
            //    {
            //        emp_number = emp_number,
            //        file_id = document.file_id
            //    };

            //    _context.admin_hrm_emp_file.Add(emp_file);

            //    _context.SaveChanges();
            //}

           

            //public admin_hrm_uploaded_document GetDocument(Guid document_id)
            //{
            //    return _context.admin_hrm_uploaded_document.FirstOrDefault(e => e.file_id == document_id);
            //}

            //public void AddReportTo(admin_hrm_emp_reportto reportto)
            //{
            //    //remove supervisee from old supervisor
            //    var old_reportTo = _context.admin_hrm_emp_reportto.FirstOrDefault(e => e.erep_sub_emp_number == reportto.erep_sub_emp_number);
            //    if (old_reportTo != null)
            //    {
            //        _context.admin_hrm_emp_reportto.Remove(old_reportTo);
            //    }
            //    _context.admin_hrm_emp_reportto.Add(reportto);
            //    _context.SaveChanges();
            //}

         

            public void AddAddress(admin_hrm_emp_locations locations)
            {
                _context.admin_hrm_emp_locations.Add(locations);
                _context.SaveChanges();
            }

            public Guest GetGuest(string email)
            {
            
                return _context.Guests.FirstOrDefault(m => m.Email == email && m.Deleted == 0);
            }

            public Guest GetDeactivatedGuest(string email)
            {
                return _context.Guests.FirstOrDefault(m => m.Email == email);
            }

            public void DeactivateGuest(int guest_id)
            {
                var old_guest = _context.Guests.FirstOrDefault(e => e.GuestID == guest_id);
                if (old_guest != null)
                {
                    old_guest.Active = 0;
                    old_guest.Deleted = 1;
                    _context.Entry(old_guest).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
        }
    }


