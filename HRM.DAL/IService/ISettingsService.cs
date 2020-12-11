using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.IService
{
    public interface ISettingsService
    {
        void AddPermissionsToRole(List<int> permissions, string role);
        List<AspNetPermission> GetPermissions();

        ws_lookup GetLookup(int LookupId);
        IQueryable<ws_lookup> GetAllLookup();

        void UpdateLookUp(ws_lookup lookup);
        int AddLookUp(ws_lookup lookup);
        int DeleteLookUp(int id);


        admin_hrm_lkup_job_title GetJobTitle(int jobTitleId);
        IQueryable<admin_hrm_lkup_job_title> GetAllJobTitle();

        void UpdateJobTitle(admin_hrm_lkup_job_title jobTitle);
        int AddJobTitle(admin_hrm_lkup_job_title jobTitle);
        int DeleteJobTitle(int id);



        admin_hrm_lkup_subunit GetSubunit(int subunitId);
        IQueryable<admin_hrm_lkup_subunit> GetAllSubunit();

        void UpdateSubunit(admin_hrm_lkup_subunit subunit);
        int AddSubunit(admin_hrm_lkup_subunit subunit);
        int DeleteSubunit(int id);


        admin_hrm_lkup_employment_status GetEmploymentStatus(int employmentStatusId);
        IQueryable<admin_hrm_lkup_employment_status> GetAllEmploymentStatus();

        void UpdateEmploymentStatus(admin_hrm_lkup_employment_status employmentStatus);
        int AddEmploymentStatus(admin_hrm_lkup_employment_status employmentStatus);
        int DeleteEmploymentStatus(int id);


        admin_hrm_location GetLocation(int locationId);
        IQueryable<admin_hrm_location> GetAllLocation();

        void UpdateLocation(admin_hrm_location location);
        int AddLocation(admin_hrm_location location);
        int DeleteLocation(int id);



        ws_app_settings GetAppSettings(int appsettingsId);
        IQueryable<ws_app_settings> GetAllAppSettings();

        void UpdateAppSettings(ws_app_settings appsettings);
        int AddAppSettings(ws_app_settings appsettings);
        int DeleteAppSettings(int id);

    }
}
