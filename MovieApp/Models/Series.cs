using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
    public class Series
    {
        #region Columns
        public int SeriesID { get; set; }
        [MaxLength(20)]
        [Required(ErrorMessage = "Series name is required.")]
        public string SeriesName { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }
        [MaxLength(20)]

        public string? Poster { get; set; }

        [Range(0.0, 10.0, ErrorMessage = "Rate must be a number between 0 and 10.")]
        public float? Rate { get; set; }
        public int? Votes { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Views must be a positive number.")]
        public int? Views { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }        
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [DisplayName("Series Trailer")]
        [Url(ErrorMessage = "Invalid link format.")]
        public string? Trailer { get; set; }

        public string Director { get; set; }

        #endregion

        #region navigates
        public virtual ICollection<Genrie> Genries { get; set; } = new HashSet<Genrie>();
        public virtual ICollection<Season> Seasons { get; set; } = new HashSet<Season>();
        public virtual ICollection<Actor> Actors { get; set; } = new HashSet<Actor>();
        public virtual ICollection<Profile> Profiles { get; set; } = new HashSet<Profile>();
        public virtual ICollection<SeriesViewHistory> ViewHistories { get; set; } = new HashSet<SeriesViewHistory>();

        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Series))
                return false;

            Series other = (Series)obj;
            return SeriesID == other.SeriesID;
        }
        public override int GetHashCode()
        {
            return SeriesID.GetHashCode();
        }
        #endregion

    }
}
