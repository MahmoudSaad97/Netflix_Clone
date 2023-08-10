using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using System.Security.Claims;

namespace MovieApp.Services
{
    public class LoggedDataService: ILoggedDataService
    {
        private readonly MovieAppContext db;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserService userService;

        public LoggedDataService(MovieAppContext _db, IHttpContextAccessor _httpContextAccessor, IUserService _userService)
        {
            db = _db;
            httpContextAccessor = _httpContextAccessor;
            userService = _userService;
        }
        public int LoggedUserId()
        {
            var User = httpContextAccessor.HttpContext.User;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return int.Parse(claim.Value);
        }
        public async Task<int> GetRoleIdByName(string roleName)
        {
            if (roleName != "")
            {
                var userRole = await db.Roles.Where(a => a.Name == roleName).FirstOrDefaultAsync();
                if(userRole != null)
                    return userRole.Id;
            }
            return 0;
        }
        public async Task<bool> checkUserAccessByRoleName(int? userId, string roleName)
        {
            int roleId = await GetRoleIdByName(roleName);
            if (userId != null && roleName != "")
            {
                var x = await db.UserRoles.Where(a => a.RoleId == roleId && a.UserId == userId).FirstOrDefaultAsync();
                if (x != null) 
                    return true;
            }
            return false;
        }
        public async Task<string> GetUserFullName()
        {
            int userId = LoggedUserId();
            if (userId > 0)
            {
                User loggeduser = await userService.GetById(userId);
                if (loggeduser != null)
                    return loggeduser.fname + " " + loggeduser.lname;
            }
            return "";
        }
    }
}
