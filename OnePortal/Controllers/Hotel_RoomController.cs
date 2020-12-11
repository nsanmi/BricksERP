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
using System.IO;
using Microsoft.SqlServer.Server;
using OnePortal.Models.ViewModels;

namespace OnePortal.Controllers
{
    public class Hotel_RoomController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();

        // GET: Hotel_Room
        public async Task<ActionResult> Index()
        {
            var hotel_Room = db.Hotel_Room.Include(h => h.Hotel_promo).Include(h => h.Hotel_Room_Type);
            return View(await hotel_Room.ToListAsync());
        }

        // GET: Hotel_Room/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_Room hotel_Room = await db.Hotel_Room.FindAsync(id);
            if (hotel_Room == null)
            {
                return HttpNotFound();
            }
            return View(hotel_Room);
        }

        public ActionResult Make(ImageVModel image)
        {
            string filename = Path.GetFileNameWithoutExtension(image.ImageFile.FileName);
            string extension = Path.GetExtension(image.ImageFile.FileName);
            filename = filename + DateTime.Now.ToString("0:dd-MM-yyyy_hh_mm_ss") + extension;
            //image.RoomImage1 = "~/Documents/Room/" + filename.Replace(':', '-');

            filename = Path.Combine(Server.MapPath("~/Documents/Room/"), filename.Replace(':', '-'));
            image.RoomImage1 = filename.Replace(':', '-');
            image.ImageFile.SaveAs(filename);
            var firstimage = filename;

            string filename2 = Path.GetFileNameWithoutExtension(image.ImageFile2.FileName);
            string extension2 = Path.GetExtension(image.ImageFile2.FileName);
            filename2 = filename2 + DateTime.Now.ToString("0:dd-MM-yyyy_hh_mm_ss") + extension2;
            //image.RoomImage2 = "~/Documents/Room/" + filename2.Replace(':', '-');

            filename2 = Path.Combine(Server.MapPath("~/Documents/Room/"), filename2.Replace(':', '-'));
            image.RoomImage2 = filename2.Replace(':', '-');
            image.ImageFile.SaveAs(filename2);
            var secondimage = filename2;

            var room = new Hotel_Room
            {
                Room_number = image.Room_number,
                room_typeid = image.room_typeid,
                RoomImage1 = firstimage,
                RoomImage2 = secondimage,
                //RoomImage1 = Path.Combine(Server.MapPath("~/Documents/Room/"), filename.Replace(':', '-')),
                //RoomImage2 = Path.Combine(Server.MapPath("~/Documents/Room/"), filename2.Replace(':', '-')),
                created_date = DateTime.Now,
                room_category = image.room_category,
                rate = image.rate,
                Capacity = image.Capacity,
                status = 0,
                max_adults = image.max_adults,
                max_children = image.max_children,
                max_people =  image.max_people,
                min_people = image.min_people,
                Room_Description = image.Room_Description
                 
        };


            db.Hotel_Room.Add(room);
            db.SaveChanges();
            return RedirectToAction("Index");

        }


        // GET: Hotel_Room/Create
        public ActionResult Create()
        {
            ViewBag.promo_id = new SelectList(db.Hotel_promo, "Id", "Promo_type");
            ViewBag.room_typeid = new SelectList(db.Hotel_Room_Type, "RoomTypeId", "RoomType");
            return View();
        }

        // POST: Hotel_Room/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Room_number,Hotel_id,Location_id,room_type,room_typeid,room_category,status,rate,promo_id,created_date,updated_date,Capacity,RoomImage1,RoomImage2,RoomImage3,RoomImage4,RoomImage5,Room_Description,max_children,max_adults,max_people,min_people,start_lock,end_lock")] Hotel_Room hotel_Room, ImageVModel image)
        {

            //var room = new Hotel_Room
            //     {
            //   RoomImage5 = image.ImageFile,

            //     };


            //string filename = Path.GetFileNameWithoutExtension(image.ImageFile.FileName);
            //string extension = Path.GetExtension(image.ImageFile.FileName);
            //filename = filename + DateTime.Now.ToString("0:dd-MM-yyyy_hh_mm_ss") + extension;
            //hotel_Room.RoomImage5 = "~/Documents/Room/" + filename;

            //filename = Path.Combine(Server.MapPath("~/Documents/Room/"), filename);
            //image.ImageFile.SaveAs(filename);

            //db.Hotel_Room.Add(hotel_Room);
            //await db.SaveChangesAsync();
            //return RedirectToAction("Index"); 


            ViewBag.promo_id = new SelectList(db.Hotel_promo, "Id", "Promo_type", hotel_Room.promo_id);
            ViewBag.room_typeid = new SelectList(db.Hotel_Room_Type, "RoomTypeId", "RoomType", hotel_Room.room_typeid);
            return View(hotel_Room);
        }

        // GET: Hotel_Room/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_Room hotel_Room = await db.Hotel_Room.FindAsync(id);
            if (hotel_Room == null)
            {
                return HttpNotFound();
            }
            ViewBag.promo_id = new SelectList(db.Hotel_promo, "Id", "Promo_type", hotel_Room.promo_id);
            ViewBag.room_typeid = new SelectList(db.Hotel_Room_Type, "RoomTypeId", "RoomType", hotel_Room.room_typeid);
            return View(hotel_Room);
        }

        // POST: Hotel_Room/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Room_number,Hotel_id,Location_id,room_type,room_typeid,room_category,status,rate,promo_id,created_date,updated_date,Capacity,RoomImage1,RoomImage2,RoomImage3,RoomImage4,RoomImage5,Room_Description,max_children,max_adults,max_people,min_people,start_lock,end_lock")] Hotel_Room hotel_Room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_Room).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.promo_id = new SelectList(db.Hotel_promo, "Id", "Promo_type", hotel_Room.promo_id);
            ViewBag.room_typeid = new SelectList(db.Hotel_Room_Type, "RoomTypeId", "RoomType", hotel_Room.room_typeid);
            return View(hotel_Room);
        }

        // GET: Hotel_Room/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel_Room hotel_Room = await db.Hotel_Room.FindAsync(id);
            if (hotel_Room == null)
            {
                return HttpNotFound();
            }
            return View(hotel_Room);
        }

        // POST: Hotel_Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Hotel_Room hotel_Room = await db.Hotel_Room.FindAsync(id);
            db.Hotel_Room.Remove(hotel_Room);
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
