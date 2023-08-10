using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class MovieViewHistory
    {
        #region Columns
        [Key]
        public int HistoryID { get; set; }
        [ForeignKey("Movie")]
        public int MovieID { get; set; }
        [ForeignKey("Profile")]
        public int ProfileID { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int? ProgressMinutes { get; set; }
        #endregion

        #region Navigateors
        public virtual Movie Movie { get; set; }
        public virtual Profile Profile { get; set; }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MovieViewHistory))
                return false;

            MovieViewHistory other = (MovieViewHistory)obj;
            return HistoryID == other.HistoryID;
        }
        public override int GetHashCode()
        {
            return HistoryID.GetHashCode();
        }
        #endregion


    }
}
