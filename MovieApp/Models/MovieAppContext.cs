using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace MovieApp.Models
{
    public class MovieAppContext:IdentityDbContext<User,IdRoles,int>//DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Series> Series { get; set; }
        public virtual DbSet<Subscribe> Subscribes { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Season> Seasons { get; set; }
        public virtual DbSet<Episode> Eposides { get; set; }
        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<Genrie> Genries { get; set; }
        public virtual DbSet<RatedMovie> MoviesRates { get; set; }
        public virtual DbSet<RatedSeries> SeriesRates { get; set; }
        public virtual DbSet<MovieViewHistory> MovieviewHistories { get; set; }
        public virtual DbSet<SeriesViewHistory> SeriesviewHistories { get; set; }
        public virtual DbSet<EpisodeViewHistory> EpisodeViewHistories { get; set; }
        public virtual DbSet<ProfileUser> ProfilesUsers { get; set; }
        public virtual DbSet<Country> Countries { get; set; }

        public MovieAppContext(DbContextOptions<MovieAppContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpisodeViewHistory>()
                .HasOne(ev => ev.SeriesHistory)
                .WithMany(sh => sh.EpisodeViewHistories)
                .HasForeignKey(ev => ev.HistoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProfileUser>().HasKey(a => new { a.ProfileId, a.UserId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
