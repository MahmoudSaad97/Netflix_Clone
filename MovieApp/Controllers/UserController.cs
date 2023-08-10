using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using MovieApp.Services;
using System.Net.Http;

namespace MovieApp.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly ILoggedDataService loggedDataService;
        private readonly IUserStore<User> userStore;
        private readonly UserManager<User> userManager;
        private readonly IPasswordHasher<User> PasswordHasher;
        private readonly RoleManager<IdRoles> roleManager;

        public UserController(IUserService _userService, ILoggedDataService _loggedDataService, IUserStore<User> _userStore,
            UserManager<User> _userManager, IPasswordHasher<User> _PasswordHasher, RoleManager<IdRoles> _roleManager)
        {
            userService = _userService;
            loggedDataService = _loggedDataService;
            userStore = _userStore;
            userManager = _userManager;
            PasswordHasher = _PasswordHasher;
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

        // GET: User
        public async Task<IActionResult> Index()
        {
            List<User> users = await userService.GetAll();
            if (users.Count > 0)
            {
                List<string> currRoles = new List<string>();
                List<string> colors = new List<string>();
                foreach (var user in users)
                {
                    foreach (var item in roleManager.Roles.ToList())
                    {
                        if ((await userManager.IsInRoleAsync(user, item.Name)))
                        {
                            currRoles.Add(item.Name);
                            if (item.Name == "Admin")
                                colors.Add("text-danger");
                            else
                                colors.Add("");
                        }
                    }
                }

                ViewBag.currRoles = currRoles;
                ViewBag.colors = colors;

                return View(users);
            }
            return View();
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || await userService.GetAll() == null)
            {
                return NotFound();
            }

            var user = await userService.GetById((int)id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            ViewBag.RolesList = new SelectList(roleManager.Roles.ToList(), "Id", "Name");

            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int RoleId, [Bind("fname,lname,country,BirthDate,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] User user)
        {
            if (ModelState.IsValid)
            {
                await userStore.SetUserNameAsync(user, user.Email, CancellationToken.None);
                
                //Using Identity to create user.
                var result = await userManager.CreateAsync(user, user.PasswordHash);

                if (result.Succeeded)
                {
                    string currRoleName = "";
                    foreach (var item in roleManager.Roles.ToList())
                        if (item.Id == RoleId)
                            currRoleName = item.Name;
                    
                    IdentityResult roleresult = await userManager.AddToRoleAsync(user, currRoleName);
                    //await userService.Add(user);
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.RolesList = new SelectList(roleManager.Roles.ToList(), "Id", "Name");
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || await userService.GetAll() == null)
            {
                return NotFound();
            }

            var user = await userService.GetById((int)id);
            if (user == null)
            {
                return NotFound();
            }

            int currRoleId = -1;
            foreach (var item in roleManager.Roles.ToList())
                if (await userManager.IsInRoleAsync(user, item.Name))
                    currRoleId = item.Id;

            ViewBag.RoleId = currRoleId;
            ViewBag.RolesList = new SelectList(roleManager.Roles.ToList(), "Id", "Name", currRoleId);

            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int RoleId, [Bind("fname,lname,country,BirthDate,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.NormalizedEmail = user.Email.ToUpper();
                    user.UserName = user.Email;
                    user.NormalizedUserName = user.UserName.ToUpper();
                    user.PasswordHash = PasswordHasher.HashPassword(user, user.PasswordHash);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await UserExists(user.Id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                await userService.Update(user);

                string currRoleName = "";
                List<string> currRoles = new List<string>();
                foreach (var item in roleManager.Roles.ToList())
                {
                    if (item.Id == RoleId)
                        currRoleName = item.Name;

                    if ((await userManager.IsInRoleAsync(user, item.Name)))
                        currRoles.Add(item.Name);
                }

                bool checkUserRole = await userManager.IsInRoleAsync(user, currRoleName);

                if (!checkUserRole)
                {
                    var removeResult = await userManager.RemoveFromRolesAsync(user, currRoles);
                    if (removeResult.Succeeded)
                    {
                        IdentityResult roleresult = await userManager.AddToRoleAsync(user, currRoleName);

                        return RedirectToAction(nameof(Index));
                    }
                    else
                        return RedirectToAction(nameof(Index));
                }
                else
                    return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || await userService.GetAll() == null)
            {
                return NotFound();
            }

            var user = await userService.GetById((int)id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await userService.GetAll() == null)
            {
                return Problem("Entity set 'MovieAppContext.Users'  is null.");
            }
            var user = await userService.GetById(id);
            if (user != null)
            {
                await userService.DeleteById(id);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> UserExists(int id)
        {
            var x = await userService.GetById(id);
            return x != null ? true: false ;
        }
    }
}
