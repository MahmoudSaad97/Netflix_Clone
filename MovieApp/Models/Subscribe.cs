using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MovieApp.Models
{
    public class Subscribe
    {
        public int SubscribeID { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public int Price { get; set; }

        #region navigate
        [ForeignKey("RoleId")]
        public virtual IdRoles Role {get; set;}
        #endregion
    }
}
