CREATE PROCEDURE [dbo].[spSaleDetail_Insert]
	@Id int output,
	@SaleId int,
	@ProductId int,
	@Quantity int,
	@PurchasePrice money,
	@Tax money
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.SaleDetail(SaleId, ProductId, Quantity, Tax, PurchasePrice)
	VALUES (@SaleId, @ProductId, @Quantity, @Tax, @PurchasePrice);
END
