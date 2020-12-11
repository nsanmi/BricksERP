using OnePortal.Helper;
using OnePortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnePortal.Controllers
{
    public class WorkflowApiController : ApiController
    {
        public PVWorkflow GetPVWorkflow(Guid id)
        {
            return new PVHelper().GetWorkflowPVSummary(id);
        }
    }
}
