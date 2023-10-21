using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCore_DBLibrary;
using EFCore_DBLibrary.Migrations;
using InventoryBusinessLayer;
using InventoryHelpers;
using InventoryModels;
using InventoryModels.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore_Activity
{
    public class Program
    {
        //конфигурация базы данных
        private static IConfigurationRoot _configuration;
        private static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder;

        //Конфигурация автомапера
        private static MapperConfiguration _mapperConfig;
        private static IMapper _mapper;
        private static IServiceProvider _serviceProvider;

        //конфигурация сервисов из InventoryBuisnesLayer
        private static IItemsService _itemsService;
        private static ICategoriesService _categoriesService;

        //пока нет нормальных пользователей заменяем их константой
        private const string _systemUserId = "2df28110-93d0-427d-9207-d55dbca680fa";
        private const string _loggedInUserID = "e2eb8989-a81a-4151-8e86-eb95-a7961da2";
        
        static void Main(string[] args)
        {
            BuildOptions();
            BuildMapper();

            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                _itemsService = new ItemsService(db, _mapper);
                _categoriesService = new CategoriesService(db, _mapper);

                ListInventory();
                GetItemsForListing();
                GetAllActiveItemsAsPipeDelimitedString();
                GetItemsTotalValues();
                GetFullItemDetails();
                GetItemsForListingLinq();
                ListCategoriesAndColors();
            }

            //DeleteAllItems();
            //EnsureItems();
            //UpdateItems();

            //ListInventory();
            //ListInventoryWithProjections();
            //ListCategoriesAndColors();
            //GetItemsForListing();
            //GetItemsForListingLinq();
            //GetAllActiveItemsAsPipeDelimitedString();
            //GetItemsTotalValues();
            //GetFullItemDetails();
        }
        //методы для настройки базы данный и автомапера
        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
        }
        static void BuildMapper()
        {
            //Встариваем сервис автомапера
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(InventoryMapper));
            _serviceProvider = services.BuildServiceProvider();
            //настраиваем автомапер
            _mapperConfig = new MapperConfiguration(cfg => { cfg.AddProfile<InventoryMapper>(); });
            _mapperConfig.AssertConfigurationIsValid();
            _mapper = _mapperConfig.CreateMapper();
        }

        //старые методы для заполнения базы
        /*static void EnsureItems()
        {
            EnsureItem("Batman Begins", "Как милиардер стал мутузить людей на улице", "Кристиан Бейл");
            EnsureItem("Inception", "Сны бывают очень запутанные", "Лёнчик Дикаприо");
            EnsureItem("Remember te Titans", "Хрень", "Дензел вашингтон");
            EnsureItem("Star Wars: The Empire Strikes Back", "В далекой, далекой галактике опять неспокойно", "Харисон форд, Марк Хемил, Чубака");
            EnsureItem("Top Gun", "Упоротый лётчик", "Том Круз");
        }
        private static void EnsureItem(string name, string description, string notes)
        {
            Random r = new Random();//случайное чисто для количечества
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                //опрееляем есть ли подобные записи
                var existingItem = db.Items.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
                if (existingItem == null)
                {
                    //если нет, то создаем новый объект и добавлем его
                    var item = new Item() { Name = name, CreatedByUserId = _loggedInUserID, IsActive = true, Quantity = r.Next(1,1000),
                        Description = description, Notes = notes};
                    db.Items.Add(item);
                    db.SaveChanges();
                }
            }
        }
        private static void DeleteAllItems()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var items = db.Items.ToList();
                db.Items.RemoveRange(items);
                db.SaveChanges();
            }
        }
        private static void UpdateItems()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var items = db.Items.ToList();
                foreach (var item in items)
                {
                    item.CurrentOrFinalPrice = 9.99M;
                }
                db.Items.UpdateRange(items);
                db.SaveChanges();
            }
        }
        */

        //методы для работы с базой
        private static void ListInventory()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var result = _itemsService.GetItems();
                result.ForEach(x => Console.WriteLine($"New item: {x}"));

                //НЕ РАБОТАЕТ КАК В КНИГЕ ХОТЬ УБЕЙ
                //Приходится добавлять ToList в начале
                /*var result = db.Items.ToList().OrderBy(x => x.Name).Take(20)
                    .Select(x => new ItemDTO
                    {
                        Name = x.Name,
                        Description = x.Description
                    }).ToList();
                result.ForEach(x => Console.WriteLine($"New item: {x}"));*/
                /*var items = db.Items.OrderBy(x => x.Name).ToList();
                //внедряем использование автомапера c ручным меппингом после выпонения запроса
                var result = _mapper.Map<List<Item>,List<ItemDTO>>(items);
                result.ForEach(x => Console.WriteLine($"New item: {x}"));
                //items.ForEach(x => Console.WriteLine($"New Item: {x.Name}"));*/
            }
        }
        private static void ListInventoryWithProjections()
        {
            //автомапер с проекцией прямо в запросе
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var iems = db.Items
                    //отключили из-за шифрования
                    //.OrderBy(x => x.Name)
                    .ProjectTo<ItemDTO>(_mapper.ConfigurationProvider)
                    .ToList();
                //iems.ForEach(x => Console.WriteLine($"New Item: {x}"));
                //из-за шифрования приходится делать сортировки и пр после получения всех данных с сревера и дешифровки - УДАР ПО ПРОИЗВОДИТЕЛЬНОСТИ
                iems.OrderBy(x => x.Name).ToList().ForEach(x => Console.WriteLine($"New Item: {x}"));
            }
        }
        private static void ListCategoriesAndColors()
        {
            var results = _categoriesService.ListCategoriesAndDetails();
            foreach (var c in results)
            {
                Console.WriteLine($"Category [{c.Category}] is {c.CategotyDetail?.Color}");
            }

            //изначальная версия
            /*using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.Categories
                    .Include(x => x.CategoryDetail)
                    .ProjectTo<CategoryDTO>(_mapper.ConfigurationProvider).ToList();
                foreach (var c in results)
                {
                    Console.WriteLine($"Category [{c.Category}] is {c.CategotyDetail?.Color}");
                }
            }*/
            //проверка мапинга с изменеными названиями
            /*using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.Categories
                .Include(x => x.CategoryDetail)
                //.Select(x => x.CategoryDetail)
                .ProjectTo<CategoryDTO>(_mapper.ConfigurationProvider).ToList();

                foreach (var c in results)
                {
                    Console.WriteLine($"Category [{c.Category}] is {c.CategotyDetail?.Color}");
                }
            }*/
        }
        private static void GetItemsForListing()
        {
            var results = _itemsService.GetItemsForListingFromProcedure();
            foreach (var item in results)
            {
                var output = $"ITEM {item.Name}] {item.Description}";
                if (!string.IsNullOrWhiteSpace(item.CategoryName))
                {
                    output = $"{output} has category: {item.CategoryName}";
                }
                Console.WriteLine(output);
            }
            
            /*using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                //используем ДТОшку вместо прямого вывода, чтобы избежать ошибок
                //var results = db.Items.FromSqlRaw("EXECUTE dbo.GetItemsForListing").ToList();
                var results = db.ItemsForListing.FromSqlRaw("EXECUTE dbo.GetItemsForListing").ToList();
                foreach (var item in results)
                {
                    //не работает та как нет id в выходных данных встроенной процедуры
                    //Console.WriteLine($"ITEM {item.Id} {item.Name}");
                    var output = $"ITEM {item.Name}] {item.Description}";
                    if (!string.IsNullOrWhiteSpace(item.CategoryName))
                    {
                        output = $"{output} has category: {item.CategoryName}";
                    }

                    Console.WriteLine(output);
                }
            }*/
        }
        private static void GetItemsForListingLinq()
        {
            var minDateValue = new DateTime(2021, 1, 1);
            var maxDateValue = new DateTime(2024, 1, 1);

            var results = _itemsService.GetItemsByDateRange(minDateValue, maxDateValue)
                .OrderBy(y => y.CategoryName).ThenBy(z => z.Name);
            foreach (var itemDto in results)
            {
                Console.WriteLine(itemDto);
            }

            //замена сохраненной процедуре выше чрез линк и анонимный класс
            /*using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.Items.Select(x => new
                {
                    x.CreatedDate,
                    CategoryName = x.Category.Name,
                    x.Description,
                    x.IsActive,
                    x.IsDeleted,
                    x.Name,
                    x.Notes
                }).Where(x => x.CreatedDate >= minDateValue && x.CreatedDate <= maxDateValue)
                .OrderBy(y => y.CategoryName).ThenBy(z => z.Name).ToList();
                foreach (var item in results)
                {
                    Console.WriteLine($"ITEM {item.CategoryName}| {item.Name} - {item.Description}");
                }
            }*/
            // Вариант с ДТОшкой
            /*using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.Items.Select(x => new ItemDTO
                {
                    CreatedDate = x.CreatedDate,
                    CategoryName = x.Category.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    Name = x.Name,
                    Notes = x.Notes,
                    CategoryId = x.Category.Id,
                    Id = x.Id
                }).Where(x => x.CreatedDate >= minDateValue && x.CreatedDate <= maxDateValue)
                    .OrderBy(y => y.CategoryName).ThenBy(z => z.Name).ToList();
                foreach (var itemDTO in results)
                {
                    Console.WriteLine(itemDTO);
                }
            }*/
            //Вариант для шифрования
            /*using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.Items.Include(x => x.Category).ToList().Select(x => new ItemDTO
                {
                    CreatedDate = x.CreatedDate,
                    CategoryName = x.Category.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    Name = x.Name,
                    Notes = x.Notes,
                    CategoryId = x.Category.Id,
                    Id = x.Id
                }).Where(x => x.CreatedDate >= minDateValue && x.CreatedDate <= maxDateValue)
                .OrderBy(y => y.CategoryName).ThenBy(z => z.Name)
                .ToList();
                foreach (var itemDTO in results)
                {
                    Console.WriteLine(itemDTO);
                }
            }*/
        }
        private static void GetAllActiveItemsAsPipeDelimitedString()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                Console.WriteLine($"All active Items: {_itemsService.GetAllItemsPipeDelimitedString()}");
                /*var isActiveParm = new SqlParameter("IsActive", 1);
                var result = db.AllItemsOutput
                    .FromSqlRaw("SELECT [dbo].[ItemNamesPipeDelimitedString] (@IsActive) AllItems", isActiveParm)
                    .FirstOrDefault();
                Console.WriteLine($"All active Items: {result.AllItems}");*/
                /*var result = db.Items.Where(x => x.IsActive).ToList();
                var pipeDelimitedString = string.Join("|", result);
                Console.WriteLine($"All active Items: {pipeDelimitedString}");*/
            }
        }
        private static void GetItemsTotalValues()
        {
            var results = _itemsService.GetItemsTotalValues(true);
            foreach (var item in results)
            {
                Console.WriteLine($"New Item] {item.Id,-10}" +
                    $" | {item.Name,-50}" +
                    $" | {item.Quantity,-4}" +
                    $" | {item.TotalValue,-5}");
            }
            /*using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var isActiveParm = new SqlParameter("IsActive", 1);
                var result = db.GetItemsTotalValues.FromSqlRaw("SELECT * FROM [dbo].[GetItemsTotalValue] (@IsActive)", isActiveParm).ToList();
                foreach (var item in result)
                {
                    Console.WriteLine($"New Item] {item.Id, -10}" + $" | {item.Name, -50}" + $" | {item.Quantity, -4}" + $" | {item.TotalValue, -5}");
                }
            }*/
        }
        private static void GetFullItemDetails()
        {
            var result = _itemsService.GetItemsWithGenresAndCategories();
            foreach (var item in  result)
            {
                Console.WriteLine($"New Item] {item.Id,-10}" +
                    $"|{item.ItemName,-50}" +
                    $"|{item.ItemDescription,-4}" +
                    $"|{item.PlayerName,-5}" +
                    $"|{item.Category,-5}" +
                    $"|{item.GenreName,-5}");
            }
            
            /*using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                //var result = db.FullItemsDetailDtos.FromSqlRaw("SELECT * FROM [dbo].[vwFullItemsDetails]").ToList();
                var result = db.FullItemsDetailDtos.FromSqlRaw("SELECT * FROM [dbo].[vwFullItemsDetails]").ToList()
                    .OrderBy(x => x.ItemName).ThenBy(x => x.GenreName).ThenBy(x => x.Category).ThenBy(x => x.PlayerName);
                foreach (var item in result)
                {
                    Console.WriteLine($"New Item] {item.Id, -10}" + $"|{item.ItemName, -50}" + $"|{item.ItemDescription, -4}" + $"|{item.PlayerName, -5}" + $"|{item.Category, -5}" + $"|{item.GenreName, -5}");
                }
            }*/
        }
    }
}