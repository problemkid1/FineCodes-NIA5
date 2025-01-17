using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Address
    {
        public int ID { get; set; }

        [Display(Name = "Address Line 1")]
        [Required(ErrorMessage = "You cannot leave the address line 1 blank.")]
        [MaxLength(50, ErrorMessage = "Address line 1 cannot be more than 50 characters long.")]
        public string AddressLine1 { get; set; } = "";

        [Display(Name = "Address Line 2")]
        [MaxLength(50, ErrorMessage = "Address line 2 cannot be more than 50 characters long.")]
        public string AddressLine2 { get; set; } = "";

        [Display(Name = "City")]
        [Required(ErrorMessage = "You cannot leave the city blank.")]
        [MaxLength(50, ErrorMessage = "City cannot be more than 50 characters long.")]
        public string AddressCity { get; set; } = "";

        [Display(Name = "Province")]
        [Required(ErrorMessage = "You cannot leave the province blank.")]
        [MaxLength(50, ErrorMessage = "Province cannot be more than 50 characters long.")]
        public string Province { get; set; } = "";

        [Display(Name = "Postal Code")]
        [Required]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Invalid postal code.")]
        public string PostalCode { get; set; } = "";

        [Required(ErrorMessage = "You must select the address type.")]
        [Display(Name = "Address Type")]
        public AddressType AddressType { get; set; }
    }
}
