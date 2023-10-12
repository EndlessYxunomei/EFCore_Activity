using InventoryModels.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryModels
{
    //В для Player мы делали связь много-много через FluentAPI в InventoryDbContext через OnModelCreating
    //теперь попробуем создать отдельный класс и использовать аннотации
    [Table("ItemGenres")]
    [Index(nameof(ItemId), nameof(GenreId), IsUnique=true)]
    public class ItemGenre : IIdentityModel
    {
        public int Id { get; set; }
        public virtual int ItemId { get; set; }
        public virtual Item Item { get; set; }
        public virtual int GenreId { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
