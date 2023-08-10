using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieApp.IServices;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Areas.Identity.Pages.Account.Manage
{
    public class ViewRateHistoryModel : PageModel
    {
        private readonly ILoggedDataService loggedDataService;
        private readonly IUserService userService;
        private readonly IProfileService profileService;
        public ViewRateHistoryModel(ILoggedDataService _loggedDataService, IUserService _userService,
            IProfileService _profileService)
        {
            loggedDataService = _loggedDataService;
            userService = _userService;
            profileService = _profileService;
        }
        public User loggedUser { get; set; }
        public List<Profile> loggedProfiles { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var userid = loggedDataService.LoggedUserId();
            if (userid <= 0) return NotFound();

            var user = await userService.GetById(userid);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userid}'.");
            }

            var Profiles = await profileService.GetProfileByUserId(userid);
            if(Profiles == null) return NotFound();

            loggedProfiles = Profiles;
            loggedUser = user;

            return Page();
        }
    }
}
