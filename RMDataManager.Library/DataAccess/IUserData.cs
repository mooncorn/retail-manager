using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public interface IUserData
    {
        UserDBModel GetUserById(string Id);
    }
}