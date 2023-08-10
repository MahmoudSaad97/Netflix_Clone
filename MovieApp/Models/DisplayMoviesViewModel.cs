namespace MovieApp.Models
{
    public class DisplayMoviesViewModel
    {
        #region Fields
        public Movie PlayedMovie { get; set; }
        public Series PlayedSeries { get; set; }
        public Episode PlayedEposide { get; set; }
        public Season Season { get; set; }
        public List<Season> restSeasons { get; set; }
        public List<Season> AllSeasons { get; set; }
        public List<Movie> NewestMovies { get; set; }
        public List<Movie> TrendingMovies { get; set; }
        public List<Movie> TopMovies { get; set; }
        public List<Movie> AllMovies { get; set; }
        public List<Movie>? wishListMovies { get; set; }
        public List<MovieViewHistory>? ViewHistoryMovies { get; set; }
        public List<Series> NewestSeries { get; set; }
        public List<Series> TrendingSeries { get; set; }
        public List<Series> TopSeries { get; set; }
        public List<Series> AllSeries { get; set; }
        public List<Series>? wishListSeries { get; set; }
        public List<SeriesViewHistory>? ViewHistorySeries { get; set; }

        #endregion

    }
}
