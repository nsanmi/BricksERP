using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.IService
{
    public interface ITaskService
    {
        pm_project_task AddTask(pm_project_task task);
        pm_project_task GetTask(int task_id);
        void AddAdmin(pm_task_admin admin);
        void RemoveAdmin(pm_task_admin admin);
        void UpdateTask(pm_project_task task);
        void DeleteDeliverable(int id);
        void AddDeliverable(pm_task_deliverables deliverables);
        pm_task_files AddFile(pm_task_files file);
        pm_task_files GetFile(int id);
        IQueryable<pm_task_files> GetTaskFiles(int task_id);
        IQueryable<pm_project_task> GetUserTask(int emp_number);
        void Delete(int id);
    }
}
