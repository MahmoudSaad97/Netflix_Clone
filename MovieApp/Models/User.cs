using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MovieApp.Models
{
    public class User : IdentityUser<int>
    {
        #region Columns

        [MaxLength(20)]
        [Required(ErrorMessage = "First name is required.")]
        public string fname { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Last name is required.")]
        public string lname { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Country is required.")]
        public string country { get; set; }

        [Required(ErrorMessage = "Birth date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime BirthDate { get; set; }
        #endregion

        #region navigation Properties
        public virtual Subscribe? Subscribe { get; set; }
        public virtual ICollection<Payment>? Payment { get; set; } = new HashSet<Payment>();
        public virtual ICollection<ProfileUser>? ProfileUsers { get; set; } = new HashSet<ProfileUser>();
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is User))
                return false;

            User other = (User)obj;
            return Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion
    }
}
