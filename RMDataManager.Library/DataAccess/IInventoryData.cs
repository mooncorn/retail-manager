using RMDataManager.Library.Models;
using System.Collections.Generic;

namespace RMDataManager.Library.DataAccess
{
    public interface IInventoryData
    {
        List<InventoryDBModel> GetAll();
        void SaveInventory(InventoryModel item);
    }
}