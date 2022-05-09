using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RMApi.Data;
using RMApi.Models;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System.Security.Claims;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(IConfiguration config, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public UserDBModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserData data = new UserData(_config);
            return data.GetUserById(userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAll")]
        public List<ApplicationUserModel> GetAll()
        {
            List<ApplicationUserModel> userList = new List<ApplicationUserModel>();

            var users = _context.Users.ToList();
            var userRoles = from userRole in _context.UserRoles
                            join role in _context.Roles on userRole.RoleId equals role.Id
                            select new { userRole.RoleId, userRole.UserId, RoleName = role.Name };

            foreach (var user in users)
            {
                ApplicationUserModel appUser = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = userRoles
                                .Where(userRole => userRole.UserId == user.Id)
                                .ToDictionary((role) => role.RoleId, (role) => role.RoleName)
                };

                userList.Add(appUser);
            }

            return userList;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            return _context.Roles.ToDictionary(role => role.Id, role => role.Name);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AssignRole")]
        public async Task AssignRole(UserRolePairModel userRolePair)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userRolePair.UserId);
            await _userManager.AddToRoleAsync(user, userRolePair.RoleName);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/UnassignRole")]
        public async Task UnassignRole(UserRolePairModel userRolePair)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userRolePair.UserId);
            await _userManager.RemoveFromRoleAsync(user, userRolePair.RoleName);
        }
    }
}
