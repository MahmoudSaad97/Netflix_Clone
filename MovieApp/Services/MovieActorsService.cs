using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Services
{
    public class MovieActorsService : IMovieActorsService
    {
        private MovieAppContext _db;
        private IMovieService _movies;

        public MovieActorsService(MovieAppContext db, IMovieService movies)
        {
            _db = db;
            _movies = movies;
        }
        public async Task<List<Actor>> GetActorIn(int id)
        {
            var movie = await _movies.GetById(id);
            return movie.Actors.OrderBy(a => a.ActorName).ToList();
        }

        public async Task<List<Actor>> GetActorOut(int id)
        {
            var actors = await GetActorIn(id);
            var allActors = await _db.Actors.ToListAsync();
            return allActors.Except(actors).OrderBy(a => a.ActorName).ToList();
        }

        public async Task RemoveActor(int id, int[] ActorIds)
        {
            var movie = await _movies.GetById(id);
            foreach (var item in ActorIds)
            {
                var actor = await _db.Actors.FirstOrDefaultAsync(g => g.ActorID == item);
                movie.Actors.Remove(actor);
            }
            await _db.SaveChangesAsync();
        }

        public async Task UpdateActor(int id, int[] ActorIds)
        {
            var movie = await _movies.GetById(id);
            foreach (var item in ActorIds)
            {
                var actor = await _db.Actors.FirstOrDefaultAsync(g => g.ActorID == item);
                movie.Actors.Add(actor);
            }
            await _db.SaveChangesAsync();
        }
    }
}
