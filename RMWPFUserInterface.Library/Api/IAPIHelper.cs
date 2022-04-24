using RMWPFUserInterface.Library.Models;
using System.Threading.Tasks;

namespace RMWPFUserInterface.Library.Api
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
        Task GetLoggedInUserDetails(string token);
    }
}