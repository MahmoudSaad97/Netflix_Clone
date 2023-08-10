using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class RatedMovie
    {
        #region Fields
        [Key]
        public int RateID { get; set; }
        [ForeignKey("Movie")]
        public int MovieID { get; set; }

        [ForeignKey("Profile")]
        public int ProfileID { get; set; }
        public int Rateing { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        #endregion

        #region Navigates
        public virtual Movie Movie { get; set; }
        public virtual Profile Profile { get; set; }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is RatedMovie))
                return false;

            RatedMovie other = (RatedMovie)obj;
            return RateID == other.RateID;
        }
        public override int GetHashCode()
        {
            return RateID.GetHashCode();
        }
        #endregion

    }
}
