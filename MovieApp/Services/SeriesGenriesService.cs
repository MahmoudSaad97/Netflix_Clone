using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using System.Collections.Generic;

namespace MovieApp.Services
{
    public class SeriesGenriesService : ISeriesGenriesService
    {
        private readonly MovieAppContext db;
        private readonly ISeriesService seriesService;

        public SeriesGenriesService(MovieAppContext _db, ISeriesService _seriesService)
        {
            db = _db;
            seriesService = _seriesService;
        }
        public async Task<List<Genrie>> GetGenrieIn(int id)
        {
            var serie = await seriesService.GetById(id);
            return serie.Genries.OrderBy(a => a.GenrieName).ToList();
        }

        public async Task<List<Genrie>> GetGenrieOut(int id)
        {
            var genries = await GetGenrieIn(id);
            var allgenries = await db.Genries.ToListAsync();
            return allgenries.Except(genries).OrderBy(a => a.GenrieName).ToList();
        }

        public async Task RemoveGenrie(int id, int[] GenrieIds)
        {
            var serie = await seriesService.GetById(id);
            foreach (var item in GenrieIds)
            {
                var genrie = await db.Genries.FirstOrDefaultAsync(g => g.GenrieID == item);
                serie.Genries.Remove(genrie);
            }
            await db.SaveChangesAsync();
        }

        public async Task UpdateGenrie(int id, int[] GenrieIds)
        {
            var serie = await seriesService.GetById(id);
            foreach (var item in GenrieIds)
            {
                var genrie = await db.Genries.FirstOrDefaultAsync(g => g.GenrieID == item);
                serie.Genries.Add(genrie);
            }
            await db.SaveChangesAsync();
        }
    }
}
