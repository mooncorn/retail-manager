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
        private IUserData _userData;
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ILogger<UserController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager, IUserData userData)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _userData = userData;
        }

        // TODO: Use Model Validation
        public record UserRegistrationModel(string FirstName, string LastName, string Email, string Password);

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationModel userInfo)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(userInfo.Email);

                if (existingUser != null)
                    return BadRequest("An account with this email address already exists.");

                IdentityUser newUser = new()
                {
                    Email = userInfo.Email,
                    EmailConfirmed = true, // TODO: Send an email to confirm account
                    UserName = userInfo.Email
                };

                IdentityResult result = await _userManager.CreateAsync(newUser, userInfo.Password);

                if (result.Succeeded)
                {
                    var createdUserIdentity = await _userManager.FindByEmailAsync(userInfo.Email);

                    if (createdUserIdentity == null)
                        return BadRequest();

                    UserDBModel userModel = new()
                    {
                        Id = createdUserIdentity.Id,
                        FirstName = userInfo.FirstName,
                        LastName = userInfo.LastName,
                        EmailAddress = createdUserIdentity.Email
                    };

                    _userData.CreateUser(userModel);

                    return Ok();
                }
                    
            }

            return BadRequest();
        }

        [HttpGet]
        public UserDBModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _userData.GetUserById(userId);
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
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IdentityUser user = await _userManager.FindByIdAsync(userRolePair.UserId);

            _logger.LogInformation("Admin {Admin} added user {User} to role {Role}",
                loggedInUserId, user.Id, userRolePair.RoleName);

            await _userManager.AddToRoleAsync(user, userRolePair.RoleName);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/UnassignRole")]
        public async Task UnassignRole(UserRolePairModel userRolePair)
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IdentityUser user = await _userManager.FindByIdAsync(userRolePair.UserId);

            _logger.LogInformation("Admin {Admin} removed user {User} from role {Role}",
                loggedInUserId, user.Id, userRolePair.RoleName);

            await _userManager.RemoveFromRoleAsync(user, userRolePair.RoleName);
        }
    }
}
