using AutoMapper;
using InventoryModels;
using InventoryModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagerUnitTests
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
            CreateMap<Item, ItemDTO>().ReverseMap();
            CreateMap<CategoryDetail, CategoryDetailDTO>()
                .ForMember(x => x.Color, opt => opt.MapFrom(y => y.ColorName))
                .ForMember(x => x.Value, opt => opt.MapFrom(y => y.ColorValue))
                .ReverseMap()
                .ForMember(y => y.ColorValue, opt => opt.MapFrom(x => x.Value))
                .ForMember(y => y.ColorName, opt => opt.MapFrom(x => x.Color));
            CreateMap<Category, CategoryDTO>()
                .ForMember(x => x.Category, opt => opt.MapFrom(y => y.Name))
                //ВАЖНО БЛЯДЬ!!!!!
                //Я НЕ ЗНАЮ НАХРЕН НУЖЕН ЭТОТ АВТОМЕПЕР ЕСЛИ ОН НЕ МОЖЕНТ САМ БЛЯДЬ ПОНЯТЬ ЧТО ДЛЯ ДТОШКИ НУЖНО БРАТЬ ОБЪЕКТ КОТОРЫЙ УЖЕ БЫЛ УКАЗАН РАНЕЕ
                .ForMember(x => x.CategotyDetail, opt => opt.MapFrom(y => y.CategoryDetail))
                .ReverseMap()
                .ForMember(y => y.Name, opt => opt.MapFrom(x => x.Category));
            CreateMap<Item,CreateOrUpdateItemDTO>()
                .ReverseMap()
                .ForMember(x => x.Category, opt => opt.Ignore());
        }
    }
}
