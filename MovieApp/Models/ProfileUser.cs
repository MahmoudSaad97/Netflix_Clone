using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class ProfileUser
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }

        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
