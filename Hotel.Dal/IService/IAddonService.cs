using Hotel.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Dal.IService
{
    public interface IAddonService
    {

        int AddAddon(Hotel_AddOns addon);
        Hotel_AddOns GetAddon(int addonId);
        IQueryable<Hotel_AddOns> GetAllAddon();
        void UpdateAddon(Hotel_AddOns addon);
        int DeleteAddon(int id);
    }
}
