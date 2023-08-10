using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using MovieApp.Services;
using NToastNotify;
using System.Data;

namespace MovieApp.Controllers
{
    //allow admins only to access this.
    [Authorize(Roles = "Admin")]
    public class MovieController : Controller
    {
        #region Fields & Ctor
        private readonly IMovieService movieService;
        private readonly IMovieGenriesService movieGenriesService;
        private readonly IToastNotification toast;
        private readonly IMovieActorsService movieActorsService;
        private readonly ILoggedDataService loggedDataService;
        private readonly IUserService userService;
        private readonly IActorService actorService;
        private readonly IGenrieService genrieService;

        public MovieController(IMovieService _movieService, IMovieGenriesService _movieGenriesService,
            IToastNotification _toast, IMovieActorsService _movieActorsService, ILoggedDataService _loggedDataService,
            IUserService _userService, IActorService _actorService, IGenrieService _genrieService)
        {
            movieService = _movieService;
            movieGenriesService = _movieGenriesService;
            toast = _toast;
            movieActorsService = _movieActorsService;
            loggedDataService = _loggedDataService;
            userService = _userService;
            actorService = _actorService;
            genrieService = _genrieService;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var actionName = RouteData.Values["action"].ToString();
                if (HttpContext.Request.Method != HttpMethod.Post.Method/* && actionName != "Edit"*/)
                {
                    var fullname = (await loggedDataService.GetUserFullName());
                    if (fullname != null)
                        ViewBag.loggedUserFullName = fullname;
                }
            }
            finally
            {
                await base.OnActionExecutionAsync(context, next);
            }
        }
        #endregion

        #region Actions
        public async Task<IActionResult> Index()
        {
            int userId = loggedDataService.LoggedUserId();
            if (userId > 0)
            {
                bool admin = await loggedDataService.checkUserAccessByRoleName(userId, "Admin");
                if (admin)
                {
                    List<Movie> allmovies = await movieService.GetAll();

                    return View(allmovies);
                }
                return NotFound("Access denied!");
            }
            return NotFound("Access denied!");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id != null)
            {
                var movie = await movieService.GetById(id);
                if (movie != null)
                {
                    var GenriesToRemove = await movieGenriesService.GetGenrieIn(movie.MovieID);
                    var AddGenrie = await movieGenriesService.GetGenrieOut(movie.MovieID);
                    var ActorsToRemove = await movieActorsService.GetActorIn(movie.MovieID);
                    var AddActor = await movieActorsService.GetActorOut(movie.MovieID);

                    ViewBag.RGenrie = new SelectList(GenriesToRemove, "GenrieID", "GenrieName");
                    ViewBag.Genries = new SelectList(AddGenrie, "GenrieID", "GenrieName");

                    ViewBag.RActor = new SelectList(ActorsToRemove, "ActorID", "ActorName");
                    ViewBag.Actors = new SelectList(AddActor, "ActorID", "ActorName");
                    return View("MovieForm", movie);
                }
                else
                    return NotFound();

            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, int[] RemoveGenries, int[] AddGenries, int[] RemoveActors, int[] AddActors,IFormFile? Poster)
        {
            var oldMovie = await movieService.GetById(movie.MovieID);
            if (RemoveGenries.Length > 0)
            {
                await movieGenriesService.RemoveGenrie(oldMovie.MovieID, RemoveGenries);
            }
            if (AddGenries.Length > 0)
            {
                await movieGenriesService.UpdateGenrie(oldMovie.MovieID, AddGenries);
            }
            if (RemoveActors.Length > 0)
            {
                await movieActorsService.RemoveActor(oldMovie.MovieID, RemoveActors);
            }
            if (AddActors.Length > 0)
            {
                await movieActorsService.UpdateActor(oldMovie.MovieID, AddActors);
            }

            movieService.EntityDetach(oldMovie);

            CheckDate(movie);


            var Mposter = await MoviePoster(movie.MovieID, Poster);
            //var Video = await MovieVideo(movie.MovieID, Link);
            if (Mposter == null)
            {
                movie.Poster = oldMovie.Poster;
            }
            else
            {
                movie.Poster = await SavePoster(movie.MovieID, Poster);
            }
            
            //if (Video == null)
            //{
            //    movie.Link = oldMovie.Link;
            //}
            //else
            //{
            //    movie.Link = await SaveVideo(movie.MovieID, Link);
            //}
            


            if (ModelState.IsValid)
            {
                movie.votes = oldMovie.votes;
                movie.Rate = oldMovie.Rate;
                movie.Views = oldMovie.Views;
                await movieService.Update(movie);
                toast.AddSuccessToastMessage("Movie Updated Successfully");

                return RedirectToAction("index");
            }
            var Actors = await movieActorsService.GetActorOut(movie.MovieID);
            ViewBag.Genries = new SelectList(await movieGenriesService.GetGenrieOut(movie.MovieID), "GenrieID", "GenrieName");
            ViewBag.RGenrie = new SelectList(await movieGenriesService.GetGenrieIn(movie.MovieID), "GenrieID", "GenrieName");
            ViewBag.Actors = new SelectList(Actors, "ActorID", "ActorName");
            ViewBag.RActor = new SelectList(await movieActorsService.GetActorIn(movie.MovieID), "ActorID", "ActorName");
            return View("MovieForm", movie);
        }



        //Add New Movie View
        public async Task<IActionResult> Add()
        {
            var Genries = await genrieService.GetAll();
            var Actors = await actorService.GetAll();
            ViewBag.Genries = new SelectList(Genries, "GenrieID", "GenrieName");
            ViewBag.Actors = new SelectList(Actors, "ActorID", "ActorName");
            return View("MovieForm");
        }

        //Add New Movie Action
        [HttpPost]
        public async Task<IActionResult> Add(Movie movie, IFormFile Poster, int[] AddGenries, int[] AddActors)
        {
            if (AddActors.Length <= 0)
            {
                ModelState.AddModelError("Actors", "Please Select Movie Actors");
                ViewBag.Genries = new SelectList(await genrieService.GetAll(), "GenrieID", "GenrieName");
                ViewBag.Actors = new SelectList(await actorService.GetAll(), "ActorID", "ActorName");
                return View("MovieForm", movie);
            }

            if (AddGenries.Length <= 0)
            {
                ModelState.AddModelError("Genries", "Please Select Movie Genrie");
                ViewBag.Genries = new SelectList(await genrieService.GetAll(), "GenrieID", "GenrieName");
                ViewBag.Actors = new SelectList(await actorService.GetAll(), "ActorID", "ActorName");
                return View("MovieForm", movie);
            }

            var Mposter = await MoviePoster(movie.MovieID, Poster);
            //var MVideo = await MovieVideo(movie.MovieID, Link);

            if (Mposter == null)
            {
                ViewBag.Genries = new SelectList(await genrieService.GetAll(), "GenrieID", "GenrieName");
                ViewBag.Actors = new SelectList(await actorService.GetAll(), "ActorID", "ActorName");
                return View("MovieForm", movie);
            }
            movie.ReleaseDate = movie.ReleaseDate ?? DateTime.Now.Date;
            CheckDate(movie);
            
            if (ModelState.IsValid)
            {
                movie.votes = 0;
                movie.Views = 0;
                movie.Rate = 0;
                await movieService.Add(movie);
                movie.Poster = await SavePoster(movie.MovieID, Poster);
                //movie.Link = await SaveVideo(movie.MovieID, Link);
                await movieGenriesService.UpdateGenrie(movie.MovieID, AddGenries);
                await movieActorsService.UpdateActor(movie.MovieID, AddActors);
                toast.AddSuccessToastMessage("Movie Added Successfully");
                return RedirectToAction("index");
            }
            ViewBag.Genries = new SelectList(await genrieService.GetAll(), "GenrieID", "GenrieName");
            ViewBag.Actors = new SelectList(await actorService.GetAll(), "ActorID", "ActorName");
            return View("MovieForm", movie);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await movieService.Delete(id);
            return Ok();

        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == null) return BadRequest();

            var movie = await movieService.GetById(id);

            if (movie == null) return NotFound();

            return View(movie);
        }
        #endregion

        #region Methods
        public async Task<string> MoviePoster(int? id, IFormFile poster)
        {
            if (id > 0 && poster == null)
            {
                return null;
            }
            var allowedExtensions = new List<string> { ".jpg", ".jpeg" };
            var fileExtension = Path.GetExtension(poster.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("Poster", "Only Jpg and Jpeg Images Allowed!");
                return null;
            }

            if (poster.Length > 1048576)
            {
                ModelState.AddModelError("Poster", "Image cannot be more than 1 MB");
                return null;
            }
            return "OK";
        }
        public async Task<string> SavePoster(int id, IFormFile Poster)
        {
            string movieimg = id.ToString() + "." + Poster.FileName.Split(".").Last();
            var imageFile = Path.Combine("wwwroot/images/movies", movieimg);

            using (var memoryStream = new FileStream(imageFile, FileMode.Create))
            {
                await Poster.CopyToAsync(memoryStream);
                return movieimg;
            }
        }
        
        public async Task<string> MovieVideo(int? id, IFormFile video)
        {
            if (id > 0 && video == null)
            {
                return null;
            }
            var allowedExtensions = new List<string> { ".mp4" };
            var fileExtension = Path.GetExtension(video.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("Link", "Only Mp4 Videos Allowed!");
                return null;
            }
            return "OK";
        }
        public async Task<string> SaveVideo(int id, IFormFile video)
        {
            string movieVid = id.ToString() + "." + video.FileName.Split(".").Last();
            var imageFile = Path.Combine("wwwroot/Videos/movies", movieVid);

            using (var memoryStream = new FileStream(imageFile, FileMode.Create))
            {
                await video.CopyToAsync(memoryStream);
                return movieVid;
            }
        }

        public void CheckDate(Movie movie)
        {
            if (movie.ReleaseDate.HasValue)
            {
                DateTime minDate = new DateTime(1900, 1, 1);
                DateTime maxDate = DateTime.Now.Date;

                if (movie.ReleaseDate.Value < minDate || movie.ReleaseDate.Value > maxDate)
                {
                    ModelState.AddModelError("ReleaseDate", "Invalid date.");
                }
            }
        }
        #endregion
    }
}