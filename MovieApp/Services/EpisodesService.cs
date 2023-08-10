using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Services
{
    public class EpisodesService : IEpisodesService
    {
        private readonly MovieAppContext db;
        private readonly ISeasonService seasonService;

        public EpisodesService(MovieAppContext _db, ISeasonService _seasonService)
        {
            db = _db;
            seasonService = _seasonService;
        }

        public async Task<Episode> GetByID(int id)
        {
            return await db.Eposides.FirstOrDefaultAsync(s => s.EpisodeID == id);
        }

        public async Task<Episode> Add(Episode Episode)
        {
            await db.Eposides.AddAsync(Episode);
            await db.SaveChangesAsync();
            return Episode;
        }

        public async Task AddToSeries(int id, Episode Episode)
        {
            var season = await seasonService.GetByID(id);
            season.Episodes.Add(Episode);
            await db.SaveChangesAsync();
        }

        public async Task Update(Episode Episode)
        {
            db.Eposides.Update(Episode);
            await db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var Episode = await GetByID(id);
            db.Eposides.Remove(Episode);
            await db.SaveChangesAsync();
        }

        public void EntryDetached(Episode episode)
        {
            db.Entry(episode).State = EntityState.Detached;
        }

        public async Task SaveChanges()
        {
            await db.SaveChangesAsync();
        }
    }
}
