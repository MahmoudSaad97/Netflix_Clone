using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace MovieApp.Services
{
    public class ProfileService: IProfileService
    {
        MovieAppContext db;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILoggedDataService loggedDataService;
        private readonly IProfileUserService profileUserService;

        public ProfileService(MovieAppContext _db, IHttpContextAccessor _httpContextAccessor, ILoggedDataService _loggedDataService, IProfileUserService _profileUserService)
        {
            db = _db;
            httpContextAccessor = _httpContextAccessor;
            loggedDataService = _loggedDataService;
            profileUserService = _profileUserService;
        }

        public async Task<List<Profile>> GetProfileByUserId(int userId)
        {
            return await db.ProfilesUsers.Where(a => a.UserId == userId).Select(a => a.Profile).ToListAsync();
        }

        public async Task<List<Profile>> GetAll()
        {
            return await db.Profiles.Include(a => a.ProfileUsers)
                .Include(a=> a.Movies).Include(a=>a.MovieViewHistories).Include(a=>a.SeriesViewHistories).ToListAsync();
        }
        public async Task<Profile> GetById(int id)
        {
            Profile Prof = await db.Profiles.Include(a => a.ProfileUsers).Include(a => a.Movies).Include(a=>a.Series)
                .Include(a=>a.SeriesViewHistories).ThenInclude(s=>s.Series)
                .Include(a => a.MovieViewHistories).ThenInclude(h=>h.Movie).FirstOrDefaultAsync(a => a.ProfileID == id);
            return Prof;
        }
        public async Task<Profile> Add(Profile Profile)
        {
            int userId = loggedDataService.LoggedUserId();
            int ProfileCount = profileUserService.GetProfileByUserId(userId).Result.Count;

            if (ProfileCount < 5) // Max 3
            {
                await db.Profiles.AddAsync(Profile);
                await db.SaveChangesAsync();

                int profileId = Profile.ProfileID;
                await db.ProfilesUsers.AddAsync(new ProfileUser() { ProfileId = profileId, UserId = userId });
                await db.SaveChangesAsync();
            }

            return Profile;
        }
        public async Task Update(Profile Profile)
        {
            Profile profOld = await GetById(Profile.ProfileID);
            if (profOld != null)
            {
                profOld.ProfileName = Profile.ProfileName;
            }
            await db.SaveChangesAsync();
        }
        public async Task DeleteById(int id)
        {
            db.Profiles.Remove(await GetById(id));
            await db.SaveChangesAsync();
        }
    }
}
