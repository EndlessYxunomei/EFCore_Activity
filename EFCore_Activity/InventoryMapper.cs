using AutoMapper;
using InventoryModels;
using InventoryModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore_Activity
{
    //Класс конфигурации автомапера. Пока одни на всё, но по жизни нужно больше
    public class InventoryMapper: Profile
    {
        public InventoryMapper()
        {
            CreateMaps();
        }
        private void CreateMaps()
        {
            CreateMap<Item, ItemDTO>();
            CreateMap<Category, CategoryDTO>();
        }
    }
}
