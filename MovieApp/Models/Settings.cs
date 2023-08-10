using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class Settings
    {
        [ForeignKey("Profile"),Key]
        public int ProfileID { get; set; }
        [MaxLength(20)]
        public string Language { get; set; }
        public float Rating { get; set; }
        public bool profilelock { get; set; }
        public int quality { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
