using HRM.DAL.IService;
using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Service
{
    public class CommentService: ICommentService
    {
        readonly oneportalEntities _context = new oneportalEntities();

        public void AddComment(pm_comment comment)
        {
            _context.pm_comment.Add(comment);
            _context.SaveChanges();
        }

        public IQueryable<pm_comment> GetItemComments(int item_id,string module)
        {
            return _context.pm_comment.Where(e => e.item_id == item_id && e.module == module);
        }

        public pm_comment GetComment(int comment_id)
        {
            return _context.pm_comment.FirstOrDefault(e => e.id == comment_id);
        }

        public IQueryable<pm_comment> GetItemCategoryComments(int item_id, string module)
        {
            return _context.pm_comment.Where(e => e.item_id == item_id && e.module == module);
        }

        public void UpdateComment(pm_comment comment)
        {
            var old_comment = _context.pm_comment.FirstOrDefault(e => e.id == comment.id);
            _context.Entry(old_comment).CurrentValues.SetValues(comment);
            _context.SaveChanges();
        }
    }
}
