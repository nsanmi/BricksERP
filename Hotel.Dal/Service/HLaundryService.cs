using Hotel.Dal.Models;
using Hotel.Dal.IService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hotel.Dal.Service
{
    public class HLaundryService: IHLaundryService
    {
        readonly HotelDBEntities _context = new HotelDBEntities();

        public List<string> GetLaundryAdmin()
        {
            var productsAdmin = _context.product_admin.Where(m => m.active == 1).Select(m => m.admin_hrm_employee.emp_work_email).ToList();
            return productsAdmin;
        }

        public List<string> ResolvedNotification()
        {
            //var userEmail =   _context.admin_hrm_employee.Where(x => x.u)
            var notification = _context.admin_hrm_employee.Select(m => m.emp_work_email).ToList();
            return notification;

        }


        //public IQueryable<collection_register> GetMonthlyAudit(int year, int month)
        //{
        //    return _context.collection_register.Where(e => e.collection_date.Year == year && e.collection_date.Month == month);
        //}


        public int AddEntry(collection_register register)
        {
            //var id = 0;
            register.collection_date = DateTime.Now;
            _context.collection_register.Add(register);

            _context.SaveChanges();
            return register.id;
        }

        public int AddCollection(entry_information collection)
        {
            var id = 0;
          _context.entry_information.Add(collection);

            _context.SaveChanges();
            return collection.id;
        }

        public void AddCol(List<entry_information> col)
        {
            _context.entry_information.AddRange(col);
            _context.SaveChanges();
        }

        public collection_register GetEntry(int entryId)
        {
            return _context.collection_register.FirstOrDefault(m => m.id == entryId);
        }

        public void UpdateEntry(collection_register register)
        {
            var register_old = _context.collection_register.First(e => e.id == register.id);

            register_old.quantity = register.quantity;
            register_old.collection_date = DateTime.Now;
            _context.Entry(register_old).State = EntityState.Modified;
            _context.SaveChanges();

        }
        public IQueryable<collection_register> GetAllEntry()
        {

            return _context.collection_register.AsQueryable();
        }

        //public void AddEntryFiles(product_files productsFiles)
        //{
        //    _context.product_files.Add(productsFiles);

        //    _context.SaveChanges();
        //}

        public int DeleteEntry(int id)
        {
            var existingentry = _context.collection_register.First(m => m.id == id);
            existingentry.Deleted = 1;
            _context.Entry(existingentry).State = EntityState.Modified;
            //_context.Entry  (existingproducts).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingentry.id;
        }


        // Pricing

        public int AddPricing(pricing price)
        {

            _context.pricings.Add(price);

            _context.SaveChanges();
            return price.id;
        }

        public pricing GetPrice(int id)
        {
            var a = _context.pricings.FirstOrDefault(e => e.id == id);
            return _context.pricings.FirstOrDefault(m => m.id == id);
        }

        public void UpdatePricee(pricing price)
        {
            var price_old = _context.pricings.FirstOrDefault(e => e.id == price.id);
            if (price_old != null)
            {
                _context.Entry(price_old).CurrentValues.SetValues(price);
                _context.SaveChanges();
            }
        }

        public void UpdatePrice(pricing price)
        {

            var price_old = _context.pricings.First(e => e.id == price.id);
            price_old.Deleted = 0;
            price_old.ClothName = price.ClothName;
            price_old.clotheType = price.clotheType;
            price_old.Description = price.Description;
            price_old.washing_price = price.washing_price;
            //price_old.created_date =  DateTime.Now;
            price.updated_date = DateTime.Now;
            price.Deleted = 0;
            price.created_date = price_old.created_date;
            //price_old.updated_date = DateTime.Now;
            _context.Entry(price_old).State = EntityState.Modified;
            _context.SaveChanges();

        }
        public IQueryable<pricing> GetAllPrice()
        {

            return _context.pricings.AsQueryable();
        }



        public int DeletePrice(int id)
        {
            var existingprice = _context.pricings.First(m => m.id == id);
            existingprice.Deleted = 1;
            _context.Entry(existingprice).State = EntityState.Modified;
            //_context.Entry  (existingproducts).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingprice.id;
        }

        //Delivery
        public int AddDelivery(delivery deliver)
        {

            _context.deliveries.Add(deliver);

            _context.SaveChanges();
            return deliver.id;
        }

        public delivery GetDelivery(int deliveryId)
        {
            return _context.deliveries.FirstOrDefault(m => m.id == deliveryId);
        }

        public void UpdateDelivery(delivery deliver)
        {
            var delivery_old = _context.deliveries.First(e => e.id == deliver.id);

            delivery_old.description = deliver.description;
            //delivery_old.pricing = deliver.pricing;
            delivery_old.finished_date = DateTime.Now;
            delivery_old.updated_date = DateTime.Now;
            _context.Entry(delivery_old).State = EntityState.Modified;
            _context.SaveChanges();

        }
        public IQueryable<delivery> GetAllDelivery()
        {

            return _context.deliveries.AsQueryable();
        }



        public int DeleteDelivery(int id)
        {
            var existingdelivery = _context.deliveries.First(m => m.id == id);
            existingdelivery.Deleted = 1;
            _context.Entry(existingdelivery).State = EntityState.Modified;
            //_context.Entry  (existingproducts).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingdelivery.id;
        }

        //Billing
        public int AddBilling(billing bill)
        {

            _context.billings.Add(bill);

            _context.SaveChanges();
            return bill.billing_id;
        }

        public billing GetBilling(int billId)
        {
            return _context.billings.FirstOrDefault(m => m.billing_id == billId);
        }

        public void UpdateBilling(billing bill)
        {
            var bill_old = _context.billings.First(e => e.billing_id == bill.billing_id);

            bill_old.bill_amount = bill.bill_amount;
            bill_old.quantity = bill.quantity;
            //bill_old. = DateTime.Now;
            //bill_old.updated_date = DateTime.Now;
            _context.Entry(bill_old).State = EntityState.Modified;
            _context.SaveChanges();

        }
        public IQueryable<billing> GetAllBilling()
        {

            return _context.billings.AsQueryable();
        }



        public int DeleteBilling(int id)
        {
            var existingbill = _context.billings.First(m => m.billing_id == id);
            existingbill.Deleted = 1;
            _context.Entry(existingbill).State = EntityState.Modified;
            //_context.Entry  (existingproducts).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingbill.billing_id;
        }


    }
}
