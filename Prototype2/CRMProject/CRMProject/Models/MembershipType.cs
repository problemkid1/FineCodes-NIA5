using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class MembershipType
    {
        public int ID { get; set; }

        [Display(Name = "Membership Type")]
        [Required(ErrorMessage = "You cannot leave the membership type blank.")]
        public string MembershipTypeName { get; set; }

        [Display(Name = "Description")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for description.")]
        [DataType(DataType.MultilineText)]
        public string? MembershipTypeDescription { get; set; }

        [Display(Name = "Fee")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Fee must be a positive value.")]
        public double? MembershipTypeFee { get; set; }

        [Display(Name = "Benefit")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for benefit.")]
        [DataType(DataType.MultilineText)]
        public string? MembershipTypeBenefits { get; set; } 

        [Display(Name = "Member")]
        public ICollection<MemberMembershipType> MemberMembershipTypes { get; set; } = new HashSet<MemberMembershipType>();
    }
}
