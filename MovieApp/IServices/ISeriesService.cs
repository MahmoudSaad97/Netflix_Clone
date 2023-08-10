using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface ISeriesService
    {
        public Task<List<Series>> GetAll();
        public Task<Series> GetById(int id);
        public Task<Series> Add(Series t);
        public Task Update(Series Series);
        public Task Delete(int id);
        public Task<List<Series>> GetByActor(string actorName);
        public Task<List<Series>> GetByGenrie(string genrieName);
        public Task<List<Series>> GetBySearch(string searchName, int skip, int take);

        public Task<List<Series>> GetTop();
        public Task<List<Series>> GetTrend(int skip,int take);
        public Task<List<Series>> GetNewest(int skip,int take);
        public Task<List<Series>> Similar(Series series);
        public void SeriesDetach(Series series);
        public Task<List<Series>> LoadSeries(int skip, int take);
        public Task SetRate(int id, int movieRate);


    }
}
