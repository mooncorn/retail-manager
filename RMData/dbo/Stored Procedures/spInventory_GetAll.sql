CREATE PROCEDURE [dbo].[spInventory_GetAll]
AS
BEGIN
	SELECT [Id], [ProductId], [Quantity], [PurchasePrice], [PurchaseDate] 
	FROM dbo.Inventory
END
