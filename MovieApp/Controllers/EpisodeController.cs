using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MovieApp.IServices;
using MovieApp.Models;
using MovieApp.Services;
using NToastNotify;
using System.Data;

namespace MovieApp.Controllers
{
    //allow admins only to access this.
    [Authorize(Roles = "Admin")]
    public class EpisodeController : Controller
    {
        #region Fileds & Ctor
        private readonly ISeasonService seasonService;
        private readonly ISeriesGenriesService seriesGenriesService;
        private readonly IToastNotification toast;
        private readonly IEpisodesService episodesService;
        private readonly ILoggedDataService loggedDataService;

        public EpisodeController(ISeasonService _seasonService, ISeriesGenriesService _seriesGenriesService, 
            IToastNotification _toast, IEpisodesService _episodesService, ILoggedDataService _loggedDataService)
        {
            seasonService = _seasonService;
            seriesGenriesService = _seriesGenriesService;
            toast = _toast;
            episodesService = _episodesService;
            loggedDataService = _loggedDataService;
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
        public async Task<IActionResult> Index(int id)
        {
            var CurrSeason = await seasonService.GetByID(id);

            return View(CurrSeason);
        }
        public async Task<IActionResult> Add(int id)
        {
            ViewBag.ID = id;
            return View("EpisodesForm");
        }

        [HttpPost]
        public async Task<IActionResult> Add(int id, Episode eposide)
        {

            var currSeason = await seasonService.GetByID(id);
            ModelState.Remove("Season");
            await CheckEpNum(currSeason, eposide.EpNum);

            // var Video = await MovieVideo(eposide.EpisodeID, link);


            if (!ModelState.IsValid)
            {
                return View("EpisodesForm", eposide);
            }

            if (currSeason.Episodes.Count < currSeason.EposidesCount)
            {
                eposide.SeasonID = id;
                var AddedEposides = await episodesService.Add(eposide);
                // eposide.link = await SaveVideo(eposide.EpisodeID, link);
                // await episodesService.SaveChanges();
                toast.AddSuccessToastMessage("Episode Added Successfully");
                return RedirectToAction("Index", new { id = eposide.SeasonID });

            }
            else
            {
                toast.AddErrorToastMessage("Cant Add This Episode You Already Reatch The Episodes Count Limit ");
                return View("EpisodesForm", eposide);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id != null)
            {
                var eposide = await episodesService.GetByID(id);
                if (eposide != null)
                {
                    return View("EpisodesForm", eposide);
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
        public async Task<IActionResult> Edit(Episode eposide)
        {
            var season = await seasonService.GetByID(eposide.SeasonID);
            ModelState.Remove("Season");
            if (season.Episodes.Where(e => e.EpisodeID != eposide.EpisodeID).Any(e => e.EpNum == eposide.EpNum))
                ModelState.AddModelError("EpNum", "This Episode Number Already Exist");

            var oldEpisode = await episodesService.GetByID(eposide.EpisodeID);

            // var Video = await MovieVideo(eposide.EpisodeID, link);
            // if (Video == null)
            // {
            //     eposide.link = oldEpisode.link;
            // }
            // else
            // {
            //     eposide.link = await SaveVideo(eposide.EpisodeID, link);
            // }
            episodesService.EntryDetached(oldEpisode);

            if (!ModelState.IsValid)
            {
                return View("EpisodesForm", eposide);
            }
            await episodesService.Update(eposide);
            toast.AddSuccessToastMessage("Season Updated Successfully");
            return RedirectToAction("Index", new { id = eposide.SeasonID });
        }

        public async Task<IActionResult> Delete(int id)
        {
            await episodesService.Delete(id);
            return Ok();
        }
        #endregion

        #region Methods
        public async Task CheckEpNum(Season currSeason, int epnum)
        {
            var exist = currSeason.Episodes.Any(e => e.EpNum == epnum);
            if (exist)
            {
                ModelState.AddModelError("EpNum", "This Episode Number Already Exist");
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
                ModelState.AddModelError("link", "Only Mp4 Videos Allowed!");
                return null;
            }
            return "OK";
        }
        public async Task<string> SaveVideo(int id, IFormFile video)
        {
            string episodeVid = id.ToString() + "." + video.FileName.Split(".").Last();
            var imageFile = Path.Combine("wwwroot/Videos/episodes", episodeVid);

            using (var memoryStream = new FileStream(imageFile, FileMode.Create))
            {
                await video.CopyToAsync(memoryStream);
                return episodeVid;
            }
        }
        #endregion
    }
}
