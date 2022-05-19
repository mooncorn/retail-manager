using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RMDataManager.Library.DataAccess
{
    public class SaleData : ISaleData
    {
        private readonly IConfiguration _configuration;
        private ISqlDataAccess _sqlDataAccess;
        private IProductData _productData;

        public SaleData(IConfiguration configuration, ISqlDataAccess sqlDataAccess, IProductData productData)
        {
            _configuration = configuration;
            _sqlDataAccess = sqlDataAccess;
            _productData = productData;
        }

        public decimal GetTaxRate()
        {
            return Convert.ToDecimal(_configuration["TaxRate"]) / 100;
        }

        public void SaveSale(SaleModel saleInfo, string userId)
        {
            // TODO: Make this better please

            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();

            foreach (SaleDetailModel item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                };

                // Get information about current product
                var productInfo = _productData.GetById(detail.ProductId);

                if (productInfo == null)
                    throw new Exception($"The product Id of {detail.ProductId} could not be found in the database.");

                // Fill detail model with current product information and add it to the list
                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;

                if (productInfo.IsTaxable)
                    detail.Tax = detail.PurchasePrice * GetTaxRate();

                details.Add(detail);
            }

            // Create a sale model
            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = details.Sum((detail) => detail.PurchasePrice),
                Tax = details.Sum((detail) => detail.Tax),
                SellerId = userId
            };

            sale.Total = sale.SubTotal + sale.Tax;

            try
            {
                _sqlDataAccess.StartTransaction("RMData");

                // Save the sale model
                _sqlDataAccess.SaveDataInTransaction("spSale_Insert", sale);

                // Get the id of inserted sale
                var parameters = new { sale.SellerId, sale.SaleDate };
                sale.Id = _sqlDataAccess.LoadDataInTransaction<int, dynamic>("spSale_Lookup", parameters).FirstOrDefault();

                // Finish filling in the sale detail models
                foreach (SaleDetailDBModel detail in details)
                {
                    detail.SaleId = sale.Id;

                    // Save the detail models
                    _sqlDataAccess.SaveDataInTransaction("spSaleDetail_Insert", detail);
                }

                _sqlDataAccess.CommitTransaction();
            }
            catch
            {
                _sqlDataAccess.RollbackTransaction();
                throw; // throw original exception
            }
        }

        public List<SaleReportModel> GetSaleReport()
        {
            return _sqlDataAccess.LoadData<SaleReportModel, dynamic>("spSale_SaleReport", null, "RMData");
        }
    }
}
