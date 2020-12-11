using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.DAL.Data;

namespace WorkFlow.DAL.IService
{
    public interface IPaymentService
    {
        void AddPayment(bpm_payments payment);
        bpm_payments GetPayment(int payment_id);
        IQueryable<bpm_payments> GetPayments();
        void Update(bpm_payments payment);
    }
}
