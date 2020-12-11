using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace OnePortal.Helper
{
    public class CommentHelper
    {
        public XElement CreateComment(string comment,string user_id,string employee_name,int comment_type)
        {
            var xml = new XElement("comment_tree",new XElement("comment",comment),new XElement("user_id",user_id),new XElement("employee",employee_name),new XElement("comment_date",DateTime.Now),new XElement("comment_type",comment_type),new XElement("comment_id",Guid.NewGuid()));

            return xml;
        }

        public XElement AddReplyToComment(string comment, string user_id, string employee_name, int comment_type,string reply)
        {
            var main_comment = new XElement(comment);

            var xml = new XElement("reply_item", new XElement("reply", reply), new XElement("user_id", user_id), new XElement("employee", employee_name), new XElement("comment_date", DateTime.Now), new XElement("comment_type", comment_type), new XElement("reply_id", Guid.NewGuid()));

            main_comment.Add(xml);

            return main_comment;
        }

        public XElement AddReplyToReply (string comment, string user_id, string employee_name, int comment_type, string reply,string parent_id)
        {
            var main_comment = new XElement(comment);

            var xml = new XElement("reply_item", new XElement("reply", reply), new XElement("user_id", user_id), new XElement("employee", employee_name), new XElement("comment_date", DateTime.Now), new XElement("comment_type", comment_type), new XElement("reply_id", Guid.NewGuid()));

            main_comment.Elements("reply_item").FirstOrDefault(e => e.Element("reply_id").Value == parent_id).Add(xml);

            return main_comment;
        }
    }
}