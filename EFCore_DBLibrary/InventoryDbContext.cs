using Microsoft.EntityFrameworkCore;

namespace EFCore_DBLibrary
{
    public class InventoryDbContext : DbContext
    {
        //пустой конструктор для возможности scuffold базы данных
        public InventoryDbContext() { }
        //конструктор для dependensy injection
        public InventoryDbContext(DbContextOptions options) : base(options) { }
    }
}