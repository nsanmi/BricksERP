using Hotel.Dal.IService;
using Hotel.Dal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Dal.Service
{
    public class AddonService: IAddonService
    {
        readonly HotelDBEntities _context = new HotelDBEntities();

        public int AddAddon(Hotel_AddOns addon)
        {

            _context.Hotel_AddOns.Add(addon);

            _context.SaveChanges();
            return addon.Id;
        }

        public Hotel_AddOns GetAddon(int addonId)
        {
            return _context.Hotel_AddOns.FirstOrDefault(m => m.Id == addonId);
        }

        public void UpdateAddon(Hotel_AddOns addon)
        {
            var addon_old = _context.Hotel_AddOns.First(e => e.Id == addon.Id);

            addon_old.name = addon.name;
            addon_old.status = addon.status;
            addon_old.rate = addon.rate;
            addon_old.type = addon.type;
            _context.Entry(addon_old).State = EntityState.Modified;

            _context.SaveChanges();

        }
        public IQueryable<Hotel_AddOns> GetAllAddon()
        {

            return _context.Hotel_AddOns.AsQueryable();
        }

        //public void AddRoomFiles(ws_complain_files complainFiles)
        //{
        //    _context.ws_complain_files.Add(complainFiles);

        //    _context.SaveChanges();
        //}

        public int DeleteAddon(int id)
        {
            var existingAddon = _context.Hotel_AddOns.First(m => m.Id == id);
            existingAddon.status = 4;
            _context.Entry(existingAddon).State = EntityState.Modified;
            //_context.Entry(existingComplain).CurrentValues.SetValues(id);
            _context.SaveChanges();
            return existingAddon.Id;
        }

    }
}
