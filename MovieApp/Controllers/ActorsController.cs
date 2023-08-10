using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Controllers
{
    //allow admins only to access this.
    [Authorize(Roles = "Admin")]
    public class ActorsController : Controller
    {
        private readonly IActorService actorService;
        private readonly ILoggedDataService loggedDataService;

        public ActorsController(IActorService _actorService, ILoggedDataService _loggedDataService)
        {
            actorService = _actorService;
            loggedDataService = _loggedDataService;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var actionName = RouteData.Values["action"].ToString();
                if (HttpContext.Request.Method != HttpMethod.Post.Method /*&& actionName != "Edit"*/)
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

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await actorService.GetAll());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await actorService.GetById((int)id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActorID,ActorName")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                await actorService.Add(actor);
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await actorService.GetById((int)id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActorID,ActorName")] Actor actor)
        {
            if (id != actor.ActorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await actorService.Update(actor);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await ActorExists(actor.ActorID)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await actorService.GetById((int)id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await actorService.GetAll() == null)
            {
                return Problem("Entity set 'MovieAppContext.Genries'  is null.");
            }
            var actor = await actorService.GetById(id);
            if (actor != null)
            {
                await actorService.DeleteById(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ActorExists(int id)
        {
            var x = await actorService.GetById(id);
            return x != null ? true : false;
        }
    }
}
