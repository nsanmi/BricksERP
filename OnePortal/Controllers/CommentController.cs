using HRM.DAL.IService;
using HRM.DAL.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using OnePortal.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnePortal.Controllers
{
    public class CommentController : ApiController
    {
        ICommentService _commentService;
        IEmployeeService _employeeService;
        public CommentController(ICommentService commentService,IEmployeeService employeeService)
        {
            _commentService = commentService;
            _employeeService = employeeService;
        }

        [HttpPost]
        public void PostComment(HttpRequestMessage message)
        {

            var post_text =message.Content.ReadAsStringAsync().Result;
            var obj = JObject.Parse(post_text);

            var comment = WebUtility.HtmlEncode(obj["comment"].ToString());

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);
            var helper = new CommentHelper();

            var cmt = helper.CreateComment(comment, user_id, employee.emp_lastname, 1);

            var p_comment = new pm_comment();
            p_comment.comment = cmt.ToString();
            p_comment.module = obj["module"].ToString();
            p_comment.item_id =Convert.ToInt32(obj["item_id"].ToString());
            p_comment.category = Convert.ToInt32(obj["category"].ToString());

            _commentService.AddComment(p_comment);

           // return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public void PostCommentReply(int comment_id,string reply_text)
        {

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);
            var helper = new CommentHelper();

            var cmt = _commentService.GetComment(comment_id);

            var reply = helper.AddReplyToComment(cmt.comment, user_id, employee.emp_lastname, 1, reply_text);
            cmt.comment = reply.ToString();

            _commentService.UpdateComment(cmt);

           // return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public void PostReplyReply(int comment_id, string reply_text,string parent_id)
        {

            var user_id = User.Identity.GetUserId();
            var employee = _employeeService.GetEmployeeByUserId(user_id);
            var helper = new CommentHelper();

            var cmt = _commentService.GetComment(comment_id);

            var reply = helper.AddReplyToReply(cmt.comment,user_id,employee.emp_lastname,1,reply_text,parent_id);
            cmt.comment = reply.ToString();

           // return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
