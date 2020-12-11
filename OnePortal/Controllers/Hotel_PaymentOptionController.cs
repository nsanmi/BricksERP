using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Hotel.Dal.Models;

namespace OnePortal.Controllers
{
    public class Hotel_PaymentOptionController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();

        // GET: Hotel_PaymentOption
        public async Task<ActionResult> Index()
        {
            return View(await db.Hotel_PaymentOption.ToListAsync());
        }

        // GET: Hotel_PaymentOption/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_PaymentOption hotel_PaymentOption = await db.Hotel_PaymentOption.FindAsync(id);
            if (hotel_PaymentOption == null)
            {
                return HttpNotFound();
            }
            return View(hotel_PaymentOption);
        }

        // GET: Hotel_PaymentOption/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hotel_PaymentOption/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Type,Options,Description,Status,deleted")] Hotel_PaymentOption hotel_PaymentOption)
        {
            if (ModelState.IsValid)
            {
                db.Hotel_PaymentOption.Add(hotel_PaymentOption);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(hotel_PaymentOption);
        }

        // GET: Hotel_PaymentOption/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_PaymentOption hotel_PaymentOption = await db.Hotel_PaymentOption.FindAsync(id);
            if (hotel_PaymentOption == null)
            {
                return HttpNotFound();
            }
            return View(hotel_PaymentOption);
        }

        // POST: Hotel_PaymentOption/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Type,Options,Description,Status,deleted")] Hotel_PaymentOption hotel_PaymentOption)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_PaymentOption).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(hotel_PaymentOption);
        }

        // GET: Hotel_PaymentOption/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_PaymentOption hotel_PaymentOption = await db.Hotel_PaymentOption.FindAsync(id);
            if (hotel_PaymentOption == null)
            {
                return HttpNotFound();
            }
            return View(hotel_PaymentOption);
        }

        // POST: Hotel_PaymentOption/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Hotel_PaymentOption hotel_PaymentOption = await db.Hotel_PaymentOption.FindAsync(id);
            db.Hotel_PaymentOption.Remove(hotel_PaymentOption);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
