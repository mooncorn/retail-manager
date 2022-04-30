using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public class ProductData
    {
        public List<ProductDBModel> GetAll()
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess();
            return sqlDataAccess.LoadData<ProductDBModel, dynamic>("spProduct_GetAll", null, "RMData");
        }

        public ProductDBModel GetById(int id)
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess();
            return sqlDataAccess.LoadData<ProductDBModel, dynamic>("spProduct_GetById", new {Id = id}, "RMData").FirstOrDefault();
        }
    }
}
