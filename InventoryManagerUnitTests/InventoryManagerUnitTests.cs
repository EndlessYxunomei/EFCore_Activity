using InventoryBusinessLayer;
using InventoryDatabaseLayer;
using InventoryModels;
using Moq;

namespace InventoryManagerUnitTests
{
    [TestClass]
    public class InventoryManagerUnitTests
    {
        private IItemsService _itemsService;
        private Mock<IItemsRepo> _itemsRepo;

        [TestMethod]
        public void TestGetItems()
        {
            var result = _itemsService.GetItems();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestInitialize]
        public void InitializeTests()
        {
           // _itemsService = new ItemsService();

            //—ќздаем фальшивые данные, которые будут передаватьс€ модулю дл€ тестировани€ вместо реальной базы данных
            _itemsRepo = new Mock<IItemsRepo>();
            var items = new List<Item>()
            { 
                new Item() { Id = 1, Name = "Star Wars IV: A New Hope", Description = "Luke's Friends", CategoryId = 2},
                new Item() { Id = 2, Name = "Star Wars V: The Empire Strikes Back", Description = "Luke's Father", CategoryId = 2},
                new Item() { Id = 3, Name = "Star Wars VI: The Return of the Jedi", Description = "Luke's Sister", CategoryId = 2}
            };
            _itemsRepo.Setup(m => m.GetItems()).Returns(items);
        }
    }
}