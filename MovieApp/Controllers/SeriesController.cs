using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using MovieApp.Services;
using NToastNotify;
using System.Collections.Generic;

namespace MovieApp.Controllers
{
    //allow admins only to access this.
    [Authorize(Roles = "Admin")]
    public class SeriesController : Controller
    {
        #region Fields & Ctor
        private readonly IToastNotification toast;
        private readonly ISeriesActorsService seriesActorsService;
        private readonly ISeriesService seriesService;
        private readonly ISeriesGenriesService seriesGenriesService;
        private readonly ILoggedDataService loggedDataService;
        private readonly IActorService actorService;
        private readonly IGenrieService genrieService;

        public SeriesController(ISeriesService _seriesService, IToastNotification _toast, ISeriesGenriesService _seriesGenriesService,
            ISeriesActorsService _seriesActorsService, ILoggedDataService _loggedDataService, IActorService _actorService, IGenrieService _genrieService)
        {
            seriesService = _seriesService;
            seriesGenriesService = _seriesGenriesService;
            toast = _toast;
            seriesActorsService = _seriesActorsService;
            loggedDataService = _loggedDataService;
            actorService = _actorService;
            genrieService = _genrieService;
        }
        #endregion

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

        #region Actions
        public async Task<IActionResult> Index()
        {
            var allSeries = await seriesService.GetAll();
            return View(allSeries);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id != null)
            {
                var serie = await seriesService.GetById(id);
                if (serie != null)
                {
                    var GenriesToRemove = await seriesGenriesService.GetGenrieIn(serie.SeriesID);
                    var AddGenrie = await seriesGenriesService.GetGenrieOut(serie.SeriesID);
                    var ActorsToRemove = await seriesActorsService.GetActorIn(serie.SeriesID);
                    var AddActor = await seriesActorsService.GetActorOut(serie.SeriesID);

                    ViewBag.RGenrie = new SelectList(GenriesToRemove, "GenrieID", "GenrieName");
                    ViewBag.Genries = new SelectList(AddGenrie, "GenrieID", "GenrieName");

                    ViewBag.RActor = new SelectList(ActorsToRemove, "ActorID", "ActorName");
                    ViewBag.Actors = new SelectList(AddActor, "ActorID", "ActorName");
                    return View("SeriesForm", serie);
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
        public async Task<IActionResult> Edit(Series s, int[] RemoveGenries, int[] AddGenries, int[] RemoveActors, int[] AddActors)
        {

            var oldSeries = await seriesService.GetById(s.SeriesID);
            if (RemoveGenries.Length > 0)
            {
                await seriesGenriesService.RemoveGenrie(oldSeries.SeriesID, RemoveGenries);
            }
            if (AddGenries.Length > 0)
            {
                await seriesGenriesService.UpdateGenrie(oldSeries.SeriesID, AddGenries);
            }
            if (RemoveActors.Length > 0)
            {
                await seriesActorsService.RemoveActor(oldSeries.SeriesID, RemoveActors);
            }
            if (AddActors.Length > 0)
            {
                await seriesActorsService.UpdateActor(oldSeries.SeriesID, AddActors);
            }
            seriesService.SeriesDetach(oldSeries);

            var Poster = Request.Form.Files.FirstOrDefault();
            var poster = await SeriesPoster(s.SeriesID, Poster);
            if (poster == null)
            {
                s.Poster = oldSeries.Poster;
            }
            else
            {
                s.Poster = await SavePoster(s.SeriesID, Poster);
            }
            s.Seasons = oldSeries.Seasons;
            if (ModelState.IsValid)
            {
                await seriesService.Update(s);
                toast.AddSuccessToastMessage("Series Updated Successfully");

                return RedirectToAction("index");
            }
            var Actors = await actorService.GetAll();
            ViewBag.Genries = new SelectList(await seriesGenriesService.GetGenrieOut(s.SeriesID), "GenrieID", "GenrieName");
            ViewBag.RGenries = new SelectList(await seriesGenriesService.GetGenrieIn(s.SeriesID), "GenrieID", "GenrieName");
            ViewBag.Actors = new SelectList(Actors, "ActorID", "ActorName");
            return View("SeriesForm", s);
        }

        public async Task<IActionResult> Add()
        {
            var Genries = await genrieService.GetAll();
            var Actors = await actorService.GetAll();
            ViewBag.Genries = new SelectList(Genries, "GenrieID", "GenrieName");
            ViewBag.Actors = new SelectList(Actors, "ActorID", "ActorName");
            return View("SeriesForm");
        }

        [HttpPost]
        public async Task<IActionResult> Add(Series s, IFormFile Poster, int[] AddGenries, int[] AddActors)
        {
            if (AddActors.Length <= 0)
            {
                ModelState.AddModelError("Actors", "Please Select Movie Actors");
                ViewBag.Genries = new SelectList(await genrieService.GetAll(), "GenrieID", "GenrieName");
                ViewBag.Actors = new SelectList(await actorService.GetAll(), "ActorID", "ActorName");
                return View("SeriesForm", s);
            }

            if (AddGenries.Length <= 0)
            {
                ModelState.AddModelError("Genries", "Please Select Movie Genrie");
                ViewBag.Genries = new SelectList(await genrieService.GetAll(), "GenrieID", "GenrieName");
                ViewBag.Actors = new SelectList(await actorService.GetAll(), "ActorID", "ActorName");
                return View("SeriesForm", s);
            }

            var Sposter = await SeriesPoster(s.SeriesID, Poster);

            if (Sposter == null)
            {
                ViewBag.Genries = new SelectList(await genrieService.GetAll(), "GenrieID", "GenrieName");
                ViewBag.Actors = new SelectList(await actorService.GetAll(), "ActorID", "ActorName");
                return View("SeriesForm", s);
            }
            if (ModelState.IsValid)
            {
                s.Views = 0;
                s.Rate = 0;
                s.Votes = 0;
                await seriesService.Add(s);
                s.Poster = await SavePoster(s.SeriesID, Poster);
                await seriesGenriesService.UpdateGenrie(s.SeriesID, AddGenries);
                await seriesActorsService.UpdateActor(s.SeriesID, AddActors);
                toast.AddSuccessToastMessage("Series Added Successfully");
                return RedirectToAction("index");
            }
            ViewBag.Genries = new SelectList(await genrieService.GetAll(), "GenrieID", "GenrieName");
            ViewBag.Actors = new SelectList(await actorService.GetAll(), "ActorID", "ActorName");
            return View("SeriesForm", s);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == null) return BadRequest();

            var serie = await seriesService.GetById(id);

            if (serie == null) return NotFound();

            return View(serie);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await seriesService.Delete(id);
            return Ok();
        }


        #endregion

        #region Poster Methods
        public async Task<string> SeriesPoster(int? id, IFormFile poster)
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
            string seriesimg = id.ToString() + "." + Poster.FileName.Split(".").Last();
            var imageFile = Path.Combine("wwwroot/images/series", seriesimg);

            using (var memoryStream = new FileStream(imageFile, FileMode.Create))
            {
                await Poster.CopyToAsync(memoryStream);
                return seriesimg;
            }
        }

        #endregion
    }
}
