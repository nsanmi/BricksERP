using HRM.DAL.IService;
using HRM.DAL.Models;
using System.Linq;

namespace HRM.DAL.Service
{
    public class ActivityService:IActivityService
    {
        readonly oneportalEntities _context = new oneportalEntities();

        public pm_project_objective_activity AddActivity(pm_project_objective_activity activity)
        {
            _context.pm_project_objective_activity.Add(activity);
            _context.SaveChanges();

            return activity;
        }

        public IQueryable<pm_project_objective_activity> GetObjectiveActivities(int objective_id)
        {
            return _context.pm_project_objective_activity.Where(e => e.objective_id == objective_id);
        }

        public pm_project_objective_activity GetActivity(int activity_id)
        {
            return _context.pm_project_objective_activity.FirstOrDefault(e => e.id == activity_id);
        }

        public void AddActivityAdmin(pm_activity_admin admin)
        {
            _context.pm_activity_admin.Add(admin);
            _context.SaveChanges();
        }

        public void RemoveActivityAdmin(pm_activity_admin admin)
        {
            _context.pm_activity_admin.Remove(_context.pm_activity_admin.FirstOrDefault(e => e.activity_id == admin.activity_id && e.emp_number == admin.emp_number));
            _context.SaveChanges();
        }

        public pm_activity_files AddFile(pm_activity_files file)
        {
            _context.pm_activity_files.Add(file);
            _context.SaveChanges();

            return file;
        }

        public IQueryable<pm_project_objective_activity> GetUserActivities(int emp_number)
        {
            var activities = _context.pm_activity_admin.Where(e => e.emp_number == emp_number).Select(e => e.activity_id).ToList();
            return _context.pm_project_objective_activity.Where(e => activities.Contains(e.id));
        }

        public pm_activity_files GetFile(int id)
        {
            return _context.pm_activity_files.FirstOrDefault(e => e.id == id);
        }

        public IQueryable<pm_activity_files> GetActivityFiles(int activity_id)
        {
            return _context.pm_activity_files.Where(e => e.activity_id == activity_id);
        }

        public void Update(pm_project_objective_activity activity)
        {
            var old = _context.pm_project_objective_activity.FirstOrDefault(e => e.id == activity.id);
            _context.Entry(old).CurrentValues.SetValues(activity);
            _context.SaveChanges();
        }


        public void Delete(int id)
        {
            _context.pm_project_objective_activity.Remove(_context.pm_project_objective_activity.FirstOrDefault(e => e.id == id));
            _context.SaveChanges();
        }

        public void AddBudget(pm_activity_budget budget)
        {
            _context.pm_activity_budget.Add(budget);
            _context.SaveChanges();
        }

        public void DeleteBudgetItem(int id)
        {
            _context.pm_activity_budget.Remove(_context.pm_activity_budget.FirstOrDefault(e => e.id == id));
            _context.SaveChanges();
        }
    }
}
