using HRM.DAL.IService;
using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Service
{
    public class TaskService: ITaskService
    {
        readonly oneportalEntities _context = new oneportalEntities();

        public pm_project_task AddTask(pm_project_task task)
        {
            _context.pm_project_task.Add(task);
            _context.SaveChanges();
            return task;
        }

        public pm_project_task GetTask(int task_id)
        {
            return _context.pm_project_task.FirstOrDefault(e => e.id == task_id);
        }

        public void AddAdmin(pm_task_admin admin)
        {
            _context.pm_task_admin.Add(admin);
            _context.SaveChanges();
        }


        public void UpdateTask(pm_project_task task)
        {
            var old_task = _context.pm_project_task.FirstOrDefault(e => e.id == task.id);
            _context.Entry(old_task).CurrentValues.SetValues(task);
            _context.SaveChanges();
        }


        public void RemoveAdmin(pm_task_admin admin)
        {
            _context.pm_task_admin.Remove(_context.pm_task_admin.FirstOrDefault(e=>e.task_id==admin.task_id && e.emp_number==admin.emp_number));
            _context.SaveChanges();
        }

        public void AddDeliverable(pm_task_deliverables deliverables)
        {
            _context.pm_task_deliverables.Add(deliverables);
            _context.SaveChanges();
        }

        public void DeleteDeliverable(int id)
        {
            _context.pm_task_deliverables.Remove(_context.pm_task_deliverables.FirstOrDefault(e => e.id == id));
            _context.SaveChanges();
        }

        public pm_task_files AddFile(pm_task_files file)
        {
            _context.pm_task_files.Add(file);
            _context.SaveChanges();

            return file;
        }

        public pm_task_files GetFile(int id)
        {
            return _context.pm_task_files.FirstOrDefault(e => e.id == id);
        }
      
        public IQueryable<pm_task_files> GetTaskFiles(int task_id)
        {
            return _context.pm_task_files.Where(e => e.task_id == task_id);
        }

        public IQueryable<pm_project_task> GetUserTask(int emp_number)
        {
            var task_ids = _context.pm_task_admin.Where(e => e.emp_number == emp_number).Select(e => e.task_id);
            return _context.pm_project_task.Where(e => task_ids.Contains(e.id));
        }

        public void Delete(int id)
        {
            _context.pm_project_task.Remove(_context.pm_project_task.FirstOrDefault(e => e.id == id));
            _context.SaveChanges();
        }

    }
}
