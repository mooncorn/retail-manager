CREATE PROCEDURE [dbo].[spSale_Insert]
	@Id int output,
	@SellerId nvarchar(128),
	@SaleDate datetime2,
	@SubTotal money,
	@Tax money,
	@Total money
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.Sale(SellerId, SaleDate, SubTotal, Total, Tax) 
	VALUES (@SellerId, @SaleDate, @SubTotal, @Total, @Tax);
END
