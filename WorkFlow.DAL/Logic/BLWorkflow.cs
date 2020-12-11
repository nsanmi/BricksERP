using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.Service;

namespace WorkFlow.DAL.Logic
{
    public class BLWorkflow
    {
        WorkflowService _workFlowService = new WorkflowService();
        ProcessService _processService = new ProcessService();
        bpm_workflow workflow = null;
        List<Step> steps = null;
        XElement workflow_element = null;

        public BLWorkflow(Guid workflow_id)
        {
            workflow = _workFlowService.GetWorkflow(workflow_id);
            steps = new List<Step>();
            if (workflow != null)
            {
                workflow_element = XElement.Parse(workflow.workflow);
                LoadSteps();
            }
               
        }

        //get the current workflow
        public bpm_workflow GetWorkflow()
        {
            return workflow;
        }

        //get all the steps of the workflow
        public List<Step> GetSteps()
        {
            return steps;
        }

        //Load all the steps of the workflow
        private void LoadSteps()
        {
            
            if (workflow_element.Element("steps").Elements("step").Any())
            {
               
                foreach(var element in workflow_element.Element("steps").Elements("step"))
                {
                    //var name = element.Element("name").Value;
                    //var code = element.Element("code").Value;

                    steps.Add(new Step {Id=Convert.ToInt32(element.Element("code").Value),Name=element.Element("name").Value,Definition=element });
                    
                }
            }
        }

        public Step GetCurrentStep()
        {
            return steps.FirstOrDefault(e => e.Id == Convert.ToInt32(workflow_element.Element("next_step").Value));
        }

        public Step GetStep(int step_id)
        {
            return steps.FirstOrDefault(e => e.Id == step_id);
        }

        public int GetCompletePercentage()
        {
            return (Convert.ToInt32(workflow_element.Element("next_step").Value) / steps.Count) * 100;
        }

        public XElement GetDefinition()
        {
            return workflow_element;
        }

        
    }
}
