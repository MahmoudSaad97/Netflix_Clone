using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface IMovieService
    {
        public Task<List<Movie>> GetAll();
        public Task<Movie> GetById(int id);
        public Task<Movie> LastInserted();
        public Task<Movie> Add(Movie t);
        public Task Update(Movie movie);
        public Task Delete(int id);
        public Task<List<Movie>> GetByActor(string actorName);
        public Task<List<Movie>> GetByGenrie(string genrieName);
        public Task<List<Movie>> GetBySearch(string searchName, int skip, int take);

        public Task<List<Movie>> GetTop();
        public Task<List<Movie>> GetTrend(int skip,int take);
        public Task<List<Movie>> GetNewest(int skip,int take);
        public Task<List<Movie>> Similar(Movie movie);
        public void EntityDetach(Movie movie);
        public Task<List<Movie>> LoadMovies(int skip, int take);
        public Task SetRate(int id, int movieRate);

    }
}
