using EFCore_DBLibrary;
using InventoryHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InventoryDataMigrator
{
    //весь проект для запуска миграций отдельно от остального проекта
    internal class Program
    {
        static IConfigurationRoot _configuration;
        static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder;

        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
        }

        static void Main(string[] args)
        {
            BuildOptions();
            ApplyMigrations();
            ExecuteCustomSeedData();
        }
        private static void ApplyMigrations()
        {
            //метод для миграций
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                db.Database.Migrate();
            }
        }
        private static void ExecuteCustomSeedData()
        {
            //метод для введения сведений в миграции
            using (var context = new InventoryDbContext(_optionsBuilder.Options))
            {
                //отдельный класс для категорий
                var categories = new BuildCategories(context);
                categories.ExecuteSeed();
                var items = new BuildItems(context);
                items.ExecuteSeed();
            }
        }
    }
}