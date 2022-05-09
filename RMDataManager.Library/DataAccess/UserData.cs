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
    public class UserData
    {
        private IConfiguration _config;

        public UserData(IConfiguration config)
        {
            _config = config;
        }

        public UserDBModel GetUserById(string Id)
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess(_config);

            var parameters = new { Id = Id };

            return sqlDataAccess.LoadData<UserDBModel, dynamic>("dbo.spUserLookup", parameters, "RMData").First();
        }
    }
}
