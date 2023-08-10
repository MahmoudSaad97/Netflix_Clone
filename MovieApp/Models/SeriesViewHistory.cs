using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class SeriesViewHistory
    {
        #region Columns

        [Key]
        public int HistoryID { get; set; }
        [ForeignKey("Series")]
        public int SeriesID { get; set; }
        [ForeignKey("Profile")]
        public int ProfileID { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        #endregion

        #region Navigators
        public virtual Series Series { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual ICollection<EpisodeViewHistory> EpisodeViewHistories { get; set; } = new HashSet<EpisodeViewHistory>();
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SeriesViewHistory))
                return false;

            SeriesViewHistory other = (SeriesViewHistory)obj;
            return HistoryID == other.HistoryID;
        }
        public override int GetHashCode()
        {
            return HistoryID.GetHashCode();
        }
        #endregion
    }
}
