using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using System.Collections.Generic;

namespace MovieApp.Services
{
    public class MovieGenriesService : IMovieGenriesService
    {
        private MovieAppContext _db;
        private IMovieService _movies;

        public MovieGenriesService(MovieAppContext db, IMovieService movies)
        {
            _db = db;
            _movies = movies;
        }
        public async Task<List<Genrie>> GetGenrieIn(int id)
        {
            var movie = await _movies.GetById(id);
            return movie.Genries.OrderBy(a => a.GenrieName).ToList();
        }

        public async Task<List<Genrie>> GetGenrieOut(int id)
        {
            var genries = await GetGenrieIn(id);
            var allgenries = await _db.Genries.ToListAsync();
            return allgenries.Except(genries).OrderBy(a => a.GenrieName).ToList();
        }

        public async Task RemoveGenrie(int id, int[] GenrieIds)
        {
            var movie = await _movies.GetById(id);
            foreach (var item in GenrieIds)
            {
                var genrie = await _db.Genries.FirstOrDefaultAsync(g => g.GenrieID == item);
                movie.Genries.Remove(genrie);
            }
            await _db.SaveChangesAsync();
        }

        public async Task UpdateGenrie(int id, int[] GenrieIds)
        {
            var movie = await _movies.GetById(id);
            foreach (var item in GenrieIds)
            {
                var genrie = await _db.Genries.FirstOrDefaultAsync(g => g.GenrieID == item);
                movie.Genries.Add(genrie);
            }
            await _db.SaveChangesAsync();
        }
    }
}
