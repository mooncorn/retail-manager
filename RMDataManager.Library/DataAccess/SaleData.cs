using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string userId)
        {
            // TODO: Make this better please

            ProductData productData = new ProductData();
            SqlDataAccess sqlDataAccess = new SqlDataAccess();
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

            // Save the sale model
            sqlDataAccess.SaveDate("spSale_Insert", sale, "RMData");

            // Get the id of inserted sale
            var parameters = new { sale.SellerId, sale.SaleDate };
            sale.Id = sqlDataAccess.LoadData<int, dynamic>("spSale_Lookup", parameters, "RMData").FirstOrDefault();

            // Finish filling in the sale detail models
            foreach (SaleDetailDBModel detail in details)
            {
                detail.SaleId = sale.Id;

                // Save the detail models
                sqlDataAccess.SaveDate("spSaleDetail_Insert", detail, "RMData");
            }
        }
    }
}
