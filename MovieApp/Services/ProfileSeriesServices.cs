using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Services
{
    public class ProfileSeriesServices
    {
        private readonly MovieAppContext db;
        private readonly IProfileService profiles;

        public ProfileSeriesServices(MovieAppContext _db,IProfileService _profiles)
        {
            db = _db;
            profiles = _profiles;
        }

        public async Task AddSeriesToWishList(int id, Series series)
        {
            var profile = await profiles.GetById(id);
            profile.Series?.Add(series);
            await db.SaveChangesAsync();
        }
        public async Task DeleteSeriesFromList(int id, Series series)
        {
            var profile = await profiles.GetById(id);
            profile.Series?.Remove(series);
            await db.SaveChangesAsync();
        }

        public async Task<List<Series>> WhishlistSeries(int id)
        {
            if (id == null)
                return null;

            var profile = await profiles.GetById((int)id);
            if (profile.Series != null)
                return profile.Series.ToList();

            return null;
        }

        public async Task AddToHistory(int id, Series series)
        {
            var profile = await profiles.GetById(id);
            if (!profile.SeriesViewHistories.Any(s=>s.SeriesID == series.SeriesID))
            {
                var history = new SeriesViewHistory()
                {
                    SeriesID = series.SeriesID,
                    ProfileID = id,
                    Date = DateTime.Now,
                };

            profile.SeriesViewHistories?.Add(history);
            }
            await db.SaveChangesAsync();
        }

        public async Task<List<SeriesViewHistory>> ViewHistoryMovies(int id)
        {
            var profile = await profiles.GetById(id);

            return profile.SeriesViewHistories.ToList();
        }

        public async Task AddRatedSeries(Profile profile, int sid, int seriesRate)
        {
            if (profile.RatedSeries == null || !profile.RatedSeries.Any(s => s.SeriesID == sid))
            {
                RatedSeries series = new RatedSeries()
                {
                    SeriesID = sid,
                    ProfileID = profile.ProfileID,
                    Date = DateTime.Now,
                    Rateing = seriesRate
                };
                profile.RatedSeries.Add(series);
                await db.SaveChangesAsync();
            }
        }
        public async Task AddEpisodeHistory(int id, int eid, int sid)
        {
            var profile = await profiles.GetById(id);
            var serieshistory = profile.SeriesViewHistories.FirstOrDefault(s => s.SeriesID == sid);
            if (serieshistory != null)
            {
                if (serieshistory.EpisodeViewHistories == null || !serieshistory.EpisodeViewHistories.Any(e => e.EpisodeID == eid))
                {
                    EpisodeViewHistory episodehistory = new EpisodeViewHistory()
                    {
                        EpisodeID = eid,
                        HistoryID = serieshistory.HistoryID,
                        Date = DateTime.Now,
                        ProgressMinutes = 0
                    };
                    serieshistory.EpisodeViewHistories.Add(episodehistory);
                }
            }
            await db.SaveChangesAsync();
        }

        public async Task UpdateProgress(int id, int sid, int eid, int prog)
        {
            var profile = await profiles.GetById(id);
            var serieshistory = profile.SeriesViewHistories.FirstOrDefault(s => s.SeriesID == sid);
            var episode = serieshistory?.EpisodeViewHistories.FirstOrDefault(e => e.EpisodeID == eid);
            if (episode != null)
            {
                episode.ProgressMinutes = prog;
                await db.SaveChangesAsync();
            }
        }
    }
}
