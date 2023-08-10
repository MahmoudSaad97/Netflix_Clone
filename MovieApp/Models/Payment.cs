using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Credit card number is required.")]
        [MaxLength(20)]
        [CreditCard(ErrorMessage = "Invalid credit card number.")]
        public string CreditNumber { get; set; }

        [Required(ErrorMessage = "Payment date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }

        #region navigate
        public virtual User User { get; set; }
        public virtual Subscribe Subscribe { get; set; }
        #endregion
    }
}
