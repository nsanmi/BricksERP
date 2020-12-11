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
using OnePortal.Models.ViewModels;

namespace OnePortal.Controllers
{
    public class ReservationController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();


        // GET: Reservation
        public async Task<ActionResult> Index(int? year, int? month, int? category, int? category_id, string search)
        {
            var yr = DateTime.Now.Year;
            var mnth = DateTime.Now.Month;
            int cat = 1;
            int cate = 1;

            if (category_id.HasValue) cate = category_id.Value;

            if (category.HasValue)
            {
                cat = category.Value;

            }

            if (year.HasValue && month.HasValue)
            {
                yr = year.Value;
                mnth = month.Value;

            }


            
            //ViewBag.audits = _inventoryService.GetMonthlyAudit(yr, mnth);
            ViewBag.search = search;
            ViewBag.category = cat;
            ViewBag.month = mnth;
            ViewBag.year = yr;
            ViewBag.prcategory = cate;
            


            var hotel_reservation = db.Hotel_reservation.Include(h => h.Hotel_guest).Include(h => h.Hotel_PaymentOption).Include(h => h.Hotel_ReservationType).Include(h => h.Hotel_Room);
            return View(await hotel_reservation.ToListAsync());
        }

        // GET: Reservation/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_reservation hotel_reservation = await db.Hotel_reservation.FindAsync(id);
            if (hotel_reservation == null)
            {
                return HttpNotFound();
            }
            return View(hotel_reservation);
        }

        // GET: Reservation/Create
        public ActionResult Create()
        {
            ViewBag.guest_id = new SelectList(db.Hotel_guest, "Id", "First_name");
            ViewBag.paymentTypeId = new SelectList(db.Hotel_PaymentOption, "Id", "Type");
            ViewBag.reservation_typeId = new SelectList(db.Hotel_ReservationType, "Id", "Type_name");
            ViewBag.Room_id = new SelectList(db.Hotel_Room, "Id", "Room_number");
            ViewBag.room_types = new SelectList(db.Hotel_Room_Type, "RoomTypeId", "RoomType");
            return View();
        }

        // POST: Reservation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Room_id,guest_id,reservation_date,reservation_status,reservation_comment,reservation_checking_date,reservation_checkout_date,deleted,reservation_typeId,total,payment_date,payment_option,paymentTypeId,payment_status,session")] Hotel_reservation hotel_reservation)
        {
            if (ModelState.IsValid)
            {
                db.Hotel_reservation.Add(hotel_reservation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.guest_id = new SelectList(db.Hotel_guest, "Id", "First_name", hotel_reservation.guest_id);
            ViewBag.paymentTypeId = new SelectList(db.Hotel_PaymentOption, "Id", "Type", hotel_reservation.paymentTypeId);
            ViewBag.reservation_typeId = new SelectList(db.Hotel_ReservationType, "Id", "Type_name", hotel_reservation.reservation_typeId);
            ViewBag.Room_id = new SelectList(db.Hotel_Room, "Id", "Room_number", hotel_reservation.Room_id);
            return View(hotel_reservation);
        }

        // GET: Reservation/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_reservation hotel_reservation = await db.Hotel_reservation.FindAsync(id);
            if (hotel_reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.guest_id = new SelectList(db.Hotel_guest, "Id", "First_name", hotel_reservation.guest_id);
            ViewBag.paymentTypeId = new SelectList(db.Hotel_PaymentOption, "Id", "Type", hotel_reservation.paymentTypeId);
            ViewBag.reservation_typeId = new SelectList(db.Hotel_ReservationType, "Id", "Type_name", hotel_reservation.reservation_typeId);
            ViewBag.Room_id = new SelectList(db.Hotel_Room, "Id", "Room_number", hotel_reservation.Room_id);
            return View(hotel_reservation);
        }

        // POST: Reservation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Room_id,guest_id,reservation_date,reservation_status,reservation_comment,reservation_checking_date,reservation_checkout_date,deleted,reservation_typeId,total,payment_date,payment_option,paymentTypeId,payment_status,session")] Hotel_reservation hotel_reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_reservation).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.guest_id = new SelectList(db.Hotel_guest, "Id", "First_name", hotel_reservation.guest_id);
            ViewBag.paymentTypeId = new SelectList(db.Hotel_PaymentOption, "Id", "Type", hotel_reservation.paymentTypeId);
            ViewBag.reservation_typeId = new SelectList(db.Hotel_ReservationType, "Id", "Type_name", hotel_reservation.reservation_typeId);
            ViewBag.Room_id = new SelectList(db.Hotel_Room, "Id", "Room_number", hotel_reservation.Room_id);
            return View(hotel_reservation);
        }

        // GET: Reservation/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_reservation hotel_reservation = await db.Hotel_reservation.FindAsync(id);
            if (hotel_reservation == null)
            {
                return HttpNotFound();
            }
            return View(hotel_reservation);
        }

        // POST: Reservation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Hotel_reservation hotel_reservation = await db.Hotel_reservation.FindAsync(id);
            db.Hotel_reservation.Remove(hotel_reservation);
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

        [HttpPost]
        public JsonResult GetAvailableRooms(SearchRoom search)
        {
            db.Configuration.ProxyCreationEnabled = false;
            //select all available rooms between dates selected
            //var rooms_available = db.Hotel_Room.Where(n => n.room_typeid == search.room_type || n.Hotel_reservation.Any(m => m.reservation_checking_date <= search.check_in_date || m.reservation_checkout_date >= search.check_out_date));
            //var rooms_availabl = db.Hotel_Room.Where(n => n.room_typeid == search.room_type == n.Hotel_reservation.Any(m => m.reservation_checking_date <= search.check_in_date || m.reservation_checkout_date >= search.check_out_date));
            var rooms_available2 = db.Hotel_Room.Where(n => n.room_typeid == search.room_type == !n.Hotel_reservation.Any(m => m.reservation_checking_date >= search.check_in_date && m.reservation_checkout_date <= search.check_out_date));

            //var tt = rooms_available.ToList();
            var data = rooms_available2;
            var data2 = rooms_available2.ToList();


            //return Json(rooms_available.ToList());
            return Json(data2);

        }

        [HttpPost]
        public JsonResult GetAvailableRoom(SearchRoom search)
        {
            //select all available rooms between dates selected
            var sanmi = "name";

            return Json(sanmi);
        }
    }
}
