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

        private static List<CategoryDTO> _categories;

        //пока нет нормальных пользователей заменяем их константой
        private const string _systemUserId = "2df28110-93d0-427d-9207-d55dbca680fa";
        private const string _loggedInUserID = "e2eb8989-a81a-4151-8e86-eb95-a7961da2";
        
        static async Task Main(string[] args)
        {
            BuildOptions();
            BuildMapper();

            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                _itemsService = new ItemsService(db, _mapper);
                _categoriesService = new CategoriesService(db, _mapper);

                await ListInventory();
                await GetItemsForListing();
                await GetAllActiveItemsAsPipeDelimitedString();
                await GetItemsTotalValues();
                await GetFullItemDetails();
                await GetItemsForListingLinq();
                await ListCategoriesAndColors();

                Console.WriteLine("Would you like to create items?");
                var createItems = Console.ReadLine().StartsWith("y",StringComparison.OrdinalIgnoreCase);
                if (createItems)
                {
                    Console.WriteLine("Adding new Item(s)");
                    await CreateMultipleItems();
                    Console.WriteLine("Items added");
                    var inventory = await _itemsService.GetItems();
                    inventory.ForEach(x => Console.WriteLine($"Item: {x}"));
                }

                Console.WriteLine("Would you like to update items?");
                var updateItems = Console.ReadLine().StartsWith("y", StringComparison.OrdinalIgnoreCase);
                if (updateItems)
                {
                    Console.WriteLine("Updating new Item(s)");
                    await UpdateMultipleItems();
                    Console.WriteLine("Items updated");
                    var inventory2 = await _itemsService.GetItems();
                    inventory2.ForEach(x => Console.WriteLine($"Item: {x}"));
                }

                Console.WriteLine("Would you like to delete items?");
                var deteteItems = Console.ReadLine().StartsWith("y", StringComparison.OrdinalIgnoreCase);
                if (deteteItems)
                {
                    Console.WriteLine("Deleting new Item(s)");
                    await DeleteMultipleItems();
                    Console.WriteLine("Items deleted");
                    var inventory3 = await _itemsService.GetItems();
                    inventory3.ForEach(x => Console.WriteLine($"Item: {x}"));
                }
            }

            Console.WriteLine("Program complete");
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

        //GRUD
        private static async Task DeleteMultipleItems()
        {
            Console.WriteLine("Wouid you like to delete items as a batch?");
            bool batchDelete = Console.ReadLine().StartsWith("y", StringComparison.OrdinalIgnoreCase);
            var allItems = new List<int>();
            bool deleteAnother = true;
            while (deleteAnother == true)
            {
                Console.WriteLine("Items");
                Console.WriteLine("Enter te ID number to delete");
                Console.WriteLine("************************************************");
                var items = await _itemsService.GetItems();
                items.ForEach(x => Console.WriteLine($"ID: {x.Id} | {x.Name}"));
                Console.WriteLine("************************************************");
                if (batchDelete && allItems.Any())
                {
                    Console.WriteLine("Items scheduled for delete");
                    allItems.ForEach(x => Console.Write($"{x},"));
                    Console.WriteLine();
                    Console.WriteLine("************************************************");
                }
                int id = 0;
                if (int.TryParse(Console.ReadLine(), out id))
                {
                    var itemMach = items.FirstOrDefault(x => x.Id == id);
                    if (itemMach != null)
                    {
                        if (batchDelete)
                        {
                            if (!allItems.Contains(itemMach.Id))
                            {
                                allItems.Add(itemMach.Id);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Are you sure you want to delete the item {itemMach.Id}-{itemMach.Name}");
                            if (Console.ReadLine().StartsWith("y",StringComparison.OrdinalIgnoreCase))
                            {
                                await _itemsService.DeleteItem(itemMach.Id);
                                Console.WriteLine("Item deleted");
                            }
                        }
                    }
                }
                Console.WriteLine("Wouid you like to delete another item?");
                deleteAnother = Console.ReadLine().StartsWith("y", StringComparison.OrdinalIgnoreCase);
                if (batchDelete && !deleteAnother)
                {
                    Console.WriteLine("Are you sure you want to delete following items: ");
                    allItems.ForEach(x => Console.Write($"{x},"));
                    Console.WriteLine();
                    if (Console.ReadLine().StartsWith("y", StringComparison.OrdinalIgnoreCase))
                    {
                        await _itemsService.DeleteItems(allItems);
                        Console.WriteLine("Items deleted");
                    }
                }
            }
        }
        private static async Task UpdateMultipleItems()
        {
            Console.WriteLine("Wouid you like to update items as a batch?");
            bool batchUpdate = Console.ReadLine().StartsWith("y", StringComparison.OrdinalIgnoreCase);
            var allItems = new List<CreateOrUpdateItemDTO>();
            bool updateAnother = true;
            while (updateAnother == true)
            {
                Console.WriteLine("Items");
                Console.WriteLine("Enter te ID number to update");
                Console.WriteLine("************************************************");
                var items = await _itemsService.GetItems();
                items.ForEach(x => Console.WriteLine($"ID: {x.Id} | {x.Name}"));
                Console.WriteLine("************************************************");
                int id = 0;
                if (int.TryParse(Console.ReadLine(), out id))
                {
                    var itemMatch = items.FirstOrDefault(x => x.Id == id);
                    if (itemMatch != null)
                    {
                        var updItem = _mapper.Map<CreateOrUpdateItemDTO>(_mapper.Map<Item>(itemMatch));
                        Console.WriteLine("Enter the new name [leave blank to keep existing]");
                        var newName = Console.ReadLine();
                        updItem.Name = !string.IsNullOrWhiteSpace(newName) ? newName : updItem.Name;
                        Console.WriteLine("Enter the new description [leave blank to keep existing]");
                        var newDesc = Console.ReadLine();
                        updItem.Description = !string.IsNullOrWhiteSpace(newDesc) ? newDesc : updItem.Description;
                        Console.WriteLine("Enter the new notes [leave blank to keep existing]");
                        var newNotes = Console.ReadLine();
                        updItem.Notes = !string.IsNullOrWhiteSpace(newNotes) ? newNotes : updItem.Notes;
                        Console.WriteLine("Toggle item Active Status? [y/n]");
                        var toggleActive = Console.ReadLine().Substring(0, 1).Equals("y", StringComparison.OrdinalIgnoreCase);
                        if (toggleActive)
                        {
                            updItem.IsActive = !updItem.IsActive;
                        }
                        Console.WriteLine("Enter the Category [B]ooks, [M]ovies, [G]ames, or [N]o Change");
                        var userChoise = Console.ReadLine().Substring(0, 1).ToUpper();
                        updItem.CategoryId = userChoise.Equals("N",StringComparison.OrdinalIgnoreCase) ? itemMatch.CategoryId : GetCategoryId(userChoise);
                        if (!batchUpdate)
                        {
                            await _itemsService.UpsertItem(updItem);
                        }
                        else
                        {
                            allItems.Add(updItem);
                        }
                    }
                }
                Console.WriteLine("Wouid you like to update another?");
                updateAnother = Console.ReadLine().StartsWith("y", StringComparison.OrdinalIgnoreCase);
                if (batchUpdate && !updateAnother)
                {
                    await _itemsService.UpsertItems(allItems);
                }
            }
        }
        private static async Task CreateMultipleItems()
        {
            Console.WriteLine("Wouid you like to create items as a batch?");
            bool batchCreate = Console.ReadLine().StartsWith("y", StringComparison.OrdinalIgnoreCase);
            var allItems = new List<CreateOrUpdateItemDTO>();
            bool createAnother = true;
            while (createAnother == true)
            {
                var newItem = new CreateOrUpdateItemDTO();
                Console.WriteLine("Creating a new item");
                Console.WriteLine("Please enter the name");
                newItem.Name = Console.ReadLine();
                Console.WriteLine("Please enter the description");
                newItem.Description = Console.ReadLine();
                Console.WriteLine("Please enter the notes");
                newItem.Notes = Console.ReadLine();
                Console.WriteLine("Please enter the Category [B]ooks, [M]ovies, [G]ames");
                newItem.CategoryId = GetCategoryId(Console.ReadLine().Substring(0,1).ToUpper());
                if (!batchCreate)
                {
                    await _itemsService.UpsertItem(newItem);
                }
                else
                {
                    allItems.Add(newItem);
                }
                Console.WriteLine("Wouid you like to create another item?");
                createAnother = Console.ReadLine().StartsWith("y", StringComparison.OrdinalIgnoreCase);
                if (batchCreate && !createAnother)
                {
                    await _itemsService.UpsertItems(allItems);
                }
            }
        }
        private static int GetCategoryId(string input)
        {
            switch (input)
            {
                case "B":
                    return _categories.FirstOrDefault(x => x.Category.ToLower().Equals("books"))?.Id ?? -1;
                case "M":
                    return _categories.FirstOrDefault(x => x.Category.ToLower().Equals("movies"))?.Id ?? -1;
                case "G":
                    return _categories.FirstOrDefault(x => x.Category.ToLower().Equals("games"))?.Id ?? -1;
                default:
                    return -1;
            }
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
        private static async Task ListInventory()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var result = await _itemsService.GetItems();
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
        private static async Task ListInventoryWithProjections()
        {
            //автомапер с проекцией прямо в запросе
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var iems = await db.Items
                    //отключили из-за шифрования
                    //.OrderBy(x => x.Name)
                    .ProjectTo<ItemDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync();
                //iems.ForEach(x => Console.WriteLine($"New Item: {x}"));
                //из-за шифрования приходится делать сортировки и пр после получения всех данных с сревера и дешифровки - УДАР ПО ПРОИЗВОДИТЕЛЬНОСТИ
                iems.OrderBy(x => x.Name).ToList().ForEach(x => Console.WriteLine($"New Item: {x}"));
            }
        }
        private static async Task ListCategoriesAndColors()
        {
            var results = await _categoriesService.ListCategoriesAndDetails();
            foreach (var c in results)
            {
                Console.WriteLine($"Category [{c.Category}] is {c.CategotyDetail?.Color}");
            }

            _categories = results;

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
        private static async Task GetItemsForListing()
        {
            var results = await _itemsService.GetItemsForListingFromProcedure();
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
        private static async Task GetItemsForListingLinq()
        {
            var minDateValue = new DateTime(2021, 1, 1);
            var maxDateValue = new DateTime(2024, 1, 1);

            var results = await _itemsService.GetItemsByDateRange(minDateValue, maxDateValue);
            foreach (var itemDto in results.OrderBy(y => y.CategoryName).ThenBy(z => z.Name))
            {
                Console.WriteLine(itemDto);
            }
            //синхронка
            /*var results = _itemsService.GetItemsByDateRange(minDateValue, maxDateValue)
                .OrderBy(y => y.CategoryName).ThenBy(z => z.Name);
            foreach (var itemDto in results)
            {
                Console.WriteLine(itemDto);
            }*/
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
        private static async Task GetAllActiveItemsAsPipeDelimitedString()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                Console.WriteLine($"All active Items: {await _itemsService.GetAllItemsPipeDelimitedString()}");
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
        private static async Task GetItemsTotalValues()
        {
            var results = await _itemsService.GetItemsTotalValues(true);
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
        private static async Task GetFullItemDetails()
        {
            var result = await _itemsService.GetItemsWithGenresAndCategories();
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