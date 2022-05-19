using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public class ProductData : IProductData
    {
        private ISqlDataAccess _sqlDataAccess;

        public ProductData(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public List<ProductDBModel> GetAll()
        {
            return _sqlDataAccess.LoadData<ProductDBModel, dynamic>("spProduct_GetAll", new { }, "RMData");
        }

        public ProductDBModel GetById(int id)
        {
            return _sqlDataAccess.LoadData<ProductDBModel, dynamic>("spProduct_GetById", new { Id = id }, "RMData").FirstOrDefault();
        }
    }
}
