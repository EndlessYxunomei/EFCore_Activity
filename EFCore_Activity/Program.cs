﻿using EFCore_DBLibrary;
using EFCore_DBLibrary.Migrations;
using InventoryHelpers;
using InventoryModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCore_Activity
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        private static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder;

        //пока нет нормальных пользователей заменяем их константой
        private const string _systemUserId = "2df28110-93d0-427d-9207-d55dbca680fa";
        private const string _loggedInUserID = "e2eb8989-a81a-4151-8e86-eb95-a7961da2";
        
        static void Main(string[] args)
        {
            BuildOptions();
            //DeleteAllItems();
            //EnsureItems();
            //UpdateItems();
            ListInventory();
            GetItemsForListing();
            GetAllActiveItemsAsPipeDelimitedString();
            GetItemsTotalValues();
            GetFullItemDetails();
        }
        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
        }
        static void EnsureItems()
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
        private static void ListInventory()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var items = db.Items.OrderBy(x => x.Name).ToList();
                items.ForEach(x => Console.WriteLine($"New Item: {x.Name}"));
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
        private static void GetItemsForListing()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
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
            }
        }
        private static void GetAllActiveItemsAsPipeDelimitedString()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var isActiveParm = new SqlParameter("IsActive", 1);
                var result = db.AllItemsOutput
                    .FromSqlRaw("SELECT [dbo].[ItemNamesPipeDelimitedString] (@IsActive) AllItems", isActiveParm)
                    .FirstOrDefault();
                Console.WriteLine($"All active Items: {result.AllItems}");
            }
        }
        private static void GetItemsTotalValues()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var isActiveParm = new SqlParameter("IsActive", 1);
                var result = db.GetItemsTotalValues.FromSqlRaw("SELECT * FROM [dbo].[GetItemsTotalValue] (@IsActive)", isActiveParm).ToList();
                foreach (var item in result)
                {
                    Console.WriteLine($"New Item] {item.Id, -10}" + $" | {item.Name, -50}" + $" | {item.Quantity, -4}" + $" | {item.TotalValue, -5}");
                }
            }
        }
        private static void GetFullItemDetails()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var result = db.FullItemsDetailDtos.FromSqlRaw("SELECT * FROM [dbo].[vwFullItemsDetails]").ToList();
                foreach (var item in result)
                {
                    Console.WriteLine($"New Item] {item.Id, -10}" + $"|{item.ItemName, -50}" + $"|{item.ItemDescription, -4}" + $"|{item.PlayerName, -5}" + $"|{item.Category, -5}" + $"|{item.GenreName, -5}");
                }
            }
        }
    }
}