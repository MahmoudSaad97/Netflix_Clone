using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Controllers
{
    [Authorize(Roles ="Admin")]
    public class PaymentsController : Controller
    {
        private readonly IPaymentService paymentService;
        private readonly ILoggedDataService loggedDataService;
        private readonly IUserService userService;
        private readonly ISubscribesService subscribesService;

        public PaymentsController(IPaymentService _paymentService, ILoggedDataService _loggedDataService,
            IUserService _userService, ISubscribesService _subscribesService)
        {
            paymentService = _paymentService;
            loggedDataService = _loggedDataService;
            userService = _userService;
            subscribesService = _subscribesService;
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

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            return View(await paymentService.GetAll());
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || await paymentService.GetAll() == null)
            {
                return NotFound();
            }

            var payment = await paymentService.GetById((int)id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public async Task<IActionResult> Create()
        {
            var x = new SelectList(await userService.GetAll(), "Id", "Email");
            ViewData["UserID"] = x;
            ViewData["Subscribes"] = await subscribesService.GetAllSubscriptionsWithRole();
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentID,PaymentMethod,CreditNumber,PaymentDate,UserID")] Payment payment, int subID)
        {
            ModelState.Remove("User");
            ModelState.Remove("Subscribe");
            payment.Subscribe = await subscribesService.GetById(subID);
            if (ModelState.IsValid)
            {
                await paymentService.Add(payment);
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(await userService.GetAll(), "Id", "Email", payment.UserID);
            ViewData["Subscribes"] = await subscribesService.GetAllSubscriptionsWithRole();
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || await paymentService.GetAll() == null)
            {
                return NotFound();
            }

            var payment = await paymentService.GetById((int)id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(await userService.GetAll(), "Id", "Email", payment.UserID);
            ViewData["Subscribes"] = await subscribesService.GetAllSubscriptionsWithRole();
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentID,PaymentMethod,CreditNumber,PaymentDate,UserID")] Payment payment, int subID)
        {
            if (id != payment.PaymentID)
            {
                return NotFound();
            }

            ModelState.Remove("User");
            ModelState.Remove("Subscribe");
            payment.Subscribe = await subscribesService.GetById(subID);
            if (ModelState.IsValid)
            {
                try
                {
                    await paymentService.Update(payment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await PaymentExists(payment.PaymentID)))
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
            ViewData["UserID"] = new SelectList(await userService.GetAll(), "Id", "Email", payment.UserID);
            ViewData["Subscribes"] = await subscribesService.GetAllSubscriptionsWithRole();
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || await paymentService.GetAll() == null)
            {
                return NotFound();
            }

            var payment = await paymentService.GetById((int)id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await paymentService.GetAll() == null)
            {
                return Problem("Entity set 'MovieAppContext.Payments'  is null.");
            }
            var payment = await paymentService.GetById(id);
            if (payment != null)
            {
               await paymentService.DeleteById(id);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PaymentExists(int id)
        {
            var x = await paymentService.GetById(id);
            return x != null ? true : false;
        }
    }
}
