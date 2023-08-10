using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MovieApp.Services
{
    public class MovieService : IMovieService
    {
        private readonly MovieAppContext db;

        public MovieService(MovieAppContext _db)
        {
            db = _db;
        }

        public async Task<List<Movie>> GetAll()
        {
            return await db.Movies.OrderByDescending(m => m.ReleaseDate.Value.Year).ToListAsync();
        }

        public async Task<Movie> GetById(int id)
        {
            return await db.Movies.FirstOrDefaultAsync(m => m.MovieID == id);
        }

        public async Task<Movie> LastInserted()
        {
            return await db.Movies.OrderByDescending(m => m.MovieID).FirstOrDefaultAsync();
        }

        public async Task Delete(int id)
        {
            var movie = await db.Movies.FirstOrDefaultAsync(m => m.MovieID == id);
            db.Movies.Remove(movie);
            await db.SaveChangesAsync();
        }

        public async Task Update(Movie movie)
        {
            db.Movies.Update(movie);
            await db.SaveChangesAsync();
        }

        public async Task<Movie> Add(Movie movie)
        {
            db.Movies.AddAsync(movie);
            await db.SaveChangesAsync();
            return movie;
        }

        public async Task<List<Movie>> GetTrend(int skip,int take)
        {
            var averageViews = await db.Movies.AverageAsync(m => m.Views);
            return await db.Movies.Where(m => m.Views > averageViews).OrderByDescending(m => m.Views).Skip(skip).Take(take).ToListAsync();
        }
        public async Task<List<Movie>> GetNewest(int skip,int take)
        {
            try
            {
                var averageReleaseDate = await db.Movies.AverageAsync(m => m.ReleaseDate.Value.Year);
                var movies = await db.Movies.Where(m => m.ReleaseDate.Value.Year >= averageReleaseDate).OrderByDescending(m => m.ReleaseDate.Value.Year).Skip(skip).Take(take).ToListAsync();
                return movies;

            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Movie>> GetTop()
        {
            var averageRate = await db.Movies.AverageAsync(m => m.Rate);
            return await db.Movies.Where(m => m.Rate > averageRate).OrderByDescending(m=>m.Rate / m.votes).Take(10).ToListAsync();
        }

        public async Task<List<Movie>> GetByGenrie(string genrieName)
        {
            var movies = await db.Movies
                .Where(m => m.Genries.Any(g => g.GenrieName == genrieName))
                .ToListAsync();
            return movies;
        }

        public async Task<List<Movie>> GetBySearch(string searchName, int skip, int take)
        {
            var movies = await db.Movies
                .Where(m => m.MovieName.Contains(searchName) ||
                m.Actors.Any(a => a.ActorName.Contains(searchName)) ||
                m.Genries.Any(a => a.GenrieName.Contains(searchName))).Skip(skip).Take(take)
                .ToListAsync();
            return movies;
        }
        public async Task<List<Movie>> GetByActor(string actorName)
        {
            var movies = await db.Movies
                .Where(m => m.Actors.Any(a => a.ActorName.Contains(actorName))).ToListAsync();
            return movies;
        }

        public async Task<List<Movie>> Similar(Movie movie)
        {
            var movieGenres = movie.Genries.Select(g => g.GenrieID).ToList();
            var similarMovies = await db.Movies
                  .Where(m => m.MovieID != movie.MovieID && m.Genries.Any(g => movieGenres.Contains(g.GenrieID)))
                  .ToListAsync();
            return similarMovies;
        }

        public void EntityDetach(Movie movie)
        {
            db.Entry(movie).State = EntityState.Detached;
        }

        public async Task<List<Movie>> LoadMovies(int skip, int take)
        {
            var movies = await db.Movies.Skip(skip).Take(take).ToListAsync();
            return movies;
        }

        public async Task SetRate(int id, int movieRate)
        {
            var movie = await GetById(id);
            movie.votes = ++movie.votes;
            movie.Rate += movieRate;
            await db.SaveChangesAsync();
        }
    }
}
