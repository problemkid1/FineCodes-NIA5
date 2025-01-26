using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Member : IValidatableObject
    {
        public int ID { get; set; }

        public string Summary
        {
            get
            {
                return MemberName + ", " + MemberSize + "employees, " + MemberStatus;
            }
        }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "You cannot leave the member name blank.")]
        [MaxLength(255, ErrorMessage = "Member Name cannot be more than 255 characters long.")]
        public string MemberName { get; set; } = "";

        [Required(ErrorMessage = "You cannot leave the member size blank.")]
        [Display(Name = "Size")]
        public int? MemberSize { get; set; }

        [Required(ErrorMessage = "You must select the member status.")]
        [Display(Name = "Status")]
        public MemberStatus MemberStatus { get; set; }

        [Required(ErrorMessage = "You cannot leave the member accounts payable email blank.")]
        [Display(Name = "Accounts Payable Email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please follow the correct email format test@email.com")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string MemberAccountsPayableEmail { get; set; } = "";

        [Display(Name = "Join Date")]
        [Required(ErrorMessage = "Member start date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime MemberStartDate { get; set; } = DateTime.Today;

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? MemberEndDate { get; set; }

        [Display(Name = "Notes")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for note.")]
        [DataType(DataType.MultilineText)]
        public string? MemberNotes { get; set; } = "";

        [Display(Name = "Addresses")]
        public ICollection<Address> Addresses { get; set; } = new HashSet<Address>();

        [Display(Name = "Industries")]
        public ICollection<MemberIndustry> MemberIndustries { get; set; } = new HashSet<MemberIndustry>();

        [Display(Name = "Membership Types")]
        public ICollection<MemberMembershipType> MemberMembershipTypes { get; set; } = new HashSet<MemberMembershipType>();

        [Display(Name = "Cancellations")]
        public ICollection<Cancellation> Cancellations { get; set; } = new HashSet<Cancellation>();

        [Display(Name = "Contacts")]
        public ICollection<MemberContact> MemberContacts { get; set; } = new HashSet<MemberContact>();
        public MemberPhoto? MemberPhoto { get; set; }
        public MemberThumbnail? MemberThumbnail { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MemberEndDate < MemberStartDate)
            {
                yield return new ValidationResult("Membership end date cannot be earlier than the start date.", ["MemberEndDate"]);
            }
        }

    }
}
