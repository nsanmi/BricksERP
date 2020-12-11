using HRM.DAL.IService;
using HRM.DAL.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace OnePortal.Controllers
{
    public class FileApiController : ApiController
    {
        IProjectService _projectService;
        IEmployeeService _employeeService;
        IActivityService _activityService;
        ITaskService _taskService;
        public FileApiController(IProjectService projectService,IEmployeeService employeeService,IActivityService activityService,ITaskService taskService)
        {
            _projectService = projectService;
            _employeeService = employeeService;
            _activityService = activityService;
            _taskService = taskService;
        }

        public HttpResponseMessage PostFile()
        {
            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var postedFile = httpRequest.Files[0];
                string ext = Path.GetExtension(postedFile.FileName).Substring(1);

                if (ext.ToUpper() == "PNG" || ext.ToUpper() == "JPG" || ext.ToUpper() == "GIF" || ext.ToUpper() == "DOC" || ext.ToUpper() == "DOCX" || ext.ToUpper() == "PDF" || ext.ToUpper() == "XLS" || ext.ToUpper() == "XLSX" || ext.ToUpper() == "PPT" || ext.ToUpper() == "PPTX")
                {
                   
                    if (httpRequest.Form["module"].ToString() == "project")
                    {
                        var directory = "~/Uploads/project/" + httpRequest.Form["project_id"];
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(directory)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(directory));
                        }
                        var filePath = HttpContext.Current.Server.MapPath(directory +"/"+ postedFile.FileName);
                        postedFile.SaveAs(filePath);

                        
                        var file = new pm_project_files();
                        file.file_name = postedFile.FileName;
                        file.processed = 0;
                        file.project_id = Convert.ToInt32(httpRequest.Form["project_id"]);
                        file.uploaded_at = DateTime.Now;
                        file.uploaded_by = employee.emp_number;
                        file.updated_at = DateTime.Now;
                        file.updated_by= employee.emp_number;
                        file.file_type = ext;
                        file.file_size = postedFile.ContentLength;

                        _projectService.AddProjectFile(file);
                        if (file != null)
                        {
                            return Request.CreateResponse<string>(HttpStatusCode.OK, "Success");
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, " File could not be uploaded");
                        }
                    }
                    else if (httpRequest.Form["module"].ToString() == "objective")
                    {
                        var objective = _projectService.GetObjective(Convert.ToInt32(httpRequest.Form["objective_id"]));
                        var directory = "~/Uploads/project/" + objective.project_id + "/" + objective.id;
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(directory)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(directory));
                        }
                        var filePath = HttpContext.Current.Server.MapPath(directory + "/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);


                        var file = new pm_objective_files();
                        file.file_name = postedFile.FileName;
                        file.processed = 0;
                        file.objective_id =objective.id;
                        file.uploaded_at = DateTime.Now;
                        file.uploaded_by = employee.emp_number;
                        file.updated_at = DateTime.Now;
                        file.updated_by = employee.emp_number;
                        file.file_type = ext;
                        file.file_size = postedFile.ContentLength.ToString();

                        _projectService.AddObjectiveFile(file);
                        if (file != null)
                        {
                            return Request.CreateResponse<string>(HttpStatusCode.OK, "Success");
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, " File could not be uploaded");
                        }
                    }
                    else if (httpRequest.Form["module"].ToString() == "activity")
                    {
                        var activity = _activityService.GetActivity(Convert.ToInt32(httpRequest.Form["activity_id"]));
                        var directory = "~/Uploads/project/" + activity.pm_project_strategic_objective.project_id + "/" + activity.objective_id+"/"+activity.id;
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(directory)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(directory));
                        }
                        var filePath = HttpContext.Current.Server.MapPath(directory + "/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);


                        var file = new pm_activity_files();
                        file.file_name = postedFile.FileName;
                        file.processed = 0;
                        file.activity_id = activity.id;
                        file.uploaded_at = DateTime.Now;
                        file.uploaded_by = employee.emp_number;
                        file.updated_at = DateTime.Now;
                        file.updated_by = employee.emp_number;
                        file.file_type = ext;
                        file.file_size = postedFile.ContentLength;

                        _activityService.AddFile(file);
                        if (file != null)
                        {
                            return Request.CreateResponse<string>(HttpStatusCode.OK, "Success");
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, " File could not be uploaded");
                        }
                    }
                    else if (httpRequest.Form["module"].ToString() == "task")
                    {
                        var task = _taskService.GetTask(Convert.ToInt32(httpRequest.Form["task_id"]));
                        var directory = "~/Uploads/project/" + task.pm_project_objective_activity.pm_project_strategic_objective.project_id + "/" + task.pm_project_objective_activity.objective_id + "/" + task.objective_activity_id+"/"+task.id;
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(directory)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(directory));
                        }
                        var filePath = HttpContext.Current.Server.MapPath(directory + "/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);


                        var file = new pm_task_files();
                        file.file_name = postedFile.FileName;
                        file.processed = 0;
                        file.task_id = task.id;
                        file.uploaded_at = DateTime.Now;
                        file.uploaded_by = employee.emp_number;
                        file.updated_at = DateTime.Now;
                        file.updated_by = employee.emp_number;
                        file.file_type = ext;
                        file.file_size = postedFile.ContentLength;

                        _taskService.AddFile(file);
                        if (file != null)
                        {
                            return Request.CreateResponse<string>(HttpStatusCode.OK, "Success");
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, " File could not be uploaded");
                        }
                    }
                    else if (httpRequest.Form["module"].ToString() == "hrm")
                    {
                        var file_id = Guid.NewGuid();
                        var directory = "~/Uploads/hrm/" + httpRequest.Form["emp_number"];
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(directory)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(directory));
                        }
                        var filePath = HttpContext.Current.Server.MapPath(directory + "/" + file_id+"."+ext);
                        postedFile.SaveAs(filePath);

                        var file_type = httpRequest.Form["file_type"];

                        var document = new admin_hrm_uploaded_document();
                        document.added_by = user_id;
                        document.date_added = DateTime.Now;
                        document.file_name= file_type + " - " + postedFile.FileName;
                        document.size = postedFile.ContentLength;
                        document.file_id = file_id;
                        document.type = ext;

                        _employeeService.AddDocument(document,employee.emp_number);

                        if (document != null)
                        {
                            return Request.CreateResponse<string>(HttpStatusCode.OK, "Success");
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File could not be uploaded");
                        }
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid module");

                }
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid file type");

            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid upload");
        }

        public HttpResponseMessage GetFile(Guid id)
        {

            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
           
             
            var document = _employeeService.GetDocument(id);
            if (document != null)
            {
                string ext = Path.GetExtension(document.file_name);

                var path = "~/Uploads/hrm/" + document.admin_hrm_emp_file.FirstOrDefault().emp_number + "/" + document.file_id + ext;
                var file_Path = HttpContext.Current.Server.MapPath(path);

                if (!File.Exists(file_Path))
                {
                    result = Request.CreateResponse(HttpStatusCode.Gone);
                }
                else
                {
                    // Serve the file to the client
                    result = Request.CreateResponse(HttpStatusCode.OK);
                    result.Content = new StreamContent(new FileStream(file_Path, FileMode.Open, FileAccess.Read));
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = document.file_name;
                }
                
            }

            return result;
        }


        public HttpResponseMessage PostDownloadFile()
        {

            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            var module = httpRequest["module"].ToString();
            if (module == "hrm")
            {
                var id =new Guid(httpRequest["id"]);
                var document = _employeeService.GetDocument(id);
                if (document != null)
                {
                    string ext = Path.GetExtension(document.file_name);

                    var path = "~/Uploads/hrm/"+document.admin_hrm_emp_file.FirstOrDefault().emp_number +"/"+document.file_id+ext;
                    var file_Path = HttpContext.Current.Server.MapPath(path);

                    if (!File.Exists(file_Path))
                    {
                        result = Request.CreateResponse(HttpStatusCode.Gone);
                    }
                    else
                    {
                        // Serve the file to the client
                        result = Request.CreateResponse(HttpStatusCode.OK);
                        result.Content = new StreamContent(new FileStream(file_Path, FileMode.Open, FileAccess.Read));
                        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                        result.Content.Headers.ContentDisposition.FileName = document.file_name;
                    }
                }
            }
            
            //var localFilePath = HttpContext.Current.Server.MapPath("~/timetable.jpg");

            //if (!File.Exists(localFilePath))
            //{
            //    result = Request.CreateResponse(HttpStatusCode.Gone);
            //}
            //else
            //{
            //    // Serve the file to the client
            //    result = Request.CreateResponse(HttpStatusCode.OK);
            //    result.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
            //    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            //    result.Content.Headers.ContentDisposition.FileName = "SampleImg";
            //}

            return result;
        }
    }
}
