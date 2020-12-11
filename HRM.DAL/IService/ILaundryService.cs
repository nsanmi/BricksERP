using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.IService
{
   public interface ILaundryService
    {
        // collection and entry
        //IQueryable<collection_register> GetMonthlyAudit(int year, int month);
        int AddEntry(collection_register register);
        collection_register GetEntry(int EntryId);
        IQueryable<collection_register> GetAllEntry();
        void UpdateEntry(collection_register register);
       // void AddEntryFiles(product_files productFiles);
        int DeleteEntry(int id);
        //int ResolvedComplain(int id, string userId);
        List<string> GetLaundryAdmin();
        List<string> ResolvedNotification();

        //pricing 
        int AddPricing(pricing price);
        pricing GetPrice(int id);
        IQueryable<pricing> GetAllPrice();
        void UpdatePrice(pricing price);
        void UpdatePricee(pricing price);
        // void AddEntryFiles(product_files productFiles);
        int DeletePrice(int id);


        //delivery
        int AddDelivery(delivery deliver);
        delivery GetDelivery(int DeliveryId);
        IQueryable<delivery> GetAllDelivery();
        void UpdateDelivery(delivery deliver);
        // void AddEntryFiles(product_files productFiles);
        int DeleteDelivery(int id);


        //billing

        int AddBilling(billing bill);
        billing GetBilling(int BillId);
        IQueryable<billing> GetAllBilling();
        void UpdateBilling(billing bill);
        // void AddEntryFiles(product_files productFiles);
        int DeleteBilling(int id);


    }
}
