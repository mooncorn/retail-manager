using RMDataManager.Library.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class InventoryData
    {
        public List<InventoryDBModel> GetAll()
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess();
            return sqlDataAccess.LoadData<InventoryDBModel, dynamic>("spInventory_GetAll", null, "RMData");
        }

        public void SaveInventory(InventoryModel item)
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess();
            sqlDataAccess.SaveData("spInventory_Insert", item, "RMData");
        }
    }
}
