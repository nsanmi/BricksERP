using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.IService
{
    public interface IInventoryService
    {
        
        //IQueryable<product> GetMonthlyAudit(int year, int month);
        int AddProduct(product products);
        product GetProducts(int ProductId);
        IQueryable<product> GetAllProduct();
        void UpdateProduct(product products);
        void AddProductFiles(product_files productFiles);
        int DeleteProduct(int id);
        //int ResolvedComplain(int id, string userId);
        List<string> GetProductAdmin();
        List<string> ResolvedNotification();

        product_category GetProductCategory(int jobTitleId);
        IQueryable<product_category> GetAllProductCategory();

        void UpdateProductCategory(product_category productcategory);
        int AddProductCategory(product_category productcategory);
        int DeleteProductCategory(int id);

       // int AddProductCategory(product_category product_categories);
       // int DeleteProductCategory(int id);


    }
}
