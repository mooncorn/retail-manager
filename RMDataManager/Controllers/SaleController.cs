using Microsoft.AspNet.Identity;
using RMDataManager.Library.Models;
using RMDataManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace RMDataManager.Controllers
{
    [Authorize]
    public class SaleController : ApiController
    {
        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void Post(SaleModel sale)
        {
            string userId = RequestContext.Principal.Identity.GetUserId();
            SaleData saleData = new SaleData();
            saleData.SaveSale(sale, userId);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            SaleData saleData = new SaleData();
            return saleData.GetSaleReport();
        }
    }
}
