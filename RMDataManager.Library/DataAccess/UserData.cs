using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public class UserData : IUserData
    {
        private ISqlDataAccess _sqlDataAccess;

        public UserData(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public UserDBModel GetUserById(string Id)
        {
            return _sqlDataAccess.LoadData<UserDBModel, dynamic>("dbo.spUser_Lookup", new { Id }, "RMData").First();
        }

        public void CreateUser(UserDBModel user)
        {
            _sqlDataAccess.SaveData("dbo.spUser_Insert", new { user.Id, user.FirstName, user.LastName, user.EmailAddress }, "RMData");
        }
    }
}
