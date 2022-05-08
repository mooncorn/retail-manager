using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public class InventoryData
    {
        private IConfiguration _config;

        public InventoryData(IConfiguration config)
        {
            _config = config;
        }

        public List<InventoryDBModel> GetAll()
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess(_config);
            return sqlDataAccess.LoadData<InventoryDBModel, dynamic>("spInventory_GetAll", null, "RMData");
        }

        public void SaveInventory(InventoryModel item)
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess(_config);
            sqlDataAccess.SaveData("spInventory_Insert", item, "RMData");
        }
    }
}
