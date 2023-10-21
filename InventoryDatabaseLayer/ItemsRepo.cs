using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCore_DBLibrary;
using InventoryModels.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InventoryDatabaseLayer
{
    public class ItemsRepo : IItemsRepo
    {
        private readonly IMapper _mapper;
        private readonly InventoryDbContext _context;
        public ItemsRepo(InventoryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public List<ItemDTO> GetItems()
        {
            var items = _context.Items
                .ProjectTo<ItemDTO>(_mapper.ConfigurationProvider)
                .ToList();
            return items;
        }

        public List<ItemDTO> GetItemsByDateRange(DateTime minDateValue, DateTime maxDateValue)
        {
            var items = _context.Items.Include(x => x.Category)
                .Where(x => x.CreatedDate >= minDateValue && x.CreatedDate <= maxDateValue)
                .ProjectTo<ItemDTO>(_mapper.ConfigurationProvider)
                .ToList();
            return items;
        }

        public List<GetItemForListingDTO> GetItemsForListingFromProcedure()
        {
            return _context.ItemsForListing.FromSqlRaw("EXECUTE dbo.GetItemsForListing").ToList();
        }

        public List<GetItemsTotalValueDTO> GetItemsTotalValues(bool isActive)
        {
            var isActiveParam = new SqlParameter ("IsActive",1);
            return _context.GetItemsTotalValues
                .FromSqlRaw("SELECT * FROM [dbo].[GetItemsTotalValue] (@IsActive)", isActiveParam)
                .ToList();
        }

        public List<FullItemDetailDTO> GetItemsWithGenresAndCategories()
        {
            return _context.FullItemsDetailDtos
                .FromSqlRaw("SELECT * FROM [dbo].[vwFullItemsDetails]").AsEnumerable()
                .OrderBy(x => x.ItemName).ThenBy(x => x.GenreName)
                .ThenBy(x => x.Category).ThenBy(x => x.PlayerName)
                .ToList();
        }
    }
}