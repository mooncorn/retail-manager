using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public class InventoryData : IInventoryData
    {
        private ISqlDataAccess _sqlDataAccess;

        public InventoryData(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public List<InventoryDBModel> GetAll()
        {
            return _sqlDataAccess.LoadData<InventoryDBModel, dynamic>("spInventory_GetAll", null, "RMData");
        }

        public void SaveInventory(InventoryModel item)
        {
            _sqlDataAccess.SaveData("spInventory_Insert", item, "RMData");
        }
    }
}
