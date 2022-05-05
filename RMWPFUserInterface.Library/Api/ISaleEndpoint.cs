using RMWPFUserInterface.Library.Models;
using System.Threading.Tasks;

namespace RMWPFUserInterface.Library.Api
{
    public interface ISaleEndpoint
    {
        Task PostSale(SaleModel sale);
    }
}