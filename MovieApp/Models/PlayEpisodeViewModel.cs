namespace MovieApp.Models
{
    public class PlayEpisodeViewModel
    {
        public int SeriesID { get; set; }
        public List<Episode> Episodes { get; set; }
        public Episode PlayedEpisode { get; set; }
        public int NextEpisode { get; set; }
        public int PreviousEposide { get; set; }
        public List<Season> AllSeasons { get; set; }
        public List<Season> RestSeasons { get; set; }
        public List<EpisodeViewHistory> EpisodesHistory { get; set; }
    }
}
