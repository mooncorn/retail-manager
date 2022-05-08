using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cashier")]
    public class ProductController : ControllerBase
    {
        private IConfiguration _config;

        public ProductController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("All")]
        public List<ProductDBModel> GetAll()
        {
            ProductData productData = new ProductData(_config);
            return productData.GetAll();
        }
    }
}
