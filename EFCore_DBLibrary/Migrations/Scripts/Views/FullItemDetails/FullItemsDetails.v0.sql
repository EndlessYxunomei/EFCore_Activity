CREATE OR ALTER VIEW [dbo].[vwFullItemsDetails]
AS
SELECT item.Id, item.[Name] ItemName, item.[Description] ItemDescription
	, item.IsActive, item.IsDeleted, item.Notes, item.CurrentOrFinalPrice
	, item.IsOnSale, item.PurchasedDate, item.PurchasePrice, item.Quantity
	, item.SoldDate, cat.[Name] Category, cat.IsActive CategoryIsActive
	, cat.IsDeleted CategoryIsDeleted, catDetail.ColorName, catDetail.ColorValue
	, player.[Name] PlayerName, player.[Description] PlayerDescription
	, player.IsActive PlayerIsActive, player.IsDeleted PlayerIsDeleted
	, genre.[Name] GenreName, genre.[IsActive] GenreIsActive, genre.IsDeleted GenreIsDeleted
FROM Items item
LEFT JOIN Categories cat on item.CategoryId = cat.Id
LEFT JOIN CategoryDetails catDetail on cat.Id = catDetail.Id
LEFT JOIN ItemPlayers ip on item.Id = ip.ItemId
LEFT JOIN Player player on ip.PlayerId = player.Id
LEFT JOIN ItemGenres ig on item.Id = ig.ItemId
LEFT JOIN Genres genre on ig.GenreId = genre.Id