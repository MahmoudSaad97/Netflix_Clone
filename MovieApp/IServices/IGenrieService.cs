using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface IGenrieService
    {
        public Task<List<Genrie>> GetAll();
        public Task<Genrie> GetById(int id);
        public Task<Genrie> Add(Genrie Genrie);
        public Task Update(Genrie Genrie);
        public Task DeleteById(int id);
    }
}
