using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryModels.DTOs
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string? Notes { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public override string ToString()
        {
            return $"ITEM {Name,-25}] {Description} has categoty: {CategoryName}";
        }

    }
}
