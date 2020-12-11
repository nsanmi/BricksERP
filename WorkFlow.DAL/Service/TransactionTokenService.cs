using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.IService;

namespace WorkFlow.DAL.Service
{
    public class TransactionTokenService: ITransactionTokenService
    {
        readonly workFlowEntities _context = new workFlowEntities();

        public int AddToken(bpm_transaction_token token)
        {

            token.created_at = DateTime.Now;
            _context.bpm_transaction_token.Add(token);
            _context.SaveChanges();

            return token.id;
        }

        public IQueryable<bpm_transaction_token> GetTransactionTokens(Guid transaction_id)
        {
            return _context.bpm_transaction_token.Where(e => e.transaction_id == transaction_id);
        }

        public bpm_transaction_token GetToken(int id)
        {
            return _context.bpm_transaction_token.FirstOrDefault(e => e.id == id);
        }

        public void AddApprovalToken(bpm_approval_token token)
        {
            token.created_at = DateTime.Now;
            _context.bpm_approval_token.Add(token);
            _context.SaveChanges();
        }

        public bpm_approval_token GetApprovalToken(string token_value)
        {
            return _context.bpm_approval_token.FirstOrDefault(e => e.token == token_value);
        }

        public void UpdateApprovalToken(bpm_approval_token token)
        {
            var old_token = _context.bpm_approval_token.FirstOrDefault(e => e.id == token.id);
            _context.Entry(old_token).CurrentValues.SetValues(token);
            _context.SaveChanges();
        }
    }
}
