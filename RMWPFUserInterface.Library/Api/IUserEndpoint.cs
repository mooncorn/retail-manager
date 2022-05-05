using RMWPFUserInterface.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMWPFUserInterface.Library.Api
{
    public interface IUserEndpoint
    {
        Task AssignRole(string userId, string roleName);
        Task<List<UserModel>> GetAll();
        Task<Dictionary<string, string>> GetAllRoles();
        Task UnassignRole(string userId, string roleName);
    }
}