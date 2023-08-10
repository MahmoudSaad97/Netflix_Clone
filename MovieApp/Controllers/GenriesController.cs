using System;
using System.Collections.Generic;
using System.Data;
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
    public class GenriesController : Controller
    {
        private readonly IGenrieService genrieService;
        private readonly ILoggedDataService loggedDataService;

        public GenriesController(IGenrieService _genrieService, ILoggedDataService _loggedDataService)
        {
            genrieService = _genrieService;
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

        // GET: Genries
        public async Task<IActionResult> Index()
        {
              return View(await genrieService.GetAll());
        }

        // GET: Genries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genrie = await genrieService.GetById((int)id);
            if (genrie == null)
            {
                return NotFound();
            }

            return View(genrie);
        }

        // GET: Genries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GenrieID,GenrieName")] Genrie genrie)
        {
            if (ModelState.IsValid)
            {
                await genrieService.Add(genrie);
                return RedirectToAction(nameof(Index));
            }
            return View(genrie);
        }

        // GET: Genries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genrie = await genrieService.GetById((int)id);
            if (genrie == null)
            {
                return NotFound();
            }
            return View(genrie);
        }

        // POST: Genries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GenrieID,GenrieName")] Genrie genrie)
        {
            if (id != genrie.GenrieID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await genrieService.Update(genrie);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await GenrieExists(genrie.GenrieID)))
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
            return View(genrie);
        }

        // GET: Genries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genrie = await genrieService.GetById((int)id);
            if (genrie == null)
            {
                return NotFound();
            }

            return View(genrie);
        }

        // POST: Genries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await genrieService.GetAll() == null)
            {
                return Problem("Entity set 'MovieAppContext.Genries'  is null.");
            }
            var genrie = await genrieService.GetById(id);
            if (genrie != null)
            {
                await genrieService.DeleteById(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> GenrieExists(int id)
        {
            var x = await genrieService.GetById(id);
            return x != null ? true : false;
        }
    }
}
