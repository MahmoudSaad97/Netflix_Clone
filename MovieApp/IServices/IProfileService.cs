using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface IProfileService
    {
        public Task<List<Profile>> GetProfileByUserId(int userId);
        public Task<List<Profile>> GetAll();
        public Task<Profile> GetById(int id);
        public Task<Profile> Add(Profile Profile);
        public Task Update(Profile Profile);
        public Task DeleteById(int id);
    }
}
