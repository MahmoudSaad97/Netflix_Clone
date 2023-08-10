using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface ISeasonService
    {
        public Task<Season> GetByID(int id);
        public Task<Season> Add(Season t);
        public Task AddToSeries(int id, Season t);
        public Task Update(Season t);
        public Task Delete(int id);
    }
}
