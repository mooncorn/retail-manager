using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using Microsoft.AspNet.Identity;
using RMDataManager.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RMDataManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        [HttpGet]
        public UserDBModel GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData();
            return data.GetUserById(userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAll")]
        public List<ApplicationUserModel> GetAll()
        {
            List<ApplicationUserModel> userList = new List<ApplicationUserModel>();

            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var users = userManager.Users.ToList();

                var roles = context.Roles.ToList();

                foreach (var user in users)
                {
                    ApplicationUserModel appUser = new ApplicationUserModel
                    {
                        Id = user.Id,
                        Email = user.Email
                    };

                    foreach (var userRole in user.Roles)
                    {
                        string roleName = roles.Where(role => role.Id == userRole.RoleId).First().Name;
                        string roleId = userRole.RoleId;
                        appUser.Roles.Add(roleId, roleName);
                    }

                    userList.Add(appUser);
                }
            }

            return userList;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Roles.ToDictionary(role => role.Id, role => role.Name);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AssignRole")]
        public void AssignRole(UserRolePairModel userRolePair)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.AddToRole(userRolePair.UserId, userRolePair.RoleName);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/UnassignRole")]
        public void UnassignRole(UserRolePairModel userRolePair)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.RemoveFromRole(userRolePair.UserId, userRolePair.RoleName);
            }
        }
    }
}