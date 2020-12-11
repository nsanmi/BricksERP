using HRM.DAL.Models;
using System.Linq;

namespace HRM.DAL.IService
{
    public interface IProjectService
    {
        pm_project AddProject(pm_project project);
        IQueryable<pm_project> GetProjects();
        pm_project GetProject(int project_id);
        IQueryable<pm_project> GetSubProjects(int parent_id);
        pm_project_strategic_objective AddProjectObjective(pm_project_strategic_objective objective);
        void AddFundingPeriod(pm_project_funding_period funding_period);
        void AddProjectAdmin(pm_project_admin admin);
        void UpdateProject(pm_project project);
        void RemoveAdmin(pm_project_admin admin);
        void AddObjectiveAdmin(pm_objective_admin admin);
        void RemoveObjectiveAdmin(pm_objective_admin admin);
        pm_project_strategic_objective GetObjective(int objective_id);
        pm_project_files AddProjectFile(pm_project_files file);
        pm_project_files GetProjectFile(int file_id);
        IQueryable<pm_project_files> GetProjectFiles(int project_id);
        pm_objective_files AddObjectiveFile(pm_objective_files files);
        IQueryable<pm_objective_files> GetObjectiveFiles(int objective_id);
        IQueryable<pm_project> GetUserProjects(int emp_number);
        IQueryable<pm_project_strategic_objective> GetUserObjectives(int emp_number);
        void DeleteProject(int id);
        void DeleteObjective(int id);
        void UpdateObjective(pm_project_strategic_objective objective);
        IQueryable<pm_project> GetUserSubProjects(int emp_number);
    }
}
