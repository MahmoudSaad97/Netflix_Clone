using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Services
{
    public class ProfileUserService: IProfileUserService
    {
        MovieAppContext db;
        public ProfileUserService(MovieAppContext _db)
        {
            db = _db;
        }
        public async Task<List<ProfileUser>> GetProfileByUserId(int userid)
        {
            return await db.ProfilesUsers.Where(a => a.UserId == userid).ToListAsync();
        }
        public async Task<ProfileUser> GetUserByProfileId(int profileid)
        {
            return await db.ProfilesUsers.FirstOrDefaultAsync(a => a.ProfileId == profileid);
        }
    }
}
