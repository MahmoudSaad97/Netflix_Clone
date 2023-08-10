using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Areas.Identity.Pages.Account.Manage
{
    public class PaymentLogsModel : PageModel
    {
        private readonly IPaymentService paymentService;

        public PaymentLogsModel(IPaymentService _paymentService)
        {
            paymentService = _paymentService;
        }

        public List<Payment> userPayments { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            userPayments = await paymentService.GetAllByLoggedUserId();

            return Page();
        }
    }
}
