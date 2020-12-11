using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.DAL.Data;

namespace WorkFlow.DAL.IService
{
    public interface ITransactionTokenService
    {
        int AddToken(bpm_transaction_token token);
        IQueryable<bpm_transaction_token> GetTransactionTokens(Guid transaction_id);
        bpm_transaction_token GetToken(int id);
        void AddApprovalToken(bpm_approval_token token);
        bpm_approval_token GetApprovalToken(string token_value);
        void UpdateApprovalToken(bpm_approval_token token);
    }

}
