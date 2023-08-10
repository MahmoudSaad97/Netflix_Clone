using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using Microsoft.AspNetCore.Identity;
using WebPWrecover.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.IServices;
using MovieApp.Services;
using NToastNotify;

namespace MovieApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddTransient<IProfileService, ProfileService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<ILoggedDataService, LoggedDataService>();
            builder.Services.AddTransient<IProfileUserService, ProfileUserService>();
            builder.Services.AddTransient<IMovieActorsService, MovieActorsService>();
            builder.Services.AddTransient<IMovieGenriesService, MovieGenriesService>();
            builder.Services.AddTransient<IMovieService, MovieService>();
            builder.Services.AddTransient<IEpisodesService, EpisodesService>();
            builder.Services.AddTransient<ISeriesService, SeriesService>();
            builder.Services.AddTransient<ISeasonService, SeasonService>();
            builder.Services.AddTransient<ISeriesGenriesService, SeriesGenriesService>();
            builder.Services.AddTransient<ISeriesActorsService, SeriesActorsService>();
            builder.Services.AddTransient<ProfileMoviesServices, ProfileMoviesServices>();
            builder.Services.AddTransient<SeasonService, SeasonService>();
            builder.Services.AddTransient<ProfileSeriesServices, ProfileSeriesServices>();
            builder.Services.AddTransient<IActorService,  ActorService>();
            builder.Services.AddTransient<IGenrieService, GenrieService>();
            builder.Services.AddTransient<ISubscribesService, SubscribesService>();
            builder.Services.AddTransient<IPaymentService, PaymentService>();
            builder.Services.AddTransient<IBraintreeService, BraintreeService>();
            builder.Services.AddTransient<ICountryService, CountryService>();

            builder.Services.AddDbContext<MovieAppContext>(a=>
                a.UseSqlServer(builder.Configuration.GetConnectionString("MovieConn")));

            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdRoles>()
                .AddEntityFrameworkStores<MovieAppContext>();

            // Toast Notification
            builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
            {
                ProgressBar = true,
                PositionClass = ToastPositions.TopRight,
                PreventDuplicates = true,
                CloseButton = true
            });

            //Access ClaimPrinciples
            builder.Services.AddHttpContextAccessor();

            //Email
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            
            builder.Services.Configure<AuthMessageSenderOptions>
                (options => builder.Configuration.GetSection("EmailSettings").Bind(options));

            //Session
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();;

            app.UseAuthorization();
            //Session
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}