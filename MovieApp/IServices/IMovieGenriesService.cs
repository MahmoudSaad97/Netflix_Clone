using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface IMovieGenriesService
    {
        public Task<List<Genrie>> GetGenrieIn(int id);
        public Task<List<Genrie>> GetGenrieOut(int id);
        public Task UpdateGenrie(int id, int[] GenrieIds);
        public Task RemoveGenrie(int id, int[] GenrieIds);
    }
}
