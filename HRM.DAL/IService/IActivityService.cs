using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.IService
{
    public interface IActivityService
    {
        pm_project_objective_activity AddActivity(pm_project_objective_activity activity);
        IQueryable<pm_project_objective_activity> GetObjectiveActivities(int objective_id);
        pm_project_objective_activity GetActivity(int activity_id);
        void AddActivityAdmin(pm_activity_admin admin);
        void RemoveActivityAdmin(pm_activity_admin admin);
        pm_activity_files AddFile(pm_activity_files file);
        pm_activity_files GetFile(int id);
        IQueryable<pm_activity_files> GetActivityFiles(int activity_id);
        IQueryable<pm_project_objective_activity> GetUserActivities(int emp_number);
        void Update(pm_project_objective_activity activity);
        void Delete(int id);
        void AddBudget(pm_activity_budget budget);
        void DeleteBudgetItem(int id);
    }
}
