﻿using System.ComponentModel.DataAnnotations;

namespace InventoryModels
{
    public class Item: FullAuditModel
    {
        [Required]
        [StringLength(InventoryModelConstants.MAX_NAME_LENGTH)]
        public required string Name { get; set; }
        [Range(InventoryModelConstants.MINIMUM_QUANTITY, InventoryModelConstants.MAXIMUM_QUANTITY)]
        public int Quantity { get; set; }
        [StringLength(InventoryModelConstants.MAX_DESCRIPTION_LENGTH)]
        [Required]
        public string? Description { get; set; }
        [StringLength(InventoryModelConstants.MAX_NOTES_LENGTH, MinimumLength = 10)]
        public string? Notes { get; set; }
        public bool IsOnSale { get; set; }
        public DateTime? PurchasedDate { get; set; }
        public DateTime? SoldDate { get; set; }
        [Range(InventoryModelConstants.MINIMUM_PRICE, InventoryModelConstants.MAXIMUM_PRICE)]
        public decimal? PurchasePrice { get; set; }
        [Range(InventoryModelConstants.MINIMUM_PRICE, InventoryModelConstants.MAXIMUM_PRICE)]
        public decimal? CurrentOrFinalPrice { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual List<Player> Players { get; set; } = new List<Player>();
        public virtual List<ItemGenre> ItemGenres { get; set; } = new List<ItemGenre>();
    }
}