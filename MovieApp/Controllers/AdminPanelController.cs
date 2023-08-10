using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.IServices;
using MovieApp.Models;
using MovieApp.Services;
using NuGet.Packaging;
using System.Data;
using System.Text.Json;

namespace MovieApp.Controllers
{
    //allow admins only to access this.
    [Authorize(Roles = "Admin")]
    public class AdminPanelController : Controller
    {
        private readonly ILoggedDataService loggedDataService;
        private readonly IUserService userService;
        private readonly IMovieService movieService;
        private readonly ISeriesService seriesService;
        private readonly IPaymentService paymentService;
        public AdminPanelController(ILoggedDataService _loggedDataService, IUserService _userService,
            IMovieService _movieService, ISeriesService _seriesService, IPaymentService _paymentService)
        {
            loggedDataService = _loggedDataService;
            userService = _userService;
            movieService = _movieService;
            seriesService = _seriesService;
            paymentService = _paymentService;
        }
        public async Task<IActionResult> Index()
        {
            int userId = loggedDataService.LoggedUserId();
            if (userId > 0)
            {
                User loggeduser = await userService.GetById(userId);
                bool admin = await loggedDataService.checkUserAccessByRoleName(userId, "Admin");
                if (loggeduser != null && admin)
                {
                    ViewBag.loggedUserFullName = loggeduser.fname + " " + loggeduser.lname;
                    var totalUsers = await userService.GetAll();
                    var totalMovies = await movieService.GetAll();
                    var totalSeries = await seriesService.GetAll();
                    var Total = totalUsers.Count + totalMovies.Count + totalSeries.Count;
                    ViewBag.TotalUserCount = Math.Round(((decimal)totalUsers.Count / Total) * 100);
                    ViewBag.TotalMoviesCount = Math.Round(((decimal)totalMovies.Count / Total) * 100);
                    ViewBag.TotalSeriesCount = Math.Round(((decimal)totalSeries.Count / Total) * 100);

                    var movieYears = totalMovies.Select(m => m.ReleaseDate.Value.Year).ToList();
                    var seriesYears = totalSeries.Select(s => s.ReleaseDate.Value.Year).ToList();

                    var totalpayments = await paymentService.GetAll();

                    ViewBag.TotalObject = JsonSerializer.Serialize(TotalObject(totalMovies,totalSeries));
                    ViewBag.PaymentObject = JsonSerializer.Serialize(PaymentObject(totalpayments));

                    return View();
                }
                return NotFound("Access denied!");
            }
            return NotFound("Access denied!");
        }
        private object myObject(List<int> AllYears, List<int> Years)
        {
            List<int> Year = new List<int>();
            List<int> Count = new List<int>();
            int i = 0;
            foreach (var item in AllYears)
            {
                if (!Year.Contains(item))
                {
                    Year.Add(item);
                    Count.Insert(i, Years.Count(a => a == item));
                    i++;
                }
            }
            return new { Year = Year, Count = Count };
        }

        private object TotalObject(List<Movie> movies, List<Series> series)
        {
            var movieYears = movies.Select(m => m.ReleaseDate.Value.Year).ToList();
            var seriesYears = series.Select(s => s.ReleaseDate.Value.Year).ToList();
            List<int> AllYears = movieYears.Concat(seriesYears).Distinct().ToList();
            AllYears.Sort();

            var moviesObject = myObject(AllYears, movieYears);
            var seriesObject = myObject(AllYears, seriesYears);

            return new { Years = AllYears, Movies = moviesObject, Series = seriesObject };
        }
        private object PaymentObject(List<Payment> totalPayments)
        {
            var paymentDateTime = totalPayments.Select(a => a.PaymentDate).ToList();
            var currYear = DateTime.Now.Year;
            var currMonth = DateTime.Now.Month;
            List<int> TotalRevenue = new List<int>();
            List<int> DaysOfCurrentMonth = new List<int>();
            var DaysOfMonthCount = DateTime.DaysInMonth(currYear, currMonth);
            for (int i =1; i<= DaysOfMonthCount; i++)
                DaysOfCurrentMonth.Add(i);

            foreach(var item in DaysOfCurrentMonth)
            {
                var paymentsOfCurrMonth = totalPayments.Where(a => a.PaymentDate.Year == currYear && a.PaymentDate.Month == currMonth && a.PaymentDate.Day == item)
                .Select(a => a.Subscribe.Price).ToList();

                if (paymentsOfCurrMonth.Count == 0)
                    TotalRevenue.Insert(item-1, 0);
                else
                {
                    int Total = 0;
                    foreach (var payment in paymentsOfCurrMonth)
                    {
                        Total += payment;
                    }
                    TotalRevenue.Insert(item - 1, Total);
                }
            }

            return new { Revenue = TotalRevenue, Days = DaysOfCurrentMonth };
        }
    }
}
