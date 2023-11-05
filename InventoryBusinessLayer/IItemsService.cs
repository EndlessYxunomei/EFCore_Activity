using InventoryModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryBusinessLayer
{
    public interface IItemsService
    {
        Task<List<ItemDTO>> GetItems();
        Task<List<ItemDTO>> GetItemsByDateRange(DateTime minDateValue, DateTime maxDateValue);
        Task<List<GetItemForListingDTO>> GetItemsForListingFromProcedure();
        Task<List<GetItemsTotalValueDTO>> GetItemsTotalValues(bool isActive);
        Task<string> GetAllItemsPipeDelimitedString();
        Task<List<FullItemDetailDTO>> GetItemsWithGenresAndCategories();

        Task<int> UpsertItem(CreateOrUpdateItemDTO item);
        Task UpsertItems(List<CreateOrUpdateItemDTO> items);
        Task DeleteItem(int id);
        Task DeleteItems(List<int> itemIds);
    }
}
