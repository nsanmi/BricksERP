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
    public class Hotel_amenityController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();

        // GET: Hotel_amenity
        public async Task<ActionResult> Index()
        {
            var hotel_amenity = db.Hotel_amenity.Include(h => h.Hotel_Room);
            return View(await hotel_amenity.ToListAsync());
        }

        // GET: Hotel_amenity/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_amenity hotel_amenity = await db.Hotel_amenity.FindAsync(id);
            if (hotel_amenity == null)
            {
                return HttpNotFound();
            }
            return View(hotel_amenity);
        }

        // GET: Hotel_amenity/Create
        public ActionResult Create()
        {
            ViewBag.room_id = new SelectList(db.Hotel_Room, "Id", "Room_number");
            return View();
        }

        // POST: Hotel_amenity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,room_id,amenity_name,amenity_category,amenity_status,amenity_description")] Hotel_amenity hotel_amenity)
        {
            if (ModelState.IsValid)
            {
                db.Hotel_amenity.Add(hotel_amenity);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.room_id = new SelectList(db.Hotel_Room, "Id", "Room_number", hotel_amenity.room_id);
            return View(hotel_amenity);
        }

        // GET: Hotel_amenity/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_amenity hotel_amenity = await db.Hotel_amenity.FindAsync(id);
            if (hotel_amenity == null)
            {
                return HttpNotFound();
            }
            ViewBag.room_id = new SelectList(db.Hotel_Room, "Id", "Room_number", hotel_amenity.room_id);
            return View(hotel_amenity);
        }

        // POST: Hotel_amenity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,room_id,amenity_name,amenity_category,amenity_status,amenity_description")] Hotel_amenity hotel_amenity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_amenity).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.room_id = new SelectList(db.Hotel_Room, "Id", "Room_number", hotel_amenity.room_id);
            return View(hotel_amenity);
        }

        // GET: Hotel_amenity/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_amenity hotel_amenity = await db.Hotel_amenity.FindAsync(id);
            if (hotel_amenity == null)
            {
                return HttpNotFound();
            }
            return View(hotel_amenity);
        }

        // POST: Hotel_amenity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Hotel_amenity hotel_amenity = await db.Hotel_amenity.FindAsync(id);
            db.Hotel_amenity.Remove(hotel_amenity);
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
