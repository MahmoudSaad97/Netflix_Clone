using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class RolesController : Controller
    {
        private readonly RoleManager<IdRoles> roleManager;
        private readonly ILoggedDataService loggedDataService;

        public RolesController(RoleManager<IdRoles> _roleManager, ILoggedDataService _loggedDataService)
        {
            roleManager = _roleManager;
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

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            return View(await roleManager.Roles.ToListAsync());
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || roleManager.Roles == null)
            {
                return NotFound();
            }

            var idRoles = await roleManager.FindByIdAsync(id.ToString());

            if (idRoles == null)
            {
                return NotFound();
            }

            return View(idRoles);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NormalizedName,ConcurrencyStamp")] IdRoles idRoles)
        {
            if (ModelState.IsValid)
            {
                await roleManager.CreateAsync(idRoles);

                return RedirectToAction(nameof(Index));
            }
            return View(idRoles);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || roleManager.Roles == null)
            {
                return NotFound();
            }

            var idRoles = await roleManager.FindByIdAsync(id.ToString());
            if (idRoles == null)
            {
                return NotFound();
            }
            return View(idRoles);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NormalizedName,ConcurrencyStamp")] IdRoles idRoles)
        {
            if (id != idRoles.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await roleManager.UpdateAsync(idRoles);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IdRolesExists(idRoles.Id))
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
            return View(idRoles);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || roleManager.Roles == null)
            {
                return NotFound();
            }

            var idRoles = await roleManager.FindByIdAsync(id.ToString());
            if (idRoles == null)
            {
                return NotFound();
            }

            return View(idRoles);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (roleManager.Roles == null)
            {
                return Problem("Entity set 'MovieAppContext.Roles'  is null.");
            }
            var idRoles = await roleManager.FindByIdAsync(id.ToString());
            if (idRoles != null)
            {
                await roleManager.DeleteAsync(idRoles);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool IdRolesExists(int id)
        {
          return (roleManager.Roles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
