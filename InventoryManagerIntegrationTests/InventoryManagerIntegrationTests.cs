using AutoMapper;
using EFCore_DBLibrary;
using InventoryDatabaseLayer;
using InventoryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
//using System;
//using System.Linq;
//using Xunit;

namespace InventoryManagerIntegrationTests
{
    public class InventoryManagerIntegrationTests
    {
        DbContextOptions<InventoryDbContext> _options;
        private static MapperConfiguration _mapperConfig;
        private static IMapper _mapper;
        private static IServiceProvider _serviceProvider;
        private IItemsRepo _dbRepo;

        private const string COLOR_BLUE = "Blue";
        private const string COLOR_RED = "Red";
        private const string COLOR_GREEN = "Green";
        private const string COLOR_BLUE_VALUE = "#0000FF";
        private const string COLOR_RED_VALUE = "#FF0000";
        private const string COLOR_GREEN_VALUE = "#00FF00";
        private const string CAT1_NAME = "CAT1 Books";
        private const string CAT2_NAME = "CAT2 Movies";
        private const string CAT3_NAME = "CAT3 Music";
        private const string ITEM1_NAME = "Item 1 Name";
        private const string ITEM2_NAME = "Item 2 Name";
        private const string ITEM3_NAME = "Item 3 Name";
        private const string ITEM1_DESC = "Item 1 DESC";
        private const string ITEM2_DESC = "Item 2 DESC";
        private const string ITEM3_DESC = "Item 3 DESC";
        private const string ITEM1_NOTES = "Item 1 Notes Good";
        private const string ITEM2_NOTES = "Item 2 Notes Fair";
        private const string ITEM3_NOTES = "Item 3 Notes Poor";

        public InventoryManagerIntegrationTests()
        { 
            SetupOptions();
            BuildDefaults();
        }
        private void SetupOptions()
        {
            _options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(databaseName: "InventoryManagerTest")
                .Options;
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(InventoryMapper));
            _serviceProvider = services.BuildServiceProvider();
            _mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InventoryMapper>();
            });
            _mapperConfig.AssertConfigurationIsValid();
            _mapper = _mapperConfig.CreateMapper();
        }
        
        [Fact]
        public async Task TestGetItems()
        {
            //arrange
            //BuildDefaults();
            using (var context = new InventoryDbContext(_options))
            {
                //act
                _dbRepo = new ItemsRepo(context, _mapper);
                var items = await _dbRepo.GetItems();
                //assert
                items.ShouldNotBeNull();
                items.Count.ShouldBe(3);
                var first = items.First();
                first.Name.ShouldBe(ITEM1_NAME);
                first.Description.ShouldBe(ITEM1_DESC);
                first.Notes.ShouldBe(ITEM1_NOTES);
                first.Category.Name.ShouldBe(CAT1_NAME);
                var second = items.SingleOrDefault(x => x.Name.ToLower() == ITEM2_NAME.ToLower());
                second.ShouldNotBeNull();
                second.Description.ShouldBe(ITEM2_DESC);
                second.Notes.ShouldBe(ITEM2_NOTES);
                second.Category.Name.ShouldBe(CAT2_NAME);
            }
        }

        [Theory]
        [InlineData(CAT1_NAME, COLOR_BLUE, COLOR_BLUE_VALUE)]
        [InlineData(CAT2_NAME, COLOR_RED, COLOR_RED_VALUE)]
        [InlineData(CAT3_NAME, COLOR_GREEN, COLOR_GREEN_VALUE)]
        public async Task TestCategoryColors(string name, string color, string colorValue)
        {
            //arrange
            //BuildDefaults();
            using (var contest = new InventoryDbContext(_options))
            {
                //act
                var categoriesRepo = new CategoriesRepo(_mapper, contest);
                var categories = await categoriesRepo.ListCategoriesAndDetails();
                //assert
                categories.ShouldNotBeNull();
                categories.Count.ShouldBe(3);
                var category = categories.FirstOrDefault(x => x.Category.Equals(name));
                category.ShouldNotBeNull();
                category.CategotyDetail.Color.ShouldBe(color);
                category.CategotyDetail.Value.ShouldBe(colorValue);
            }
        }

        private void BuildDefaults()
        {
            using (var context = new InventoryDbContext(_options))
            {
                //проверяем есть ли уже сделаные тестовые объекты
                var item1Detail = context.Items.SingleOrDefault(x => x.Name.Equals(ITEM1_NAME));
                var item2Detail = context.Items.SingleOrDefault(x => x.Name.Equals(ITEM2_NAME));
                var item3Detail = context.Items.SingleOrDefault(x => x.Name.Equals(ITEM3_NAME));
                if (item1Detail != null && item2Detail != null && item3Detail != null) { return; }

                //создем тестовые объект ести их нет
                //Категории
                var color1 = new CategoryDetail() { ColorName = COLOR_BLUE, ColorValue = COLOR_BLUE_VALUE };
                var color2 = new CategoryDetail() { ColorName = COLOR_RED, ColorValue = COLOR_RED_VALUE };
                var color3 = new CategoryDetail() { ColorName = COLOR_GREEN, ColorValue = COLOR_GREEN_VALUE };
                var cat1 = new Category()
                {
                    CategoryDetail = color1,
                    IsActive = true,
                    IsDeleted = false,
                    Name = CAT1_NAME
                };
                var cat2 = new Category()
                {
                    CategoryDetail = color2,
                    IsActive = true,
                    IsDeleted = false,
                    Name = CAT2_NAME
                };
                var cat3 = new Category()
                {
                    CategoryDetail = color3,
                    IsActive = true,
                    IsDeleted = false,
                    Name = CAT3_NAME
                };
                context.Categories.Add(cat1);
                context.Categories.Add(cat2);
                context.Categories.Add(cat3);
                context.SaveChanges();
                //сначала закидываем в базу, а затем используем уже из базы
                var category1 = context.Categories.Single(x => x.Name.Equals(CAT1_NAME));
                var category2 = context.Categories.Single(x => x.Name.Equals(CAT2_NAME));
                var category3 = context.Categories.Single(x => x.Name.Equals(CAT3_NAME));
                //Предметы
                var item1 = new Item()
                { 
                    Name = ITEM1_NAME,
                    Description = ITEM1_DESC,
                    Notes = ITEM1_NOTES,
                    IsActive = true,
                    IsDeleted = false,
                    CategoryId = category1.Id
                };
                context.Items.Add(item1);
                var item2 = new Item()
                {
                    Name = ITEM2_NAME,
                    Description = ITEM2_DESC,
                    Notes = ITEM2_NOTES,
                    IsActive = true,
                    IsDeleted = false,
                    CategoryId = category2.Id
                };
                context.Items.Add(item2);
                var item3 = new Item()
                {
                    Name = ITEM3_NAME,
                    Description = ITEM3_DESC,
                    Notes = ITEM3_NOTES,
                    IsActive = true,
                    IsDeleted = false,
                    CategoryId = category3.Id
                };
                context.Items.Add(item3);
                context.SaveChanges();
            }
        }
    }
}