// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdRoles> _roleManager;
        private readonly IBraintreeService braintreeService;
        private readonly ISubscribesService subscribesService;
        private readonly IPaymentService paymentService;
        private readonly ICountryService countryService;

        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdRoles> roleManager,
            IEmailSender emailSender,
            IBraintreeService _braintreeService,
            ISubscribesService _subscribesService,
            IPaymentService _paymentService,
            ICountryService _countryService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
            braintreeService = _braintreeService;
            subscribesService = _subscribesService;
            paymentService = _paymentService;
            countryService = _countryService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            
            [MaxLength(20)]
            [Required(ErrorMessage = "First name is required.")]
            public string fname { get; set; }

            [MaxLength(20)]
            [Required(ErrorMessage = "Last name is required.")]
            public string lname { get; set; }

            public int CountryId { get; set; }

            [Required(ErrorMessage = "Birth date is required.")]
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime BirthDate { get; set; }

            public string Nonce { get; set; }

            public int SubId { get; set; }
        }
        public string CreditCardNumber { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var gateway = braintreeService.GetGateway();
            var clientToken = gateway.ClientToken.Generate();  //Genarate a token
            ViewData["ClientToken"] = clientToken;

            ViewData["Subscribes"] = await subscribesService.GetAllSubscriptionsWithRole();
            ViewData["Countries"] = await countryService.GetAllAsSelectList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            var gateway = braintreeService.GetGateway();
            var clientToken = gateway.ClientToken.Generate();  //Genarate a token
            ViewData["ClientToken"] = clientToken;

            ViewData["Subscribes"] = await subscribesService.GetAllSubscriptionsWithRole();
            ViewData["Countries"] = await countryService.GetAllAsSelectList();

            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var Amount = await subscribesService.GetById(Input.SubId);
                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(Amount.Price),
                    PaymentMethodNonce = Input.Nonce,
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                Result<Transaction> paymentResult = gateway.Transaction.Sale(request);

                if (paymentResult.IsSuccess())
                {
                    var user = CreateUser();

                    user.fname = Input.fname;
                    user.lname = Input.lname;
                    user.BirthDate = Input.BirthDate;
                    user.country = await countryService.GetNameById(Input.CountryId);

                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        Payment payment = new Payment()
                        {
                            PaymentMethod = "Credit",
                            CreditNumber = "XXXX-XXXX-XXXX-XXXX",
                            Subscribe = await subscribesService.GetById(Input.SubId),
                            UserID = user.Id,
                            PaymentDate = DateTime.Now
                        };
                        await paymentService.Add(payment);
                        string RoleName = await subscribesService.GetRoleBySubscribeId(Input.SubId);

                        var defaultrole = await _roleManager.FindByNameAsync(RoleName);

                        if (defaultrole != null)
                        {
                            IdentityResult roleresult = await _userManager.AddToRoleAsync(user, defaultrole.Name);
                        }
                        else
                        {
                            IdRoles basicRole = new IdRoles();
                            basicRole.Name = RoleName;
                            basicRole.NormalizedName = basicRole.Name.ToUpper();
                            var rolecreate = await _roleManager.CreateAsync(basicRole);

                            if (rolecreate.Succeeded)
                            {
                                await _userManager.AddToRoleAsync(user, RoleName);

                                if (await _roleManager.FindByNameAsync("Admin") == null)
                                {
                                    IdRoles adminRole = new IdRoles();
                                    adminRole.Name = "Admin";
                                    adminRole.NormalizedName = adminRole.Name.ToUpper();
                                    await _roleManager.CreateAsync(adminRole);
                                }
                            }
                        }

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    if (paymentResult.Errors.DeepCount > 0)
                    {
                        foreach (ValidationError error in paymentResult.Errors.DeepAll())
                        {
                            return NotFound(error.Message);
                        }
                    }

                    return NotFound("Failure");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
