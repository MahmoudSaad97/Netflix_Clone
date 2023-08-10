using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using System.Collections.Generic;

namespace MovieApp.Services
{
    public class SeriesService : ISeriesService
    {
        private readonly MovieAppContext db;

        public SeriesService(MovieAppContext _db)
        {
            db = _db;
        }
        public async Task<List<Series>> GetAll()
        {
            return await db.Series.ToListAsync();
        }

        public async Task Update(Series series)
        {
            db.Series.Update(series);
            foreach (var season in series.Seasons)
            {
                season.SeasonName = $"{series.SeriesName}: {season.SeasonName.Split(":").Last()}";
            }
            await db.SaveChangesAsync();
        }

        public async Task<Series> Add(Series series)
        {
            await db.Series.AddAsync(series);
            await db.SaveChangesAsync();
            return series;
        }

        public async Task Delete(int id)
        {
            var series = await db.Series.FirstOrDefaultAsync(m => m.SeriesID == id);
            if (series != null)
                db.Series.Remove(series);

            await db.SaveChangesAsync();
        }

        public async Task<Series> GetById(int id)
        {
            return await db.Series.FirstOrDefaultAsync(s => s.SeriesID == id);
        }

        public async Task<List<Series>> GetByActor(string actorName)
        {
            var series = await db.Series.Where(s => s.Actors.Any(a => a.ActorName.Contains(actorName))).ToListAsync();
            return series;
        }

        public async Task<List<Series>> GetByGenrie(string genrieName)
        {
            var series = await db.Series.Where(m => m.Genries.Any(g => g.GenrieName == genrieName)).ToListAsync();
            return series;
        }
        public async Task<List<Series>> GetBySearch(string searchName, int skip, int take)
        {
            var series = await db.Series
                .Where(s => s.SeriesName.Contains(searchName) ||
                s.Actors.Any(a => a.ActorName.Contains(searchName)) ||
                s.Genries.Any(a => a.GenrieName.Contains(searchName))).Skip(skip).Take(take)
                .ToListAsync();
            return series;
        }

        public async Task<List<Series>> GetTrend(int skip,int take)
        {
            var averageViews = await db.Series.AverageAsync(m => m.Views);
            return await db.Series.Where(m => m.Views > averageViews).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<List<Series>> GetTop()
        {
            var averageRate = await db.Series.AverageAsync(m => m.Rate);
            return await db.Series.Where(m => m.Rate > averageRate).Take(10).ToListAsync();
        }

        public async Task<List<Series>> GetNewest(int skip,int take)
        {
            try
            {
                var averageReleaseDate = await db.Series.AverageAsync(m => m.ReleaseDate.Value.Year);
                var movies = await db.Series.Where(m => m.ReleaseDate.Value.Year >= averageReleaseDate).Skip(skip).Take(take).ToListAsync();
                return movies;

            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Series>> Similar(Series series)
        {
            var seriesGenres = series.Genries.Select(g => g.GenrieID).ToList();
            var similarSeries = await db.Series
                  .Where(m => m.SeriesID != series.SeriesID && m.Genries.Any(g => seriesGenres.Contains(g.GenrieID)))
                  .ToListAsync();
            return similarSeries;
        }
        public void SeriesDetach(Series series)
        {
            db.Entry(series).State = EntityState.Detached;
        }

        public async Task<List<Series>> LoadSeries(int skip, int take)
        {
            var series = await db.Series.Skip(skip).Take(take).ToListAsync();
            return series;
        }

        public async Task SetRate(int id, int movieRate)
        {
            var series = await GetById(id);
            series.Votes += 1;
            series.Rate += movieRate;
            await db.SaveChangesAsync();
        }

    }
}

