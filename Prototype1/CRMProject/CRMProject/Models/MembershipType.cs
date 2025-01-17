using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class MembershipType
    {
        public int ID { get; set; }

        [Display(Name = "Membership Type")]
        [Required(ErrorMessage = "You cannot leave the membership type blank.")]
        public MembershipTypeName MembershipTypeName { get; set; }

        [Display(Name = "Membership Type Description")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for description.")]
        [DataType(DataType.MultilineText)]
        public string MembershipTypeDescription { get; set; } = "";

        [Display(Name = "Membership Type Fees")]
        [Required(ErrorMessage = "You must enter the fee for the membership type.")]
        [DataType(DataType.Currency)]
        public double MembershipTypeFees { get; set; }

        [Display(Name = "Membership Type Benefit")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for benefit.")]
        [DataType(DataType.MultilineText)]
        public string MembershipTypeBenefit { get; set; } = "";


    }
}
