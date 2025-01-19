using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class ContactEmail
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "You cannot leave the email type blank.")]
        [Display(Name = "Email Type")]
        public EmailType EmailType { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please follow the correct email format test@email.com")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; } = "";

        public int ContactID { get; set; }
        public Contact? Contact { get; set; }
    }
}
