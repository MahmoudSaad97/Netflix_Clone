using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Controllers
{
    [Authorize(Roles ="Admin,Basic")]
    public class ProfileController : Controller
    {
        private readonly IProfileService profileService;
        private readonly ILoggedDataService loggedDataService;
        private readonly IProfileUserService profileUserService;
        private readonly ISubscribesService subscribesService;
        private readonly IPaymentService paymentService;
        private readonly RoleManager<IdRoles> roleManager;
        public ProfileController(IProfileService _profileService, ILoggedDataService _loggedDataService, IProfileUserService _profileUserService, 
            ISubscribesService _subscribesService, RoleManager<IdRoles> _roleManager, IPaymentService _paymentService)
        {
            profileService = _profileService;
            loggedDataService = _loggedDataService;
            profileUserService = _profileUserService;
            subscribesService = _subscribesService;
            roleManager = _roleManager;
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

        public async Task<IActionResult> Index()
        {
            int userId = loggedDataService.LoggedUserId();
            ViewBag.ProfileCount = profileUserService.GetProfileByUserId(userId).Result.Count;

            return View(await profileService.GetProfileByUserId(userId));
        }
        public IActionResult Create(int? id)
        {
            if (id == null)
                return BadRequest("You can't Access this page directly!");

            int userId = loggedDataService.LoggedUserId();
            if (profileUserService.GetProfileByUserId(userId).Result.Count >= 5)
                return NotFound("Access denied!");

            ViewBag.id = id;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Profile Prof)
        {
            if (ModelState.IsValid)
            {
                await profileService.Add(Prof);

                return RedirectToAction("Index", "Profile");
            }

            return View();
        }
        public async Task<IActionResult> ViewProfileToEdit()
        {
            return View(await profileService.GetProfileByUserId(loggedDataService.LoggedUserId()));
        }
        public async Task<IActionResult> Edit(int? id, int? imgid)
        {
            if (id == null || imgid == null)
                return BadRequest("You can't Access this page directly!");

            ViewBag.imgid = imgid;

            ProfileUser? ProfUser = profileUserService.GetProfileByUserId(loggedDataService.LoggedUserId()).Result.Where(a => a.ProfileId == id).FirstOrDefault();

            if (ProfUser == null)
                return NotFound("Access denied!");

            Profile selectedProfile = await profileService.GetById((int)id);

            return View(selectedProfile);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Profile Prof)
        {
            if (ModelState.IsValid)
            {
                await profileService.Update(Prof);

                return RedirectToAction("Index", "Profile");
            }

            return View();
        }
        public async Task<IActionResult> ConfirmDelete(int? id, int? imgid)
        {
            if (id == null || imgid == null)
                return BadRequest("You can't Access this page directly!");

            ViewBag.imgid = imgid;

            ProfileUser? ProfUser = profileUserService.GetProfileByUserId(loggedDataService.LoggedUserId()).Result.Where(a => a.ProfileId == id).FirstOrDefault();

            if (ProfUser == null)
                return NotFound("Access denied!");

            Profile selectedProfile = await profileService.GetById((int)id);

            return View(selectedProfile);
        }
        public async Task<IActionResult> Delete(int id)
        {
            await profileService.DeleteById(id);

            return RedirectToAction("Index", "Profile");
        }
    }
}
