using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class Season
    {
        #region Columns
        public int SeasonID { get; set; }
        [ForeignKey("Series")]
        public int SeriesID { get; set; }
        [MaxLength(50)]
        public string SeasonName { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public int EposidesCount { get; set; }
        #endregion

        #region Navigators
        public virtual ICollection<Episode> Episodes { get; set; } = new HashSet<Episode>();
        public virtual Series Series { get; set; }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Season))
                return false;

            Season other = (Season)obj;
            return SeasonID == other.SeasonID;
        }
        public override int GetHashCode()
        {
            return SeasonID.GetHashCode();
        }
        #endregion
    }
}
