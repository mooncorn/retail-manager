using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;
using Microsoft.Extensions.Configuration;

namespace RMDataManager.Library.DataAccess
{
    public class SaleData
    {
        private IConfiguration _config;

        public SaleData(IConfiguration config)
        {
            _config = config;
        }

        public void SaveSale(SaleModel saleInfo, string userId)
        {
            // TODO: Make this better please

            ProductData productData = new ProductData(_config);
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();

            foreach (SaleDetailModel item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                };

                // Get information about current product
                var productInfo = productData.GetById(detail.ProductId);

                if (productInfo == null)
                    throw new Exception($"The product Id of {detail.ProductId} could not be found in the database.");

                // Fill detail model with current product information and add it to the list
                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;

                if (productInfo.IsTaxable)
                    detail.Tax = detail.PurchasePrice * ConfigHelper.TaxRate / 100;

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

            using (SqlDataAccess sqlDataAccess = new SqlDataAccess(_config))
            {
                try
                {
                    sqlDataAccess.StartTransaction("RMData");

                    // Save the sale model
                    sqlDataAccess.SaveDataInTransaction("spSale_Insert", sale);

                    // Get the id of inserted sale
                    var parameters = new { sale.SellerId, sale.SaleDate };
                    sale.Id = sqlDataAccess.LoadDataInTransaction<int, dynamic>("spSale_Lookup", parameters).FirstOrDefault();

                    // Finish filling in the sale detail models
                    foreach (SaleDetailDBModel detail in details)
                    {
                        detail.SaleId = sale.Id;

                        // Save the detail models
                        sqlDataAccess.SaveDataInTransaction("spSaleDetail_Insert", detail);
                    }
                }
                catch
                {
                    sqlDataAccess.RollbackTransaction();
                    throw; // throw original exception
                }
            }
        }

        public List<SaleReportModel> GetSaleReport()
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess(_config);
            return sqlDataAccess.LoadData<SaleReportModel, dynamic>("spSale_SaleReport", null, "RMData");
        }
    }
}
