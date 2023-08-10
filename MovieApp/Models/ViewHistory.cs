using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class ViewHistory
    {
        [ForeignKey("Movies")]
        public int MovieID { get; set; }
        [ForeignKey("Profile")]
        public int ProfileID { get; set; }
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }
        public int progress { get; set; }
        public virtual ICollection<Series> Series { get; set; } = new HashSet<Series>();
        public virtual ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
        public virtual Profile Profile { get; set; }


    }
}
