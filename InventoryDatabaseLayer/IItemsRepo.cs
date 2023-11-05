using InventoryModels;
using InventoryModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryDatabaseLayer
{
    public interface IItemsRepo
    {
        //List<ItemDTO> GetItems();
        Task<List<Item>> GetItems();
        Task<List<ItemDTO>> GetItemsByDateRange(DateTime minDateValue, DateTime maxDateValue);
        Task<List<GetItemForListingDTO>> GetItemsForListingFromProcedure();
        Task<List<GetItemsTotalValueDTO>> GetItemsTotalValues(bool isActive);
        Task<List<FullItemDetailDTO>> GetItemsWithGenresAndCategories();

        Task<int> UpsertItem(Item item);
        Task UpsertItems(List<Item> items);
        Task DeleteItem(int id);
        Task DeleteItems(List<int> itemIds);
    }
}
