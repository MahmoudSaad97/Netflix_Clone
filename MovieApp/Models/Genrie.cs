using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace MovieApp.Models
{
    public class Genrie
    {
        #region Columns
        public int GenrieID { get; set; }
        [MaxLength(20)]
        public string GenrieName { get; set; }
        #endregion

        #region Navigators
        public virtual ICollection<Movie> Actors { get; set; } = new HashSet<Movie>();
        public virtual ICollection<Series> Series { get; set; } = new HashSet<Series>();
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Genrie))
                return false;

            Genrie other = (Genrie)obj;
            return GenrieID == other.GenrieID;
        }
        public override int GetHashCode()
        {
            return GenrieID.GetHashCode();
        }
        #endregion
    }
}
