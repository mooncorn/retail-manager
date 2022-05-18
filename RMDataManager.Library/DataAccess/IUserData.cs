using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public interface IUserData
    {
        void CreateUser(UserDBModel user);
        UserDBModel GetUserById(string Id);
    }
}