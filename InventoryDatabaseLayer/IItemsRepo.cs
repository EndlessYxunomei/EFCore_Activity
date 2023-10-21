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
        List<ItemDTO> GetItems();
        List<ItemDTO> GetItemsByDateRange(DateTime minDateValue, DateTime maxDateValue);
        List<GetItemForListingDTO> GetItemsForListingFromProcedure();
        List<GetItemsTotalValueDTO> GetItemsTotalValues(bool isActive);
        List<FullItemDetailDTO> GetItemsWithGenresAndCategories();
    }
}
