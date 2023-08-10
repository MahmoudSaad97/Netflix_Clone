using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface IActorService
    {
        public Task<List<Actor>> GetAll();
        public Task<Actor> GetById(int id);
        public Task<Actor> Add(Actor Actor);
        public Task Update(Actor Actor);
        public Task DeleteById(int id);
    }
}
