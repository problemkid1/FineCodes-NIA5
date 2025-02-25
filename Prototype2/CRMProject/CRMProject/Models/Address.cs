using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Address
    {
        public int ID { get; set; }


        [Display(Name = "Main Address")]
        public string Summary
        {
            get
            {
                return AddressLine1
                       + (string.IsNullOrEmpty(AddressLine2) ? "" :
                          ", " + AddressLine2);
            }
        }


        [Display(Name = "Address Line 1")]
        [Required(ErrorMessage = "You cannot leave the address line 1 blank.")]
        [MaxLength(255, ErrorMessage = "Address line 1 cannot be more than 255 characters long.")]
        public string AddressLine1 { get; set; } = "";

        [Display(Name = "Address Line 2")]
        [MaxLength(255, ErrorMessage = "Address line 2 cannot be more than 255 characters long.")]
        public string? AddressLine2 { get; set; }

        [Display(Name = "City")]
        [MaxLength(100, ErrorMessage = "City cannot be more than 100 characters long.")]

        public string? AddressCity { get; set; } = "";

        [Display(Name = "Province")]
        public Province? Province { get; set; } 

        [Display(Name = "Postal Code")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Invalid postal code.")]
        public string? PostalCode { get; set; } = "";

        [Required(ErrorMessage = "You must select a Member.")]
        [Display(Name = "Member")]
        public int MemberID { get; set; }
        public Member? Member { get; set; }
    }
}
