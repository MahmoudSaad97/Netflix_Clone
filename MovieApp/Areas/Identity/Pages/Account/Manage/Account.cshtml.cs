using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Areas.Identity.Pages.Account.Manage
{
    public class AccountModel : PageModel
    {
        private readonly ILoggedDataService loggedDataService;
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdRoles> roleManager;
        private readonly IPaymentService paymentService;
        private readonly ISubscribesService subscribesService;

        public AccountModel(ILoggedDataService _loggedDataService,IUserService _userService,
            UserManager<User> _userManager, RoleManager<IdRoles> _roleManager, IPaymentService _paymentService,
            ISubscribesService _subscribesService)
        {
            loggedDataService = _loggedDataService;
            userService = _userService;
            userManager = _userManager;
            roleManager = _roleManager;
            paymentService = _paymentService;
            subscribesService = _subscribesService;
        }
        public User loggedUser { get; set; }
        public string userSubscription { get; set; }
        public DateTime ExpirationDate { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var userid = loggedDataService.LoggedUserId();
            if (userid <= 0) return NotFound();

            var user = await userService.GetById(userid);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userid}'.");
            }

            //foreach (var item in roleManager.Roles.ToList())
            //    if ((await userManager.IsInRoleAsync(user, item.Name)))
            //        userRole = item.Name;

            userSubscription = await subscribesService.GetLastSubscriptionWithPrice();

            ExpirationDate = await paymentService.GetDateTimeExpireByLoggedUserId();


            loggedUser = user;

            return Page();
        }
    }
}
