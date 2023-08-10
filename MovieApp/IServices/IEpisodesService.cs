using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface IEpisodesService
    {
        public Task<Episode> GetByID(int id);
        public Task<Episode> Add(Episode t);
        public Task AddToSeries(int id, Episode t);
        public Task Update(Episode t);
        public Task Delete(int id);

        public void EntryDetached(Episode episode);
        public Task SaveChanges();

    }
}
