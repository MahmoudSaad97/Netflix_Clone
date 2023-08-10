using Braintree;
using Microsoft.AspNetCore.Mvc;
using MovieApp.IServices;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Controllers
{
    public class RenewController : Controller
    {

        private readonly IBraintreeService braintreeService;
        private readonly ISubscribesService subscribesService;
        private readonly IPaymentService paymentService;
        private readonly ILoggedDataService loggedDataService;
        public RenewController(IBraintreeService _braintreeService, ISubscribesService _subscribesService, 
            IPaymentService _paymentService, ILoggedDataService _loggedDataService)
        {
            braintreeService = _braintreeService;
            subscribesService = _subscribesService;
            paymentService = _paymentService;
            loggedDataService = _loggedDataService;

        }
        public async Task<IActionResult> Index()
        {
            var gateway = braintreeService.GetGateway();
            var clientToken = gateway.ClientToken.Generate();  //Genarate a token
            ViewData["ClientToken"] = clientToken;

            ViewData["PrevRole"] = await subscribesService.GetLastSubscriptionWithPrice();


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRenew(string Nonce)
        {
            var gateway = braintreeService.GetGateway();
            var clientToken = gateway.ClientToken.Generate();  //Genarate a token
            ViewData["ClientToken"] = clientToken;

            ViewData["Subscribes"] = await subscribesService.GetAllSubscriptionsWithRole();

            var prevPayment = await paymentService.GetAll();
            if (prevPayment != null)
            {
                var prevSubscriptionId = prevPayment.Select(a => a.Subscribe.SubscribeID).FirstOrDefault();
                if (prevSubscriptionId > 0)
                {
                    var Amount = await subscribesService.GetById(prevSubscriptionId);
                    if (Amount != null)
                    {
                        ViewData["PrevRole"] = Amount.Role.Name + " - " + Amount.Price + "$";

                        var request = new TransactionRequest
                        {
                            Amount = Convert.ToDecimal(Amount.Price),
                            PaymentMethodNonce = Nonce,
                            Options = new TransactionOptionsRequest
                            {
                                SubmitForSettlement = true
                            }
                        };

                        Result<Transaction> paymentResult = gateway.Transaction.Sale(request);
                        if (paymentResult.IsSuccess())
                        {
                            Payment payment = new Payment()
                            {
                                PaymentMethod = "Credit",
                                CreditNumber = "XXXX-XXXX-XXXX-XXXX",
                                Subscribe = await subscribesService.GetById(prevSubscriptionId),
                                UserID = loggedDataService.LoggedUserId(),
                                PaymentDate = DateTime.Now
                            };
                            await paymentService.Add(payment);

                            return RedirectToAction("Index", "Profile");
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
                    else return NotFound("No previous data founded!");
                }
                else return NotFound("No previous data founded!");
            }
            else return NotFound("No previous data founded!");
        }
    }
}
