using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http;
using MovieApp.IServices;
using MovieApp.Models;
using MovieApp.Services;
using System.Linq;
using System.Net;
using System.Text.Json;

namespace MovieApp.Controllers
{
    [Authorize(Roles = "Basic,Admin")]
    public class MainController : Controller
    {
        #region Fields And Ctor
        private readonly IProfileService profileService;
        private readonly ILoggedDataService loggedDataService;
        private readonly IProfileUserService profileUserService;
        private readonly IMovieService movieService;
        private readonly ISeriesService seriesService;
        private readonly IUserService userService;
        private readonly ProfileMoviesServices profilemovies;
        private readonly SeasonService seasons;
        private readonly ProfileSeriesServices profileSeries;
        private readonly IEpisodesService eposides;
        private readonly IPaymentService paymentService;
        DisplayMoviesViewModel viewModel = new DisplayMoviesViewModel();

        public MainController(ILoggedDataService _loggedDataService, IProfileUserService _profileUserService,
            IMovieService _movieService, IProfileService _profileService, IUserService _userService,
            ISeriesService _seriesService, ProfileMoviesServices _porfilemovies, ProfileSeriesServices _profileSeries, 
            SeasonService _seasons, IEpisodesService _eposides, IPaymentService _paymentService)
        {
            loggedDataService = _loggedDataService;
            profileUserService = _profileUserService;
            movieService = _movieService;
            profileService = _profileService;
            userService = _userService;
            seriesService = _seriesService;
            profilemovies = _porfilemovies;
            seasons = _seasons;
            profileSeries = _profileSeries;
            eposides = _eposides;
            paymentService = _paymentService;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                Controller? controller = context.Controller as Controller;
                int userId = loggedDataService.LoggedUserId();
                bool checkAdmin = await loggedDataService.checkUserAccessByRoleName(userId, "Admin");
                var fullname = (await loggedDataService.GetUserFullName());
                if (fullname != null)
                    ViewBag.loggedUserFullName = fullname;

                ViewBag.Profiles = await profileService.GetProfileByUserId(userId);
                ViewBag.lastProfileId = (int?)HttpContext.Session.GetInt32("ProfileId");
                ViewBag.checkAdmin = checkAdmin;

                if (controller != null)
                {
                    if (!checkAdmin)
                    {
                        if (DateTime.Now > await paymentService.GetDateTimeExpireByLoggedUserId())
                        {
                            controller.HttpContext.Response.Clear();
                            controller.HttpContext.Response.Redirect("/Renew/Index");
                        }
                    }
                }
            }
            finally
            {
                await base.OnActionExecutionAsync(context, next);
            }
        }
        #endregion


        #region actionsWithVIews
        public async Task<IActionResult> Index(int? id)
        {

            var checkUserProfile = await CheckUserProfile(id);
            if (checkUserProfile != null)
                return checkUserProfile;

            ViewBag.type = "all";
            viewModel.PlayedMovie = await movieService.LastInserted();
            viewModel.NewestMovies = await movieService.GetNewest(0, 10);
            viewModel.TrendingMovies = await movieService.GetTrend(0, 10);
            viewModel.TopMovies = await movieService.GetTop();
            viewModel.wishListMovies = await profilemovies.WhishlistMovies((int)id);
            viewModel.ViewHistoryMovies = await profilemovies.ViewHistoryMovies((int)id);
            viewModel.AllMovies = viewModel.NewestMovies.Union(viewModel.TrendingMovies ?? new List<Movie>()).Union(viewModel.TopMovies ?? new List<Movie>()).Union(viewModel.wishListMovies ?? new List<Movie>()).Distinct().ToList();
            


            viewModel.NewestSeries = await seriesService.GetNewest(0, 10);
            viewModel.TrendingSeries = await seriesService.GetTrend(0, 10);
            viewModel.TopSeries = await seriesService.GetTop();
            viewModel.wishListSeries = await profileSeries.WhishlistSeries((int)id);
            viewModel.ViewHistoryMovies = await profilemovies.ViewHistoryMovies((int)id);
            viewModel.AllSeries = viewModel.NewestSeries.Union(viewModel.TopSeries ?? new List<Series>()).Union(viewModel.TrendingSeries ?? new List<Series>()).Union(viewModel.wishListSeries ?? new List<Series>()).Distinct().ToList();
            return View(viewModel);
        }
        public async Task<IActionResult> S(int id, string searchName, string t)
        {
            var checkUserProfile = await CheckUserProfile(id);
            if (checkUserProfile != null)
                return checkUserProfile;
            if (searchName == null) return BadRequest();
            ViewBag.SearchName = searchName;
            ViewBag.type = t;
            List<Movie> searchedMovies = new List<Movie>();
            List<Series> searchedSeries = new List<Series>();
            switch (t.ToLower())
            {
                case "movies":
                    searchedMovies = await movieService.GetBySearch(searchName, 0, 20);
                    break;
                case "tvshow":
                    searchedSeries = await seriesService.GetBySearch(searchName, 0, 20);
                    break;
                case "all":
                case "both":
                    searchedSeries = await seriesService.GetBySearch(searchName, 0, 10);
                    searchedMovies = await movieService.GetBySearch(searchName, 0, 10);
                    break;
            }

            viewModel.TopMovies = searchedMovies;
            viewModel.TopSeries = searchedSeries;
            viewModel.wishListMovies = await profilemovies.WhishlistMovies((int)id);
            viewModel.wishListSeries = await profileSeries.WhishlistSeries((int)id);
            return View(viewModel);
        }
        public async Task<IActionResult> PlayMovie(int id, int mid)
        {
            var checkUserProfile = await CheckUserProfile(id);
            if (checkUserProfile != null)
                return checkUserProfile;
            ViewBag.type = "all";
            var movie = await movieService.GetById(mid);
            var profile = await profileService.GetById(id);
            if (profile == null || movie == null) return NotFound();

            if (profile.MovieViewHistories == null || !profile.MovieViewHistories.Any(m => m.MovieID == mid))
            {
                movie.Views = ++movie.Views;

                await profilemovies.AddToHistory(id, movie);
            }

            if (profile.MovieViewHistories != null && profile.MovieViewHistories.Any(m => m.MovieID == mid))
            {
                var prog = profile.MovieViewHistories.Where(m => m.MovieID == mid).FirstOrDefault();
                ViewBag.prog = prog?.ProgressMinutes;

            }
            var profilehistory = await profilemovies.ViewHistoryMovies(id);
            var movieRated = profile.RatedMovies.Any(m => m.MovieID == mid);
            ViewBag.exist = movieRated;
            viewModel.PlayedMovie = movie;
            ViewBag.Similar = await movieService.Similar(movie);
            viewModel.wishListMovies = await profilemovies.WhishlistMovies(id);
            return View(viewModel);
        }
        public async Task<IActionResult> DisplaySeason(int id, int sid)
        {
            var checkUserProfile = await CheckUserProfile(id);
            if (checkUserProfile != null)
                return checkUserProfile;

            ViewBag.type = "all";
            var season = await seasons.GetByID(sid);
            if (season == null) return NotFound();
            viewModel.Season = season;
            return View(viewModel);
        }
        public async Task<IActionResult> PlayEposide(int id, int eid)
        {
            var checkUserProfile = await CheckUserProfile(id);
            if (checkUserProfile != null)
                return checkUserProfile;
            ViewBag.type = "all";
            var profile = await profileService.GetById(id);
            var currentEpisode = await eposides.GetByID(eid);
            PlayEpisodeViewModel playEpisodeViewModel = new PlayEpisodeViewModel();

            if (profile == null || currentEpisode == null) return NotFound();
            //check if this series exist in viewhistory
            if (profile.SeriesViewHistories == null || !profile.SeriesViewHistories.Any(s => s.SeriesID == currentEpisode.Season.SeriesID))
            {
                var series = await seriesService.GetById(currentEpisode.Season.SeriesID);
                series.Views += 1;
                await profileSeries.AddToHistory(id, series);
            }
            //adding this eposide to the episodes view histroy

            await profileSeries.AddEpisodeHistory(id, eid, currentEpisode.Season.SeriesID);

            //check if this episode have progress or not and if it has store it in viewbag
            if (profile.SeriesViewHistories != null && profile.SeriesViewHistories.Any(s => s.SeriesID == currentEpisode.Season.SeriesID))
            {
                var serishistory = profile.SeriesViewHistories.FirstOrDefault(s => s.SeriesID == currentEpisode.Season.SeriesID);
                if (serishistory != null)
                {
                    playEpisodeViewModel.EpisodesHistory = serishistory.EpisodeViewHistories.ToList();
                    var prog = serishistory.EpisodeViewHistories.FirstOrDefault(s => s.EpisodeID == eid);
                    ViewBag.prog = prog?.ProgressMinutes;
                }

            }

            List<Episode> episodes = currentEpisode.Season.Episodes.OrderBy(e => e.EpNum).ToList();
            List<Season> seasons = currentEpisode.Season.Series.Seasons.OrderBy(s => s.ReleaseDate).ToList();

            int currentIndex = episodes.FindIndex(episode => episode.EpisodeID == eid);
            int previousEpisode = 0;
            int nextEpisode = 0;

            if (currentIndex >= 0)
            {
                if (currentIndex > 0)
                {
                    previousEpisode = episodes[currentIndex - 1].EpisodeID;
                }

                if (currentIndex < episodes.Count - 1)
                {
                    nextEpisode = episodes[currentIndex + 1].EpisodeID;
                }
            }


            var seriesRated = profile.RatedSeries.Any(s => s.SeriesID == currentEpisode.Season.SeriesID);
            ViewBag.exist = seriesRated;
            //ViewBag.SeriesID = currentEpisode.Season.SeriesID;
            //ViewBag.Episodes = episodes;
            //viewModel.PlayedEposide = currentEpisode;
            //ViewBag.PreviousEpisode = previousEpisode;
            //ViewBag.NextEpisode = nextEpisode;
            //viewModel.restSeasons = currentEpisode.Season.Series.Seasons.Where(s => s.SeasonID != currentEpisode.Season.SeasonID).ToList();
            //viewModel.AllSeasons = seasons;

            playEpisodeViewModel.PlayedEpisode = currentEpisode;
            playEpisodeViewModel.NextEpisode = nextEpisode;
            playEpisodeViewModel.PreviousEposide = previousEpisode;
            playEpisodeViewModel.SeriesID = currentEpisode.Season.SeriesID;
            playEpisodeViewModel.RestSeasons = currentEpisode.Season.Series.Seasons.Where(s => s.SeasonID != currentEpisode.Season.SeasonID).ToList();
            playEpisodeViewModel.AllSeasons = seasons;
            playEpisodeViewModel.Episodes = episodes;


            return View(playEpisodeViewModel);
        }
        public async Task<IActionResult> DisplaySeries(int id, int sid)
        {
            var checkUserProfile = await CheckUserProfile(id);
            if (checkUserProfile != null)
                return checkUserProfile;
            if (sid == null) return BadRequest();
            var serie = await seriesService.GetById(sid);
            if (serie == null) return NotFound();
            ViewBag.type = "all";
            viewModel.PlayedSeries = serie;
            return View(viewModel);
        }
        public async Task<IActionResult> MyList(int id)
        {
            var checkUserProfile = await CheckUserProfile(id);
            if (checkUserProfile != null)
                return checkUserProfile;
            ViewBag.type = "all";
            viewModel.wishListMovies = await profilemovies.WhishlistMovies(id);
            viewModel.wishListSeries = await profileSeries.WhishlistSeries(id);
            return View(viewModel);
        }
        public async Task<IActionResult> NewAndPopular(int id, string t, string category)
        {
            var checkUserProfile = await CheckUserProfile(id);
            if (checkUserProfile != null)
                return checkUserProfile;
            List<Movie> movies = new List<Movie>();
            List<Series> series = new List<Series>();
            switch (category.ToLower())
            {
                case "new":
                    movies = await movieService.GetNewest(0, 15);
                    series = await seriesService.GetNewest(0, 15);
                    break;
                case "trend":
                    movies = await movieService.GetTrend(0, 15);
                    series = await seriesService.GetTrend(0, 15);
                    break;
                case "all":
                    var trendM = await movieService.GetTrend(0, 10);
                    var newM = await movieService.GetNewest(0, 10);
                    var trenSeries = await seriesService.GetTrend(0, 10);
                    var newSeries = await seriesService.GetNewest(0, 10);
                    if (trendM != null && newM != null)
                        movies = trendM.Union(newM).Distinct().ToList();
                    if (trenSeries != null && newSeries != null)
                        series = trenSeries.Union(newSeries).Distinct().ToList();
                    break;
            }


            switch (t.ToLower())
            {
                case "tvshow":
                    viewModel.AllSeries = series;
                    break;
                case "movies":
                    viewModel.AllMovies = movies;
                    break;
                case "both":
                    viewModel.AllSeries = series;
                    viewModel.AllMovies = movies;
                    break;
            }
            ViewBag.type = t;
            ViewBag.category = category;
            viewModel.wishListMovies = await profilemovies.WhishlistMovies((int)id);
            viewModel.wishListSeries = await profileSeries.WhishlistSeries((int)id);
            return View(viewModel);
        }
        public async Task<IActionResult> Movies(int id)
        {
            var checkUserProfile = await CheckUserProfile(id);
            if (checkUserProfile != null)
                return checkUserProfile;
            ViewBag.type = "movies";
            viewModel.AllMovies = await movieService.LoadMovies(0, 20);
            viewModel.wishListMovies = await profilemovies.WhishlistMovies((int)id);
            return View(viewModel);
        }
        public async Task<IActionResult> TvShows(int id)
        {
            var checkUserProfile = await CheckUserProfile(id);
            if (checkUserProfile != null)
                return checkUserProfile;
            ViewBag.type = "TvShow";
            viewModel.AllSeries = await seriesService.LoadSeries(0, 20);
            viewModel.wishListSeries = await profileSeries.WhishlistSeries((int)id);
            return View(viewModel);
        }

        #endregion


        #region Methods
        public async Task<IActionResult> LoadMoreMovies(int id, int skip, int take)
        {
            ViewBag.profileID = id;
            viewModel.AllMovies = await movieService.LoadMovies(skip, take);
            viewModel.wishListMovies = await profilemovies.WhishlistMovies((int)id);
            return PartialView("_MoviesloaderPartial", viewModel);
        }
        public async Task<IActionResult> LoadMoreSeries(int id, int skip, int take)
        {
            ViewBag.profileID = id;
            viewModel.AllSeries = await seriesService.LoadSeries(skip, take);
            viewModel.wishListSeries = await profileSeries.WhishlistSeries((int)id);
            return PartialView("_TvShowPartial", viewModel);
        }
        public async Task<IActionResult> LoadMoreSearch(int id, int skip, int take, string t, string sName)
        {
            ViewBag.type = t;
            List<Movie> searchedMovies = new List<Movie>();
            List<Series> searchedSeries = new List<Series>();

            switch (t.ToLower())
            {
                case "movies":
                    searchedMovies = await movieService.GetBySearch(sName, skip, take);
                    break;
                case "tvshow":
                    searchedSeries = await seriesService.GetBySearch(sName, skip, take);
                    break;
                case "all":
                    searchedSeries = await seriesService.GetBySearch(sName, (skip / 2), (take / 2));
                    searchedMovies = await movieService.GetBySearch(sName, (skip / 2), (take / 2));
                    break;
            }

            ViewBag.profileID = id;
            viewModel.AllMovies = searchedMovies;
            viewModel.AllSeries = searchedSeries;
            viewModel.wishListMovies = await profilemovies.WhishlistMovies((int)id);
            viewModel.wishListSeries = await profileSeries.WhishlistSeries((int)id);
            return PartialView("_searchedPartial", viewModel);
        }
        public async Task<IActionResult> LoadNewAndPop(int id, int skip, int take, string t, string category)
        {
            ViewBag.type = t;
            List<Movie> movies = new List<Movie>();
            List<Series> series = new List<Series>();

            switch (category.ToLower())
            {
                case "new":
                    movies = await movieService.GetNewest(skip, take);
                    series = await seriesService.GetNewest(skip, take);
                    break;
                case "trend":
                    movies = await movieService.GetTrend(skip, take);
                    series = await seriesService.GetTrend(skip, take);
                    break;
                case "all":
                    var trendM = await movieService.GetTrend(skip / 2, take / 2);
                    var newM = await movieService.GetNewest(skip / 2, take / 2);
                    var trenSeries = await seriesService.GetTrend(skip / 2, take / 2);
                    var newSeries = await seriesService.GetNewest(skip / 2, take / 2);
                    movies = trendM.Union(newM).Distinct().ToList();
                    series = trenSeries.Union(newSeries).Distinct().ToList();
                    break;
            }


            switch (t.ToLower())
            {
                case "tvshow":
                    viewModel.AllSeries = series;
                    break;
                case "movies":
                    viewModel.AllMovies = movies;
                    break;
                case "both":
                    viewModel.AllSeries = series;
                    viewModel.AllMovies = movies;
                    break;
            }

            ViewBag.profileID = id;
            viewModel.wishListMovies = await profilemovies.WhishlistMovies((int)id);
            viewModel.wishListSeries = await profileSeries.WhishlistSeries((int)id);
            return PartialView("_searchedPartial", viewModel);
        }
        public async Task<IActionResult> CheckUserProfile(int? id)
        {
            if (id == null)
                return BadRequest("You can't Access this page directly!");

            int userId = loggedDataService.LoggedUserId();
            ProfileUser? ProfUser = profileUserService.GetProfileByUserId(userId).Result.Where(a => a.ProfileId == id).FirstOrDefault();

            ViewBag.profileID = id;

            if (ProfUser == null)
                return NotFound("Access denied!");
            return null;
        }
        public async Task<IActionResult> searched(string searchName, string t)
        {
            var searchedMovies = new List<Movie>();
            var searchedSeries = new List<Series>();
            try
            {
                switch (t.ToLower())
                {
                    case "m":
                        searchedMovies = await movieService.GetBySearch(searchName, 0, 10);
                        break;
                    case "s":
                        searchedSeries = await seriesService.GetBySearch(searchName, 0, 10);
                        break;
                    default:
                        searchedMovies = await movieService.GetBySearch(searchName, 0, 10);
                        searchedSeries = await seriesService.GetBySearch(searchName, 0, 10);
                        break;

                }

                var searchData = new
                {
                    movies = searchedMovies.Select(s => new { MovieID = s.MovieID, Poster = s.Poster, MovieName = s.MovieName }),
                    series = searchedSeries.Select(s => new { SeriesID = s.SeriesID, Poster = s.Poster, SeriesName = s.SeriesName })
                };

                return new JsonResult(searchData, new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred during the search.");
            }
        }
        public async Task<IActionResult> AddToList(int id, int movieID)
        {
            var movie = await movieService.GetById(movieID);
            var profile = await profileService.GetById(id);
            if (profile == null || movie == null) return NotFound();
            if (profile.Movies == null || !profile.Movies.Any(m => m.MovieID == movieID))
                await profilemovies.AddMovieToWishList(id, movie);
            else
                await profilemovies.DeleteMovieFromList(id, movie);

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> AddSeriesToList(int id, int sid)
        {
            var series = await seriesService.GetById(sid);
            var profile = await profileService.GetById(id);
            if (series == null || profile == null) return NotFound();

            if (profile.Series == null || !profile.Series.Any(s => s.SeriesID == sid))
                await profileSeries.AddSeriesToWishList(id, series);
            else
                await profileSeries.DeleteSeriesFromList(id, series);

            return Ok();
        }
        public async Task SetMovieRate(int id, int showid, int movieRate)
        {
            var profile = await profileService.GetById(id);
            if (profile != null)
            {
                if (profile.RatedMovies == null || !profile.RatedMovies.Any(m => m.MovieID == showid))
                {
                    await profilemovies.AddRatedMovie(profile, showid, movieRate);
                    await movieService.SetRate(showid, movieRate);
                }
            }
        }
        public async Task SetSeriesRate(int id, int showid, int movieRate)
        {
            var profile = await profileService.GetById(id);
            if (profile != null)

            {
                if (profile.RatedSeries == null || !profile.RatedSeries.Any(s => s.SeriesID == showid))
                {
                    await profileSeries.AddRatedSeries(profile, showid, movieRate);
                    await seriesService.SetRate(showid, movieRate);
                }
            }
        }
        public async Task movieProgress(int id, int mid, int prog)
        {
            await profilemovies.UpdateProgress(id, mid, prog);
        }
        public async Task EpisodeProgress(int id, int sid, int eid, int prog)
        {
            await profileSeries.UpdateProgress(id, sid, eid, prog);
        }
        #endregion
    }
}

