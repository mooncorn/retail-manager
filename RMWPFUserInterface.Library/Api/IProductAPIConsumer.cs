using RMWPFUserInterface.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMWPFUserInterface.Library.Api
{
    public interface IProductAPIConsumer
    {
        Task<List<ProductModel>> GetAll();
    }
}