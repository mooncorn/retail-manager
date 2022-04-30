CREATE PROCEDURE [dbo].[spSale_Lookup]
	@SellerId nvarchar(128),
	@SaleDate datetime2
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id
	FROM dbo.Sale
	WHERE SellerId = @SellerId
	AND SaleDate = @SaleDate;
END
