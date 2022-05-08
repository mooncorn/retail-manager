using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System.Security.Claims;
using TRMDataManager.Library.Models;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private IConfiguration _config;

        public SaleController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void Post(SaleModel sale)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            SaleData saleData = new SaleData(_config);
            saleData.SaveSale(sale, userId);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            SaleData saleData = new SaleData(_config);
            return saleData.GetSaleReport();
        }
    }
}
