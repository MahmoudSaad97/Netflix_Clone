using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface IUserService
    {
        public Task<List<User>> GetAll();
        public Task<User> GetById(int id);
        public Task<User> Add(User User);
        public Task Update(User User);
        public Task DeleteById(int id);
    }
}
