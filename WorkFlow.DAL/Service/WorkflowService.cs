using HRM.DAL.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.IService;

namespace WorkFlow.DAL.Service
{
    public class WorkflowService:IWorkflowService
    {
        readonly workFlowEntities _context = new workFlowEntities();

        public bpm_workflow AddWorkflow(bpm_workflow workflow)
        {
            workflow.status = 22;
            _context.bpm_workflow.Add(workflow);
            _context.SaveChanges();

            return workflow;
        }

        public IQueryable<bpm_workflow> GetWorkflows()
        {
            return _context.bpm_workflow.Where(e=>e.deleted==0).AsQueryable();
        }

        public bpm_workflow GetWorkflow(Guid id)
        {
            return _context.bpm_workflow.FirstOrDefault(e => e.id == id && e.deleted == 0);
        }

        public void Update(bpm_workflow workflow)
        {
            var old_workflow = _context.bpm_workflow.FirstOrDefault(e => e.id == workflow.id);
            _context.Entry(old_workflow).CurrentValues.SetValues(workflow);
            _context.SaveChanges();
        }

        public IQueryable<bpm_workflow> GetUserWorkflows(int emp_number)
        {
            return _context.bpm_workflow.Where(e=>e.created_by==emp_number && e.deleted == 0).AsQueryable();
        }

        public void AddUserToWorkflow(bpm_workflow_employee workflow_Employee)
        {
            try
            {
                var wk_emp = _context.bpm_workflow_employee.FirstOrDefault(e => e.workflow_id == workflow_Employee.workflow_id);
                if (wk_emp != null)
                {
                    _context.bpm_workflow_employee.Remove(wk_emp);
                }
                _context.bpm_workflow_employee.Add(workflow_Employee);
                _context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        Utils.Log(message + " - AddUserToWorkflow");
                    }
                }
            }

            //var workflows = _context.bpm_workflow_employee.Where(e => e.workflow_id == workflow_Employee.workflow_id).AsQueryable();
            //foreach(var workflow in workflows)
            //{
            //    workflow.pending = 0;
            //}
            //_context.SaveChanges();

            ////check if the employee is already in the workflow
            //var work_emp = _context.bpm_workflow_employee.FirstOrDefault(e => e.emp_number == workflow_Employee.emp_number);
            //if (work_emp == null)
            //{

            //    _context.bpm_workflow_employee.Add(workflow_Employee);
            //    _context.SaveChanges();
            //}
            //else
            //{
            //    _context.bpm_workflow_employee.Remove(_context.bpm_workflow_employee.FirstOrDefault(e => e.emp_number == workflow_Employee.emp_number));
            //    _context.bpm_workflow_employee.Add(workflow_Employee);
            //    _context.SaveChanges();
            //}
        }

        public IQueryable<bpm_workflow_employee> GetActionWorkflows(int emp_number)
        {
            //var workflow_ids = _context.bpm_workflow_employee.Where(e => e.emp_number == emp_number && e.bpm_workflow.deleted == 0).Select(e=>e.workflow_id);
            return _context.bpm_workflow_employee.Where(e => e.emp_number == emp_number && e.bpm_workflow.deleted == 0);
        }

        public IQueryable<bpm_workflow> GetActionWorkflows()
        {
            //var workflow_ids = _context.bpm_workflow_employee.Where(e => e.bpm_workflow.deleted == 0).Select(e => e.workflow_id);
            return _context.bpm_workflow.Where(e=>e.deleted==0).AsQueryable();
        }
    }
}
