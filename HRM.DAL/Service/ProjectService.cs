using HRM.DAL.IService;
using HRM.DAL.Models;
using System.Linq;

namespace HRM.DAL.Service
{
    public class ProjectService: IProjectService
    {
        readonly oneportalEntities _context = new oneportalEntities();

        public pm_project AddProject(pm_project project)
        {
            
            _context.pm_project.Add(project);
            _context.SaveChanges();

            return project;
        }

      

        public void AddFundingPeriod(pm_project_funding_period funding_period)
        {
            _context.pm_project_funding_period.Add(funding_period);
            _context.SaveChanges();
        }

        public void AddProjectAdmin(pm_project_admin admin)
        {
            _context.pm_project_admin.Add(admin);
            _context.SaveChanges();
        }

        public IQueryable<pm_project> GetProjects()
        {
            return _context.pm_project.Where(e=>e.sub_parent == null).AsQueryable();
        }

        public pm_project GetProject(int project_id)
        {
            return _context.pm_project.FirstOrDefault(e => e.id == project_id);
        }

        public IQueryable<pm_project> GetSubProjects(int parent_id)
        {
            return _context.pm_project.Where(e => e.sub_parent == parent_id);
        }

        public pm_project_strategic_objective AddProjectObjective(pm_project_strategic_objective objective)
        {
            _context.pm_project_strategic_objective.Add(objective);
            _context.SaveChanges();

            return objective;
        }


        public pm_project_strategic_objective GetObjective(int objective_id)
        {
            return _context.pm_project_strategic_objective.FirstOrDefault(e => e.id == objective_id);
        }



        public void RemoveAdmin(pm_project_admin admin)
        {
            _context.pm_project_admin.Remove(_context.pm_project_admin.FirstOrDefault(e=>e.project_id==admin.project_id && e.emp_number==admin.emp_number));
            _context.SaveChanges();
        }

        public void UpdateProject(pm_project project)
        {
            var old = _context.pm_project.FirstOrDefault(e => e.id == project.id);
            project.created_at = old.created_at;
            _context.Entry(old).CurrentValues.SetValues(project);
            _context.SaveChanges();
        }

        public void UpdateObjective(pm_project_strategic_objective objective)
        {
            var old = _context.pm_project_strategic_objective.FirstOrDefault(e => e.id == objective.id);
            _context.Entry(old).CurrentValues.SetValues(objective);
            _context.SaveChanges();
        }


        public void AddObjectiveAdmin(pm_objective_admin admin)
        {
            _context.pm_objective_admin.Add(admin);
            _context.SaveChanges();
        }

        public void RemoveObjectiveAdmin(pm_objective_admin admin)
        {
            _context.pm_objective_admin.Remove(_context.pm_objective_admin.FirstOrDefault(e => e.objective_id == admin.objective_id && e.emp_number == admin.emp_number));
            _context.SaveChanges();
        }


        public pm_project_files AddProjectFile(pm_project_files file)
        {
            _context.pm_project_files.Add(file);
            _context.SaveChanges();

            return file;
        }

        public pm_project_files GetProjectFile(int file_id)
        {
            return _context.pm_project_files.FirstOrDefault(e => e.id == file_id);
        }

        public IQueryable<pm_project_files> GetProjectFiles(int project_id)
        {
            return _context.pm_project_files.Where(e => e.project_id == project_id);
        }


        public IQueryable<pm_objective_files> GetObjectiveFiles(int objective_id)
        {
            return _context.pm_objective_files.Where(e => e.objective_id == objective_id);
        }


        public pm_objective_files AddObjectiveFile(pm_objective_files files)
        {
            _context.pm_objective_files.Add(files);
            _context.SaveChanges();

            return files;
        }

        public IQueryable<pm_project> GetUserProjects(int emp_number)
        {
            var project_id = _context.pm_project_admin.Where(e => e.emp_number == emp_number).Select(e => e.project_id).ToList();
            return _context.pm_project.Where(e => project_id.Contains(e.id)); 
        }

        public IQueryable<pm_project> GetUserSubProjects(int emp_number)
        {
            var project_id = _context.pm_project_admin.Where(e => e.emp_number == emp_number).Select(e => e.project_id).ToList();
            return _context.pm_project.Where(e => project_id.Contains(e.id) && e.sub_parent != null);
        }


        public IQueryable<pm_project_strategic_objective> GetUserObjectives(int emp_number)
        {
            var obj_ids = _context.pm_objective_admin.Where(e => e.emp_number == emp_number).Select(e => e.objective_id).ToList();
            return _context.pm_project_strategic_objective.Where(e => obj_ids.Contains(emp_number));
        }

        public void DeleteProject(int id)
        {
            _context.pm_project.Remove(_context.pm_project.FirstOrDefault(e => e.id == id));
            _context.SaveChanges();
        }


        public void DeleteObjective(int id)
        {
            var obj = _context.pm_project_strategic_objective.FirstOrDefault(e => e.id == id);
            _context.pm_project_strategic_objective.Remove(_context.pm_project_strategic_objective.FirstOrDefault(e => e.id == id));
            _context.SaveChanges();
        }
    }
}
