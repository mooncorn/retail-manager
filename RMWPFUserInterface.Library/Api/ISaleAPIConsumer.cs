using RMWPFUserInterface.Library.Models;
using System.Threading.Tasks;

namespace RMWPFUserInterface.Library.Api
{
    public interface ISaleAPIConsumer
    {
        Task PostSale(SaleModel sale);
    }
}