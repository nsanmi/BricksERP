using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.IService;

namespace WorkFlow.DAL.Service
{
    public class ProcessService: IProcessService
    {
        readonly workFlowEntities _context = new workFlowEntities();

        public bpm_process AddProcess(bpm_process process)
        {
            process.id = Guid.NewGuid();
            _context.bpm_process.Add(process);
            _context.SaveChanges();
            return process;
        }


        public bpm_process GetProcess(Guid id)
        {
            return _context.bpm_process.FirstOrDefault(e => e.id == id);
        }


        public IQueryable<bpm_process> GetProcesses()
        {
            return _context.bpm_process;
        }

        public void Update(bpm_process process)
        {
            var old_process = _context.bpm_process.FirstOrDefault(e => e.id == process.id);
            _context.Entry(old_process).CurrentValues.SetValues(process);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            _context.bpm_process.Remove(_context.bpm_process.FirstOrDefault(e => e.id == id));
            _context.SaveChanges();
        }
    }
}
