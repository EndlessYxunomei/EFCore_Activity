using InventoryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCore_DBLibrary
{
    public class InventoryDbContext : DbContext
    {
        //переменная для конфигурации
        private static IConfigurationRoot _configuration;

        //пока нет пользователей сделаем константу
        private const string _systemUserId = "2df28110-93d0-427d-9207-d55dbca680fa";

        //созданеи таблиц
        public DbSet<Item> Items { get; set; }

        //пустой конструктор для возможности scaffold базы данных
        public InventoryDbContext() { }
        //конструктор для dependensy injection
        public InventoryDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //На случай, если мы не предаём настроек, то прописываем значение по умолчанию
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
        public override int SaveChanges()
        {
            //подключаемся к теркеру изменений
            var tracker = ChangeTracker;
            foreach (var entry in tracker.Entries())
            {
                //проверяем есть ли у записи аудитные поля
                if (entry.Entity is FullAuditModel)
                {
                    var referenceEntity = entry.Entity as FullAuditModel;
                    //в зависимости от состояния добовляем нужные данные для аудита
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            {
                                //если запись новая, то добавляем дату создания
                                referenceEntity.CreatedDate = DateTime.Now;
                                //если не указан создатель, то берем по умолчанию
                                if (string.IsNullOrWhiteSpace(referenceEntity.CreatedByUserId))
                                {
                                    referenceEntity.CreatedByUserId = _systemUserId;
                                }
                                break;
                            }
                        case EntityState.Modified:
                            {
                                //вносим дату измененеия
                                referenceEntity.LastModifiedDate = DateTime.Now;
                                //если не указан пользователь, то берем по умолчанию
                                if (string.IsNullOrWhiteSpace(referenceEntity.LastModifiedUserId))
                                {
                                    referenceEntity.LastModifiedUserId = _systemUserId;
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
                //System.Diagnostics.Debug.WriteLine($"{entry.Entity} has state {entry.State}");
            }
            return base.SaveChanges();
        }
    }
}