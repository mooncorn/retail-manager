using RMWPFUserInterface.Models;
using System.Threading.Tasks;

namespace RMWPFUserInterface.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
    }
}