using RMDataManager.Library.Models;
using System.Collections.Generic;

namespace RMDataManager.Library.DataAccess
{
    public interface IProductData
    {
        List<ProductDBModel> GetAll();
        ProductDBModel GetById(int id);
    }
}