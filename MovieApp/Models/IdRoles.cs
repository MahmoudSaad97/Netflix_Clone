using Microsoft.AspNetCore.Identity;

namespace MovieApp.Models
{
    public class IdRoles:IdentityRole<int>
    {
        public virtual ICollection<Subscribe> Subscribes { get; set; } = new HashSet<Subscribe>();
    }
}
