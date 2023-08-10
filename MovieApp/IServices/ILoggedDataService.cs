namespace MovieApp.IServices
{
    public interface ILoggedDataService
    {
        public int LoggedUserId();
        public Task<int> GetRoleIdByName(string roleName);
        public Task<bool> checkUserAccessByRoleName(int? userId, string roleName);
        public Task<string> GetUserFullName();
    }
}
