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
    public class Hotel_AddOnsController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();

        // GET: Hotel_AddOns
        public async Task<ActionResult> Index()
        {
            var hotel_AddOns = db.Hotel_AddOns.Include(h => h.Hotel_Hotel_Information).Include(h => h.Hotel_promo);
            return View(await hotel_AddOns.ToListAsync());
        }

        // GET: Hotel_AddOns/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_AddOns hotel_AddOns = await db.Hotel_AddOns.FindAsync(id);
            if (hotel_AddOns == null)
            {
                return HttpNotFound();
            }
            return View(hotel_AddOns);
        }

        // GET: Hotel_AddOns/Create
        public ActionResult Create()
        {
            ViewBag.Hotel_id = new SelectList(db.Hotel_Hotel_Information, "Id", "name");
            ViewBag.promo_id = new SelectList(db.Hotel_promo, "Id", "Promo_type");
            return View();
        }

        // POST: Hotel_AddOns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Hotel_id,name,type,rate,promo_id,status,deleted")] Hotel_AddOns hotel_AddOns)
        {
            if (ModelState.IsValid)
            {
                db.Hotel_AddOns.Add(hotel_AddOns);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Hotel_id = new SelectList(db.Hotel_Hotel_Information, "Id", "name", hotel_AddOns.Hotel_id);
            ViewBag.promo_id = new SelectList(db.Hotel_promo, "Id", "Promo_type", hotel_AddOns.promo_id);
            return View(hotel_AddOns);
        }

        // GET: Hotel_AddOns/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_AddOns hotel_AddOns = await db.Hotel_AddOns.FindAsync(id);
            if (hotel_AddOns == null)
            {
                return HttpNotFound();
            }
            ViewBag.Hotel_id = new SelectList(db.Hotel_Hotel_Information, "Id", "name", hotel_AddOns.Hotel_id);
            ViewBag.promo_id = new SelectList(db.Hotel_promo, "Id", "Promo_type", hotel_AddOns.promo_id);
            return View(hotel_AddOns);
        }

        // POST: Hotel_AddOns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Hotel_id,name,type,rate,promo_id,status,deleted")] Hotel_AddOns hotel_AddOns)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_AddOns).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Hotel_id = new SelectList(db.Hotel_Hotel_Information, "Id", "name", hotel_AddOns.Hotel_id);
            ViewBag.promo_id = new SelectList(db.Hotel_promo, "Id", "Promo_type", hotel_AddOns.promo_id);
            return View(hotel_AddOns);
        }

        // GET: Hotel_AddOns/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_AddOns hotel_AddOns = await db.Hotel_AddOns.FindAsync(id);
            if (hotel_AddOns == null)
            {
                return HttpNotFound();
            }
            return View(hotel_AddOns);
        }

        // POST: Hotel_AddOns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Hotel_AddOns hotel_AddOns = await db.Hotel_AddOns.FindAsync(id);
            db.Hotel_AddOns.Remove(hotel_AddOns);
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
