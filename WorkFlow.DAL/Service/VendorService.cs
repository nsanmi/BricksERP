using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.IService;

namespace WorkFlow.DAL.Service
{
    public  class VendorService:IVendorService
    {
        readonly workFlowEntities _context = new workFlowEntities();

        public bpm_vendor AddVendor(bpm_vendor vendor)
        {
            vendor.created_at = DateTime.Now;
            _context.bpm_vendor.Add(vendor);
            _context.SaveChanges();

            return vendor;
        }

        public void AddCategory(List<bpm_lnk_vendor_category> vendor_Category)
        {
            try
            {
                if (vendor_Category.Any())
                {
                    _context.bpm_lnk_vendor_category.RemoveRange(_context.bpm_lnk_vendor_category.Where(e => e.vendor_id == vendor_Category[0].vendor_id));
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {

            }
            

            foreach(var category in vendor_Category)
            {
                _context.bpm_lnk_vendor_category.Add(category);
            }
            _context.SaveChanges();
        }

        public bpm_vendor GetVendor(int vendor_id)
        {
            return _context.bpm_vendor.FirstOrDefault(e => e.id == vendor_id);
        }

        public IQueryable<bpm_vendor> GetVendors()
        {
            return _context.bpm_vendor.AsQueryable();
        }

        public void AddDocument(bpm_vendor_document document)
        {
            document.created_at = DateTime.Now;
            document.id = Guid.NewGuid();
            _context.bpm_vendor_document.Add(document);
            _context.SaveChanges();
        }

        public void DeleteDocument(Guid id)
        {
            _context.bpm_vendor_document.Remove(_context.bpm_vendor_document.FirstOrDefault(e => e.id == id));
            _context.SaveChanges();
        }

        public IQueryable<bpm_vendor_document> GetVendorDocuments(int vendor_id)
        {
            return _context.bpm_vendor_document.Where(e => e.vendor_id == vendor_id);
        }

        public void UpdateVendor(bpm_vendor vendor)
        {
            var old_vendor = _context.bpm_vendor.FirstOrDefault(e => e.id == vendor.id);
            vendor.created_at = old_vendor.created_at;
            vendor.updated_at = DateTime.Now;
            _context.Entry(old_vendor).CurrentValues.SetValues(vendor);
            _context.SaveChanges();
        }

        /*
         * Added by Johnbosco
         * For more on Managing of Vendors
         */
        /*
         * For deleting a vendor
         */
        public bool DeleteVendor(int id)
        {
            var vendorToDelete = _context.bpm_vendor.FirstOrDefault(m => m.id == id);
            if (vendorToDelete != null)
            {
                vendorToDelete.deleted = 1;
                vendorToDelete.updated_at = DateTime.Now;
                _context.Entry(vendorToDelete).State = EntityState.Modified;
                int deletedId = _context.SaveChanges();
                return true;
            }

            return false;
        }
    }
}
