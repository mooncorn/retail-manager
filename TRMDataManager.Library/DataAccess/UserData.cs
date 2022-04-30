using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class UserData
    {
        public UserDBModel GetUserById(string Id)
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess();

            var parameters = new { Id = Id };

            return sqlDataAccess.LoadData<UserDBModel, dynamic>("dbo.spUserLookup", parameters, "RMData").First();
        }
    }
}
