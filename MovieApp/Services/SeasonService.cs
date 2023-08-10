using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using System.Collections.Generic;

namespace MovieApp.Services
{
    public class SeasonService : ISeasonService
    {
        private readonly MovieAppContext db;
        private readonly ISeriesService seriesService;

        public SeasonService(MovieAppContext _db, ISeriesService _seriesService)
        {
            db = _db;
            seriesService = _seriesService;
        }

        public async Task<Season> GetByID(int id)
        {
            Season season = await db.Seasons.FirstOrDefaultAsync(s => s.SeasonID == id);
            return season;
        }

        public async Task<Season> Add(Season season)
        {
            await db.Seasons.AddAsync(season);
            await db.SaveChangesAsync();
            return season;
        }

        public async Task AddToSeries(int id, Season season)
        {
            var serie = await seriesService.GetById(id);
            serie.Seasons.Add(season);
            await db.SaveChangesAsync();
        }

        public async Task Update(Season season)
        {
            db.Seasons.Update(season);
            await db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var season = await GetByID(id);
            db.Seasons.Remove(season);
            await db.SaveChangesAsync();
        }

    }
}
