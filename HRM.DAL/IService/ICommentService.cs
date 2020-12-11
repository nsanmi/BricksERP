using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.IService
{
    public interface ICommentService
    {
        void AddComment(pm_comment comment);
        IQueryable<pm_comment> GetItemComments(int item_id, string module);
        IQueryable<pm_comment> GetItemCategoryComments(int item_id, string module);
        void UpdateComment(pm_comment comment);
        pm_comment GetComment(int comment_id);
    }
}
