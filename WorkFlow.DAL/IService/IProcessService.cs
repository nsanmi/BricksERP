using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.DAL.Data;

namespace WorkFlow.DAL.IService
{
    public interface IProcessService
    {
        bpm_process AddProcess(bpm_process process);
        bpm_process GetProcess(Guid id);
        IQueryable<bpm_process> GetProcesses();
        void Update(bpm_process process);
        void Delete(Guid id);
    }
}
