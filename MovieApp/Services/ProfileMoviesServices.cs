using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Services
{
    public class ProfileMoviesServices
    {
        private readonly MovieAppContext db;
        private readonly IProfileService profiles;

        public ProfileMoviesServices(MovieAppContext _db,IProfileService _profiles)
        {
            db = _db;
            profiles = _profiles;
        }

        public async Task AddMovieToWishList(int id,Movie movie)
        {
            var profile = await profiles.GetById(id);
            profile.Movies?.Add(movie);
            await db.SaveChangesAsync();
        }         
        public async Task DeleteMovieFromList(int id,Movie movie)
        {
            var profile = await profiles.GetById(id);
            profile.Movies?.Remove(movie);
            await db.SaveChangesAsync();
        }  
        
        public async Task<List<Movie>> WhishlistMovies(int? id)
        {
            if (id == null)
                return null;
            
            var profile = await profiles.GetById((int)id);
            if (profile.Movies != null)
                return profile.Movies.ToList();

            return null;
        }

        public async Task AddToHistory(int id,Movie movie)
        {
            var profile = await profiles.GetById(id);
            if(!profile.MovieViewHistories.Any(m=> m.MovieID == movie.MovieID))
            {
            var history = new MovieViewHistory()
            {
                MovieID = movie.MovieID,
                ProfileID = id,
                Date = DateTime.Now,
            };
                //await db.MovieviewHistories.AddAsync(history);
                profile.MovieViewHistories?.Add(history);

            }
            await db.SaveChangesAsync();
        }

        public async Task<List<MovieViewHistory>> ViewHistoryMovies(int id)
        {
            var profile = await profiles.GetById(id);

            return profile.MovieViewHistories.ToList();
        }

        public async Task UpdateProgress(int id, int mid, int prog)
        {
            var profile = await profiles.GetById(id);
            var movie = profile.MovieViewHistories.Where(m => m.MovieID == mid).FirstOrDefault();
            if (movie != null)
            {
                movie.ProgressMinutes = prog;
                await db.SaveChangesAsync();
            }
        }

        public async Task AddRatedMovie(Profile profile, int mid, int movieRated)
        {
            if (profile.RatedMovies == null || !profile.RatedMovies.Any(m => m.MovieID == mid))
            {
                RatedMovie movie = new RatedMovie()
                {
                    MovieID = mid,
                    ProfileID = profile.ProfileID,
                    Date = DateTime.Now,
                    Rateing = movieRated
                };
                profile.RatedMovies.Add(movie);
                await db.SaveChangesAsync();
            }
        }
    }
}
