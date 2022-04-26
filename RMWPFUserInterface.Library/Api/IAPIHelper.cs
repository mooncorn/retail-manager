using RMWPFUserInterface.Library.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace RMWPFUserInterface.Library.Api
{
    public interface IAPIHelper
    {
        HttpClient ApiClient { get; }
        Task<AuthenticatedUserModel> Authenticate(string username, string password);
        Task GetLoggedInUserDetails(string token);
    }
}