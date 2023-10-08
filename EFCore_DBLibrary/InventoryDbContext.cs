using InventoryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCore_DBLibrary
{
    public class InventoryDbContext : DbContext
    {
        //переменная для конфигурации
        private static IConfigurationRoot _configuration;
        
        //созданеи таблиц
        public DbSet<Item> Items { get; set; }

        //пустой конструктор для возможности scuffold базы данных
        public InventoryDbContext() { }
        //конструктор для dependensy injection
        public InventoryDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                _configuration = builder.Build();
                var cnstr = _configuration.GetConnectionString("InventoryManager");
                
                optionsBuilder.UseSqlServer(cnstr);
            }
        }
    }
}