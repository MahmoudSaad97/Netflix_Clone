using System;
using System.Collections.Generic;
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
using SendGrid.Helpers.Mail;

namespace MovieApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubscribesController : Controller
    {
        private readonly ISubscribesService subscribesService;
        private readonly ILoggedDataService loggedDataService;
        private readonly IUserService userService;
        private readonly RoleManager<IdRoles> roleManager;

        public SubscribesController(ISubscribesService _subscribesService, ILoggedDataService _loggedDataService,
            IUserService _userService, RoleManager<IdRoles> _roleManager)
        {
            subscribesService = _subscribesService;
            loggedDataService = _loggedDataService;
            userService = _userService;
            roleManager = _roleManager;
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

        // GET: Subscribes
        public async Task<IActionResult> Index()
        {
            return View(await subscribesService.GetAll());
        }

        // GET: Subscribes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || await subscribesService.GetAll() == null)
            {
                return NotFound();
            }

            var subscribe = await subscribesService.GetById((int)id);
            if (subscribe == null)
            {
                return NotFound();
            }

            return View(subscribe);
        }

        // GET: Subscribes/Create
        public async Task<IActionResult> Create()
        {
            var subscribes = await subscribesService.GetAll();
            ViewData["RoleId"] = new SelectList(roleManager.Roles, "Id", "Name");
            return View();
        }

        // POST: Subscribes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubscribeID,RoleId,DurationDays,Price")] Subscribe subscribe)
        {
            if (ModelState.IsValid)
            {
                await subscribesService.Add(subscribe);
                return RedirectToAction(nameof(Index));
            }
            var subscribes = await subscribesService.GetAll();
            ViewData["RoleId"] = new SelectList(roleManager.Roles, "Id", "Name", subscribe.RoleId);
            return View(subscribe);
        }

        // GET: Subscribes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || await subscribesService.GetAll() == null)
            {
                return NotFound();
            }

            var subscribe = await subscribesService.GetById((int)id);
            if (subscribe == null)
            {
                return NotFound();
            }
            var subscribes = await subscribesService.GetAll();
            ViewData["RoleId"] = new SelectList(roleManager.Roles, "Id", "Name", subscribe.RoleId);
            return View(subscribe);
        }

        // POST: Subscribes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubscribeID,RoleId,DurationDays,Price")] Subscribe subscribe)
        {
            if (id != subscribe.SubscribeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await subscribesService.Update(subscribe);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await SubscribeExists(subscribe.SubscribeID)))
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
            var subscribes = await subscribesService.GetAll();
            ViewData["RoleId"] = new SelectList(roleManager.Roles, "Id", "Name", subscribe.RoleId);
            return View(subscribe);
        }

        // GET: Subscribes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || await subscribesService.GetAll() == null)
            {
                return NotFound();
            }

            var subscribe = await subscribesService.GetById((int)id);
            if (subscribe == null)
            {
                return NotFound();
            }

            return View(subscribe);
        }

        // POST: Subscribes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await subscribesService.GetAll() == null)
            {
                return Problem("Entity set 'MovieAppContext.Subscribes'  is null.");
            }
            var subscribe = await subscribesService.GetById(id);
            if (subscribe != null)
            {
                await subscribesService.DeleteById(id);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SubscribeExists(int id)
        {
            var x = await subscribesService.GetById(id);
            return x != null ? true : false;
        }
    }
}
