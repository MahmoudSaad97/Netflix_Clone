using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface IProfileUserService
    {
        public Task<List<ProfileUser>> GetProfileByUserId(int userid);
        public Task<ProfileUser> GetUserByProfileId(int profileid);
    }
}
