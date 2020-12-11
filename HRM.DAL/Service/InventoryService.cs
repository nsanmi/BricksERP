using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM.DAL.IService;
using HRM.DAL.Models;

namespace HRM.DAL.Service
{

    public class InventoryService : IInventoryService
    {
        readonly oneportalEntities _context = new oneportalEntities();

        //public int AddProductCategory (product_category products_categoryies)
        //{


        //    _context.product_category.Add(products_categoryies);

        //    _context.SaveChanges();
        //    return products_categoryies.category_id;
        //}

        //public int DeleteProductCategory(int id)
        //{
        //    var existingproducts = _context.product_category.First(m => m.category_id == id);
        //    existingproducts.Deleted = 1;
        //    _context.Entry(existingproducts).State = EntityState.Modified;
        //    //_context.Entry (existingproducts).CurrentValues.SetValues(id);
        //    _context.SaveChanges();
        //    return existingproducts.category_id;
        //}

        //public IQueryable<admin_hrm_timesheet> GetMonthlyAudit(int year, int month)
        //{
        //    return _context.admin_hrm_timesheet.Where(e => e.start_date.Year == year && e.start_date.Month == month);
        //}
        //public IQueryable<product> GetMonthlyAudit(int year, int month)
        //{
        //    return _context.products.Where(e => e.created_date.Year == year && e.created_date.Month == month);
        //}
       

        public int AddProduct(product products)
        {

            _context.products.Add(products);

            _context.SaveChanges();
            return products.product_id;
        }

        public product GetProducts(int productsId)
        {
            return _context.products.FirstOrDefault(m => m.product_id == productsId);
        }

        public void UpdateProduct(product products)
        {
            var products_old = _context.products.First(e => e.product_id == products.product_id);

            products_old.product_description = products.product_description;
            products_old.updated_date = DateTime.Now;
            _context.Entry(products_old).State = EntityState.Modified;
            _context.SaveChanges();

        }
        public IQueryable<product> GetAllProduct()
        {

            return _context.products.AsQueryable();
        }

        public void AddProductFiles(product_files productsFiles)
        {
            _context.product_files.Add(productsFiles);

            _context.SaveChanges();
        }

        public int DeleteProduct(int id)
        {
            var existingproducts = _context.products.First(m => m.product_id == id);
            existingproducts.Deleted = 1;
            _context.Entry (existingproducts).State = EntityState.Modified;
            //_context.Entry  (existingproducts).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingproducts.product_id;
        }

        //public int Resolvedproducts(int id, string userId)
            //{
            //    var resolve_old =  _context.products.First(m => m.product_id == id);
            //    resolve_old.Resolved = "Yes";
            //    resolve_old.ResolutionDate = DateTime.Now;
            //    resolve_old.ResolvedBy = userId;
            //    _context.Entry(resolve_old).State = EntityState.Modified;
            //    //_context.Entry (existingproducts).CurrentValues.SetValues(id);
            //    _context.SaveChanges();
            //    return resolve_old.Id;
            //}

       public List<string> GetProductAdmin(){
            var productsAdmin = _context.product_admin.Where(m => m.active == 1).Select(m => m.admin_hrm_employee.emp_work_email).ToList();
            return productsAdmin;
        }

        public List<string> ResolvedNotification()
        {
            //var userEmail =   _context.admin_hrm_employee.Where(x => x.u)
            var notification =_context.admin_hrm_employee.Select(m => m.emp_work_email).ToList();
            return notification;

        }


        public product_category GetProductCategory(int categoryId)
        {
            return _context.product_category.Find(categoryId);
        }

        public IQueryable<product_category> GetAllProductCategory()
        {
            return _context.product_category.Where(m => m.Deleted == 0).OrderByDescending(m => m.category_id);
        }

        public int AddProductCategory(product_category category)
        {
            _context.product_category.Add(category);

            _context.SaveChanges();
            return category.category_id;
        }


        public void UpdateProductCategory(product_category category)
        {
            var existingCategory = _context.product_category.First(e => e.category_id == category.category_id);

            existingCategory.name = category.name;
            existingCategory.description = category.description;
            
            _context.Entry(existingCategory).State = EntityState.Modified;

            _context.SaveChanges();

        }

        public int DeleteProductCategory(int id)
        {
            var existingCategory = _context.product_category.First(m => m.category_id == id);
            existingCategory.Deleted = 1;
            _context.Entry(existingCategory).State = EntityState.Modified;

            _context.SaveChanges();
            return existingCategory.category_id;
        }



    }
}
