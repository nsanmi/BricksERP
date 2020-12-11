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
    public class Hotel_ReservationTypeController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();

        // GET: Hotel_ReservationType
        public async Task<ActionResult> Index()
        {
            return View(await db.Hotel_ReservationType.ToListAsync());
        }

        // GET: Hotel_ReservationType/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_ReservationType hotel_ReservationType = await db.Hotel_ReservationType.FindAsync(id);
            if (hotel_ReservationType == null)
            {
                return HttpNotFound();
            }
            return View(hotel_ReservationType);
        }

        // GET: Hotel_ReservationType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hotel_ReservationType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Type_name,Type_description,status,deleted")] Hotel_ReservationType hotel_ReservationType)
        {
            if (ModelState.IsValid)
            {
                db.Hotel_ReservationType.Add(hotel_ReservationType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(hotel_ReservationType);
        }

        // GET: Hotel_ReservationType/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_ReservationType hotel_ReservationType = await db.Hotel_ReservationType.FindAsync(id);
            if (hotel_ReservationType == null)
            {
                return HttpNotFound();
            }
            return View(hotel_ReservationType);
        }

        // POST: Hotel_ReservationType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Type_name,Type_description,status,deleted")] Hotel_ReservationType hotel_ReservationType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_ReservationType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(hotel_ReservationType);
        }

        // GET: Hotel_ReservationType/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_ReservationType hotel_ReservationType = await db.Hotel_ReservationType.FindAsync(id);
            if (hotel_ReservationType == null)
            {
                return HttpNotFound();
            }
            return View(hotel_ReservationType);
        }

        // POST: Hotel_ReservationType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Hotel_ReservationType hotel_ReservationType = await db.Hotel_ReservationType.FindAsync(id);
            db.Hotel_ReservationType.Remove(hotel_ReservationType);
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
