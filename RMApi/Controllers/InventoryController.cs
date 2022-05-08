using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private IConfiguration _config;

        public InventoryController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public List<InventoryDBModel> GetAll()
        {
            InventoryData inventoryData = new InventoryData(_config);
            return inventoryData.GetAll();
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public void Post(InventoryModel item)
        {
            InventoryData inventoryData = new InventoryData(_config);
            inventoryData.SaveInventory(item);
        }
    }
}
