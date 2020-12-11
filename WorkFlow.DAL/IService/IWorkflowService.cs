using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.DAL.Data;

namespace WorkFlow.DAL.IService
{
    public interface IWorkflowService
    {
        bpm_workflow AddWorkflow(bpm_workflow workflow);
        IQueryable<bpm_workflow> GetWorkflows();
        bpm_workflow GetWorkflow(Guid id);
        void Update(bpm_workflow workflow);
        IQueryable<bpm_workflow> GetUserWorkflows(int emp_number);
        void AddUserToWorkflow(bpm_workflow_employee workflow_Employee);
        IQueryable<bpm_workflow_employee> GetActionWorkflows(int emp_number);
        IQueryable<bpm_workflow> GetActionWorkflows();
    }
}
