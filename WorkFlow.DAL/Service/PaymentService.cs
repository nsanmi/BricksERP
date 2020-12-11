using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.IService;

namespace WorkFlow.DAL.Service
{
    public class PaymentService: IPaymentService
    {
        readonly workFlowEntities _context = new workFlowEntities();

        public void AddPayment(bpm_payments payment)
        {
            payment.status = 0;
            _context.bpm_payments.Add(payment);
            _context.SaveChanges();
        }

        public bpm_payments GetPayment(int payment_id)
        {
            return _context.bpm_payments.FirstOrDefault(e => e.id == payment_id);
        }

        public IQueryable<bpm_payments> GetPayments()
        {
            return _context.bpm_payments.AsQueryable();
        }

        public void Update(bpm_payments payment)
        {
            var old = _context.bpm_payments.FirstOrDefault(e => e.id == payment.id);
            _context.Entry(old).CurrentValues.SetValues(payment);
            _context.SaveChanges();
        }
    }
}
