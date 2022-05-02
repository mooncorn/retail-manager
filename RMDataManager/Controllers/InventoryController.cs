using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace RMDataManager.Controllers
{
    [Authorize]
    public class InventoryController : ApiController
    {
        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public List<InventoryDBModel> GetAll()
        {
            InventoryData inventoryData = new InventoryData();
            return inventoryData.GetAll();
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public void Post(InventoryModel item)
        {
            InventoryData inventoryData = new InventoryData();
            inventoryData.SaveInventory(item);
        }
    }
}
