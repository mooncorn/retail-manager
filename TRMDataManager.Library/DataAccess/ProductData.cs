using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class ProductData
    {
        public List<ProductModel> GetAll()
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess();
            return sqlDataAccess.LoadData<ProductModel, dynamic>("spProduct_GetAll", null, "RMData");
        }
    }
}
