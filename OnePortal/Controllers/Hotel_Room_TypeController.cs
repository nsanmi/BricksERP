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
    public class Hotel_Room_TypeController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();

        // GET: Hotel_Room_Type
        public async Task<ActionResult> Index()
        {
            return View(await db.Hotel_Room_Type.ToListAsync());
        }

        // GET: Hotel_Room_Type/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_Room_Type hotel_Room_Type = await db.Hotel_Room_Type.FindAsync(id);
            if (hotel_Room_Type == null)
            {
                return HttpNotFound();
            }
            return View(hotel_Room_Type);
        }

        // GET: Hotel_Room_Type/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hotel_Room_Type/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RoomTypeId,RoomType,Description")] Hotel_Room_Type hotel_Room_Type)
        {
            if (ModelState.IsValid)
            {
                db.Hotel_Room_Type.Add(hotel_Room_Type);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(hotel_Room_Type);
        }

        // GET: Hotel_Room_Type/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_Room_Type hotel_Room_Type = await db.Hotel_Room_Type.FindAsync(id);
            if (hotel_Room_Type == null)
            {
                return HttpNotFound();
            }
            return View(hotel_Room_Type);
        }

        // POST: Hotel_Room_Type/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RoomTypeId,RoomType,Description")] Hotel_Room_Type hotel_Room_Type)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_Room_Type).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(hotel_Room_Type);
        }

        // GET: Hotel_Room_Type/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_Room_Type hotel_Room_Type = await db.Hotel_Room_Type.FindAsync(id);
            if (hotel_Room_Type == null)
            {
                return HttpNotFound();
            }
            return View(hotel_Room_Type);
        }

        // POST: Hotel_Room_Type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Hotel_Room_Type hotel_Room_Type = await db.Hotel_Room_Type.FindAsync(id);
            db.Hotel_Room_Type.Remove(hotel_Room_Type);
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
