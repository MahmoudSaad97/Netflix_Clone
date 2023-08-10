using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
    public class Actor
    {
        #region Columns
        public int ActorID { get; set; }
        [MaxLength(20)]
        public string ActorName { get; set; }
        #endregion

        #region Navigateors
        public virtual ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
        public virtual ICollection<Series> Series { get; set; } = new HashSet<Series>();
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Actor))
                return false;

            Actor other = (Actor)obj;
            return ActorID == other.ActorID;
        }
        public override int GetHashCode()
        {
            return ActorID.GetHashCode();
        }
        #endregion
    }
}
