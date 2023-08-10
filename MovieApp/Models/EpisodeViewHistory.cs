using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class EpisodeViewHistory
    {
        #region Columns      
        [Key]
        public int EpisodeHistoryID { get; set; }

        [ForeignKey("SeriesHistory")]
        public int HistoryID { get; set; }

        [ForeignKey("Episode")]
        public int EpisodeID { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public int? ProgressMinutes { get; set; }
        #endregion
        public virtual SeriesViewHistory SeriesHistory { get; set; }
        public virtual Episode Episode { get; set; }

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is EpisodeViewHistory))
                return false;

            EpisodeViewHistory other = (EpisodeViewHistory)obj;
            return EpisodeHistoryID == other.EpisodeHistoryID;
        }
        public override int GetHashCode()
        {
            return EpisodeHistoryID.GetHashCode();
        }
        #endregion
    }
}
