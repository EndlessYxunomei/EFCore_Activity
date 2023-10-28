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
        List<ItemDTO> GetItems();
        List<ItemDTO> GetItemsByDateRange(DateTime minDateValue, DateTime maxDateValue);
        List<GetItemForListingDTO> GetItemsForListingFromProcedure();
        List<GetItemsTotalValueDTO> GetItemsTotalValues(bool isActive);
        string GetAllItemsPipeDelimitedString();
        List<FullItemDetailDTO> GetItemsWithGenresAndCategories();

        int UpsertItem(CreateOrUpdateItemDTO item);
        void UpsertItems(List<CreateOrUpdateItemDTO> items);
        void DeleteItem(int id);
        void DeleteItems(List<int> itemIds);
    }
}
