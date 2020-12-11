using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.DAL.Data;

namespace WorkFlow.DAL.IService
{
    public interface IVendorService
    {
        bpm_vendor AddVendor(bpm_vendor vendor);
        void AddCategory(List<bpm_lnk_vendor_category> vendor_Category);
        bpm_vendor GetVendor(int vendor_id);
        IQueryable<bpm_vendor> GetVendors();
        IQueryable<bpm_vendor_document> GetVendorDocuments(int vendor_id);
        void DeleteDocument(Guid id);
        void AddDocument(bpm_vendor_document document);
        void UpdateVendor(bpm_vendor vendor);
        bool DeleteVendor(int id);
    }
}
