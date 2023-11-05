using AutoMapper;
using EFCore_DBLibrary;
using InventoryDatabaseLayer;
using InventoryModels.DTOs;
using InventoryModels;

namespace InventoryBusinessLayer
{
    public class ItemsService : IItemsService
    {
        private readonly IItemsRepo _dbRepo;
        private readonly IMapper _mapper;

        //конструктор зависимый от контекста (слишком жесткая связь)
        public ItemsService(InventoryDbContext context, IMapper mapper)
        {
            _dbRepo = new ItemsRepo(context, mapper);
            _mapper = mapper;
        }
        //конструктор зависимый от репозитория (так лучше и для тестов подойдёт)
        public ItemsService(IItemsRepo dbRepo, IMapper mapper)
        {
            _dbRepo = dbRepo;
            _mapper = mapper;
        }   

        /*public string GetAllItemsPipeDelimitedString()
        {
            var items = GetItems();
            return string.Join('|', items);
        }
        public List<ItemDTO> GetItems()
        {
            //return _dbRepo.GetItems();
            return _mapper.Map<List<ItemDTO>>(_dbRepo.GetItems());
        }
        public List<ItemDTO> GetItemsByDateRange(DateTime minDateValue, DateTime maxDateValue)
        {
            return _dbRepo.GetItemsByDateRange(minDateValue, maxDateValue);
        }
        public List<GetItemForListingDTO> GetItemsForListingFromProcedure()
        {
            return _dbRepo.GetItemsForListingFromProcedure();
        }
        public List<GetItemsTotalValueDTO> GetItemsTotalValues(bool isActive)
        {
            return _dbRepo.GetItemsTotalValues(isActive);
        }
        public List<FullItemDetailDTO> GetItemsWithGenresAndCategories()
        {
            return _dbRepo.GetItemsWithGenresAndCategories();
        }

        public int UpsertItem(CreateOrUpdateItemDTO item)
        {
            if (item.CategoryId <= 0)
            {
                throw new ArgumentException("Please set category id before insert or update");
            }
            return _dbRepo.UpsertItem(_mapper.Map<Item>(item));
        }
        public void UpsertItems(List<CreateOrUpdateItemDTO> items)
        {
            try
            {
                _dbRepo.UpsertItems(_mapper.Map<List<Item>>(items));
            }
            catch (Exception ex)
            {
                //TODO: better logging
                Console.WriteLine($"The transaction has failed: {ex.Message}");
            }
        }
        public void DeleteItem(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Please set a valid item id before deleting");
            }
            _dbRepo.DeleteItem(id);
        }
        public void DeleteItems(List<int> itemIds)
        {
            try
            {
                _dbRepo.DeleteItems(itemIds);
            }
            catch (Exception ex)
            {
                //TODO: better logging
                Console.WriteLine($"The transaction has failed: {ex.Message}");
            }
        }*/

        public async Task<string> GetAllItemsPipeDelimitedString()
        {
            var items = await GetItems();
            return string.Join('|', items);
        }
        public async Task<List<ItemDTO>> GetItems()
        {
            return _mapper.Map<List<ItemDTO>>(await _dbRepo.GetItems());
        }
        public async Task<List<ItemDTO>> GetItemsByDateRange(DateTime minDateValue, DateTime maxDateValue)
        {
            return await _dbRepo.GetItemsByDateRange(minDateValue, maxDateValue);
        }
        public async Task<List<GetItemForListingDTO>> GetItemsForListingFromProcedure()
        {
            return await _dbRepo.GetItemsForListingFromProcedure();
        }
        public async Task<List<GetItemsTotalValueDTO>> GetItemsTotalValues(bool isActive)
        {
            return await _dbRepo.GetItemsTotalValues(isActive);
        }
        public async Task<List<FullItemDetailDTO>> GetItemsWithGenresAndCategories()
        {
            return await _dbRepo.GetItemsWithGenresAndCategories();
        }

        public async Task<int> UpsertItem(CreateOrUpdateItemDTO item)
        {
            if (item.CategoryId <= 0)
            {
                throw new ArgumentException("Please set category id before insert or update");
            }
            return await _dbRepo.UpsertItem(_mapper.Map<Item>(item));
        }
        public async Task UpsertItems(List<CreateOrUpdateItemDTO> items)
        {
            try
            {
                await _dbRepo.UpsertItems(_mapper.Map<List<Item>>(items));
            }
            catch (Exception ex)
            {
                //TODO: better logging
                Console.WriteLine($"The transaction has failed: {ex.Message}");
            }
        }
        public async Task DeleteItem(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Please set a valid item id before deleting");
            }
            await _dbRepo.DeleteItem(id);
        }
        public async Task DeleteItems(List<int> itemIds)
        {
            try
            {
                await _dbRepo.DeleteItems(itemIds);
            }
            catch (Exception ex)
            {
                //TODO: better logging
                Console.WriteLine($"The transaction has failed: {ex.Message}");
            }
        }
    }
}