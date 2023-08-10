using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
    public class Movie
    {
        #region Columns
        public int MovieID { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Movie name is required.")]
        public string MovieName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be a positive number.")]
        public int? Duration { get; set; }
        [MaxLength(500)]

        public string Description { get; set; }
        [MaxLength(20)]

        public string? Poster { get; set; }

        [Range(0.0, 10.0, ErrorMessage = "Rate must be a number between 0 and 10.")]
        public float? Rate { get; set; }
        public int? votes { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Views must be a positive number.")]
        public int? Views { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        public string Director { get; set; }
        [DisplayName("Movie Trailer")]
        [Url(ErrorMessage = "Invalid link format.")]
        public string? Trailer { get; set; }
        [DisplayName("Movie Video")]
        [Url(ErrorMessage = "Invalid link format.")]
        public string? Link { get; set; }
        #endregion

        #region navigates
        public virtual ICollection<Actor> Actors { get; set; } = new HashSet<Actor>();
        public virtual ICollection<Genrie> Genries { get; set; } = new HashSet<Genrie>();
        public virtual ICollection<Profile> Profiles { get; set; } = new HashSet<Profile>();
        public virtual ICollection<MovieViewHistory> MovieViewHistories { get; set; } = new HashSet<MovieViewHistory>();
        #endregion


        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Movie))
                return false;

            Movie other = (Movie)obj;
            return MovieID == other.MovieID;
        }
        public override int GetHashCode()
        {
            return MovieID.GetHashCode();
        }
        #endregion

    }
}
