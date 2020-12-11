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
    public class SettingsService: ISettingsService
    {
        readonly oneportalEntities _context = new oneportalEntities();

        public void AddPermissionsToRole(List<int> permissions,string role)
        {
            foreach(var permission in permissions)
            {
                _context.AspNetRolePermissions.Add(new AspNetRolePermission { RoleId = role, PermissionId = permission });
            }

            _context.SaveChanges();
        }

        public List<AspNetPermission> GetPermissions()
        {
            return _context.AspNetPermissions.ToList();
        }

        public ws_lookup GetLookup(int LookupId)
        {
            return _context.ws_lookup.Find(LookupId);
        }

        public IQueryable<ws_lookup> GetAllLookup()
        {

            return _context.ws_lookup.AsQueryable();
        }

        public int AddLookUp(ws_lookup lookup)
        {
            _context.ws_lookup.Add(lookup);

            _context.SaveChanges();
            return lookup.id;
        }

     
        public void UpdateLookUp(ws_lookup lookup)
        {
            var existingLookup = _context.ws_lookup.First(e => e.id == lookup.id);

            existingLookup.lookup_name = lookup.lookup_name;
            existingLookup.category = lookup.lookup_name;
            existingLookup.orderd = lookup.orderd;
            _context.Entry(existingLookup).State = EntityState.Modified;

            _context.SaveChanges();

        }

        public int DeleteLookUp(int id)
        {
            var existingLookup = _context.ws_lookup.First(m => m.id == id);
            existingLookup.deleted = 1;
            _context.Entry(existingLookup).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingLookup.id;
        }



        public admin_hrm_lkup_job_title GetJobTitle(int jobTitleId)
        {
            return _context.admin_hrm_lkup_job_title.Find(jobTitleId);
        }

        public IQueryable<admin_hrm_lkup_job_title> GetAllJobTitle()
        {
            return _context.admin_hrm_lkup_job_title.Where(m => m.is_deleted == 0).OrderByDescending(m => m.id);
        }

        public int AddJobTitle(admin_hrm_lkup_job_title jobTitle)
        {
            _context.admin_hrm_lkup_job_title.Add(jobTitle);

            _context.SaveChanges();
            return jobTitle.id;
        }


        public void UpdateJobTitle(admin_hrm_lkup_job_title jobTitle)
        {
            var existingJobTitle = _context.admin_hrm_lkup_job_title.First(e => e.id == jobTitle.id);

            existingJobTitle.job_title = jobTitle.job_title;
            existingJobTitle.job_description = jobTitle.job_description;
            existingJobTitle.note = jobTitle.note;
            _context.Entry(existingJobTitle).State = EntityState.Modified;

            _context.SaveChanges();

        }

        public int DeleteJobTitle(int id)
        {
            var existingJobTitle = _context.admin_hrm_lkup_job_title.First(m => m.id == id);
            existingJobTitle.is_deleted = 1;
            _context.Entry(existingJobTitle).State = EntityState.Modified;
         
            _context.SaveChanges();
            return existingJobTitle.id;
        }




        public admin_hrm_lkup_subunit GetSubunit(int subunitId)
        {
            return _context.admin_hrm_lkup_subunit.Find(subunitId);
        }

        public IQueryable<admin_hrm_lkup_subunit> GetAllSubunit()
        {

            return _context.admin_hrm_lkup_subunit.AsQueryable();
        }

        public int AddSubunit(admin_hrm_lkup_subunit subunit)
        {
            _context.admin_hrm_lkup_subunit.Add(subunit);

            _context.SaveChanges();
            return subunit.id;
        }


        public void UpdateSubunit(admin_hrm_lkup_subunit subunit)
        {
            var existingSubunit = _context.admin_hrm_lkup_subunit.First(e => e.id == subunit.id);

            existingSubunit.name = subunit.name;
            existingSubunit.rgt = subunit.rgt;
            existingSubunit.level = subunit.level;
            existingSubunit.lft = subunit.lft;
            _context.Entry(existingSubunit).State = EntityState.Modified;

            _context.SaveChanges();

        }

        public int DeleteSubunit(int id)
        {
            var existingSubunit = _context.admin_hrm_lkup_subunit.First(m => m.id == id);
            existingSubunit.id = 1;
            _context.Entry(existingSubunit).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingSubunit.id;
        }



        public admin_hrm_lkup_employment_status GetEmploymentStatus(int employmentStatusId)
        {
            return _context.admin_hrm_lkup_employment_status.Find(employmentStatusId);
        }

        public IQueryable<admin_hrm_lkup_employment_status> GetAllEmploymentStatus()
        {

            return _context.admin_hrm_lkup_employment_status.AsQueryable();
        }

        public int AddEmploymentStatus(admin_hrm_lkup_employment_status employmentStatus)
        {
            _context.admin_hrm_lkup_employment_status.Add(employmentStatus);

            _context.SaveChanges();
            return employmentStatus.id;
        }


        public void UpdateEmploymentStatus(admin_hrm_lkup_employment_status employmentStatus)
        {
            var existingEmploymentStatus = _context.admin_hrm_lkup_employment_status.First(e => e.id == employmentStatus.id);

            existingEmploymentStatus.name = employmentStatus.name;
           
            _context.Entry(existingEmploymentStatus).State = EntityState.Modified;

            _context.SaveChanges();

        }

        public int DeleteEmploymentStatus(int id)
        {
            var existingEmploymentStatus = _context.admin_hrm_lkup_employment_status.First(m => m.id == id);
            existingEmploymentStatus.id = 1;
            _context.Entry(existingEmploymentStatus).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingEmploymentStatus.id;
        }




        public admin_hrm_location GetLocation(int locationId)
        {
            return _context.admin_hrm_location.Find(locationId);
        }

        public IQueryable<admin_hrm_location> GetAllLocation()
        {

            return _context.admin_hrm_location.AsQueryable();
        }

        public int AddLocation(admin_hrm_location location)
        {
            _context.admin_hrm_location.Add(location);

            _context.SaveChanges();
            return location.id;
        }


        public void UpdateLocation(admin_hrm_location location)
        {
            var existingLocation = _context.admin_hrm_location.First(e => e.id == location.id);

            existingLocation.name = location.name;
            existingLocation.phone = location.phone;
            existingLocation.zip_code = location.zip_code;
            existingLocation.time_zone = location.time_zone;
            existingLocation.province = location.province;
            existingLocation.notes = location.notes;
            _context.Entry(existingLocation).State = EntityState.Modified;

            _context.SaveChanges();

        }

        public int DeleteLocation(int id)
        {
            var existingLocation = _context.admin_hrm_location.First(m => m.id == id);
            existingLocation.id = 1;
            _context.Entry(existingLocation).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingLocation.id;
        }



        public ws_app_settings GetAppSettings(int appsettingsId)
        {
            return _context.ws_app_settings.Find(appsettingsId);
        }

        public IQueryable<ws_app_settings> GetAllAppSettings()
        {

            return _context.ws_app_settings.AsQueryable();
        }

        public int AddAppSettings(ws_app_settings appsettings)
        {
            _context.ws_app_settings.Add(appsettings);

            _context.SaveChanges();
            return appsettings.id;
        }


        public void UpdateAppSettings(ws_app_settings appsettings)
        {
            var existingAppSettings = _context.ws_app_settings.First(e => e.id == appsettings.id);

            existingAppSettings.key = appsettings.key;
            existingAppSettings.value = appsettings.value;
            

            _context.Entry(existingAppSettings).State = EntityState.Modified;

            _context.SaveChanges();

        }

        public int DeleteAppSettings(int id)
        {
            var existingAppSettings = _context.ws_app_settings.First(m => m.id == id);
            existingAppSettings.id = 1;
            _context.Entry(existingAppSettings).State = EntityState.Modified;
          
            _context.SaveChanges();
            return existingAppSettings.id;
        }



    }
}
