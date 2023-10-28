using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCore_DBLibrary;
using InventoryModels;
using InventoryModels.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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



        /*public List<ItemDTO> GetItems()
        {
            var items = _context.Items
                .ProjectTo<ItemDTO>(_mapper.ConfigurationProvider)
                .ToList();
            return items;
        }*/
        public List<Item> GetItems()
        {
            var items = _context.Items
                .Include(x => x.Category)
                .AsEnumerable()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
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

        public int UpsertItem(Item item)
        {
            if (item.Id > 0)
            {
                return UpdateItem(item);
            }
            return CreateItem(item);
        }

        private int CreateItem(Item item)
        {
            _context.Items.Add(item);
            _context.SaveChanges();
            var newItem = _context.Items.ToList()
            .FirstOrDefault(x => x.Name.ToLower().Equals(item.Name.ToLower()));
            if (newItem == null) { throw new Exception("Could not Create the item as expected"); }
            return newItem.Id;
        }

        private int UpdateItem(Item item)
        {
            var dbItem = _context.Items
                .Include(x => x.Category)
                .Include(x => x.ItemGenres)
                .Include(x => x.Players)
                .FirstOrDefault(x => x.Id == item.Id);
            if (dbItem == null) { throw new Exception("Item not found"); }
            dbItem.CategoryId = item.CategoryId;
            dbItem.CurrentOrFinalPrice = item.CurrentOrFinalPrice;
            dbItem.Description = item.Description;
            dbItem.IsActive = item.IsActive;
            dbItem.IsDeleted = item.IsDeleted;
            dbItem.IsOnSale = item.IsOnSale;
            if (item.ItemGenres != null)
            {
                dbItem.ItemGenres = item.ItemGenres;
            }
            dbItem.Name = item.Name;
            dbItem.Notes = item.Notes;
            if (item.Players != null)
            {
                dbItem.Players = item.Players;
            }
            dbItem.PurchasePrice = item.PurchasePrice;
            dbItem.PurchasedDate = item.PurchasedDate;
            dbItem.Quantity = item.Quantity;
            dbItem.SoldDate = item.SoldDate;
            _context.SaveChanges();
            return item.Id;
        }

        public void UpsertItems(List<Item> items)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in items)
                    {
                        var success = UpsertItem(item) > 0;
                        if (!success) { throw new Exception($"Error saving the item {item.Name}"); }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    //Log the exception
                    Debug.WriteLine(ex.ToString());
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public void DeleteItem(int id)
        {
            var item = _context.Items.FirstOrDefault(x => x.Id == id);
            if (item == null) { return; }
            item.IsDeleted = true;
            _context.SaveChanges();
        }
        public void DeleteItems(List<int> itemIds)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var itemId in itemIds)
                    {
                        DeleteItem(itemId);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    //Log the exception
                    Debug.WriteLine(ex.ToString());
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}