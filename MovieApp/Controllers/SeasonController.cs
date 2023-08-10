using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MovieApp.IServices;
using MovieApp.Models;
using MovieApp.Services;
using NToastNotify;
using System.Collections.Generic;

namespace MovieApp.Controllers
{
    //allow admins only to access this.
    [Authorize(Roles = "Admin")]
    public class SeasonController : Controller
    {
        private readonly ISeasonService seasonService;
        private readonly ISeriesService seriesService;
        private readonly ISeriesGenriesService seriesGenriesService;
        private readonly IToastNotification toast;
        private readonly ILoggedDataService loggedDataService;

        public SeasonController(ISeasonService _seasonService, ISeriesService _seriesService, 
            ISeriesGenriesService _seriesGenriesService, IToastNotification _toast,
            ILoggedDataService _loggedDataService)
        {
            seasonService = _seasonService;
            seriesService = _seriesService;
            seriesGenriesService = _seriesGenriesService;
            toast = _toast;
            loggedDataService = _loggedDataService;
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

        #region Actions
        public async Task<IActionResult> Index(int id)
        {
            var CurrSeries = await seriesService.GetById(id);

            return View(CurrSeries);
        }
        public async Task<IActionResult> Add(int id)
        {
            ViewBag.ID = id;
            return View("SeasonForm");
        }

        [HttpPost]
        public async Task<IActionResult> Add(int id, Season season)
        {

            var currSeries = await seriesService.GetById(id);
            ModelState.Remove("Series");

            if (!ModelState.IsValid)
            {
                return View("SeasonForm", season);
            }
            season.SeriesID = id;
            season.SeasonName = $"{currSeries.SeriesName}: {season.SeasonName}";
            var AddedSeason = await seasonService.Add(season);
            toast.AddSuccessToastMessage("Season Added Successfully");
            return RedirectToAction("Index", new { id = season.SeriesID });
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id != null)
            {
                var season = await seasonService.GetByID(id);
                if (season != null)
                {
                    return View("SeasonForm", season);
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
        public async Task<IActionResult> Edit(Season s)
        {
            ModelState.Remove("Series");
            if (!ModelState.IsValid)
            {
                return View("SeasonForm", s);
            }
            await seasonService.Update(s);
            toast.AddSuccessToastMessage("Season Updated Successfully");
            return RedirectToAction("Index", new { id = s.SeriesID });
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id == null) return BadRequest();

            var season = await seasonService.GetByID(id);

            if (season == null) return NotFound();

            return View(season);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await seasonService.Delete(id);
            return Ok();
        }
        #endregion
    }
}
