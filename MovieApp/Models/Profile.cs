using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class Profile
    {
        #region Columns

        [Key]
        public int ProfileID { get; set; }
        [StringLength(14), Required]
        public string ProfileName { get; set; }
        #endregion

        #region Navigators
        public virtual ICollection<ProfileUser>? ProfileUsers { get; set; } = new HashSet<ProfileUser>();
        public virtual ICollection<Movie>? Movies { get; set; } = new HashSet<Movie>();
        public virtual ICollection<Series>? Series { get; set; } = new HashSet<Series>();
        public virtual ICollection<RatedMovie>? RatedMovies { get; set; } = new HashSet<RatedMovie>();
        public virtual ICollection<RatedSeries>? RatedSeries { get; set; } = new HashSet<RatedSeries>();
        public virtual ICollection<MovieViewHistory> MovieViewHistories { get; set; } = new HashSet<MovieViewHistory>();
        public virtual ICollection<SeriesViewHistory> SeriesViewHistories { get; set; } = new HashSet<SeriesViewHistory>();
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Profile))
                return false;

            Profile other = (Profile)obj;
            return ProfileID == other.ProfileID;
        }
        public override int GetHashCode()
        {
            return ProfileID.GetHashCode();
        }
        #endregion
    }
}
