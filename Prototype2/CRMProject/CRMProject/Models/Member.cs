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
        [Range(1, int.MaxValue, ErrorMessage = "Member size must be a positive number.")]
        public int? MemberSize { get; set; }

        [Required(ErrorMessage = "You must select the member status.")]
        [Display(Name = "Status")]
        public MemberStatus? MemberStatus { get; set; }

        [Required(ErrorMessage = "You cannot leave the member accounts payable email blank.")]
        [Display(Name = "A/P Email")]
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

        [Display(Name = "Last Contacted")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? MemberLastContactDate { get; set; } = DateTime.Today;

        [Display(Name = "Notes")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for note.")]
        [DataType(DataType.MultilineText)]
        public string? MemberNotes { get; set; } = "";
               
        [Display(Name = "Address")]
        public Address? Address { get; set; }

        [Display(Name = "Industries")]
        public ICollection<MemberIndustry> MemberIndustries { get; set; } = new HashSet<MemberIndustry>();

        [Display(Name = "Membership Types")]
        public ICollection<MemberMembershipType> MemberMembershipTypes { get; set; } = new HashSet<MemberMembershipType>();

        [Display(Name = "StatusHistory")]
        public ICollection<StatusHistory> StatusHistories { get; set; } = new HashSet<StatusHistory>();

        [Display(Name = "Contacts")]
        public ICollection<MemberContact> MemberContacts { get; set; } = new HashSet<MemberContact>();

        public MemberPhoto? MemberPhoto { get; set; }
        public MemberThumbnail? MemberThumbnail { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MemberEndDate < MemberStartDate)
            {
                yield return new ValidationResult("Membership end date cannot be earlier than the start date.",
                    new[] { "MemberEndDate" });
            }

            if (MemberStartDate > DateTime.Today)
            {
                yield return new ValidationResult("Member start date cannot be in the future.",
                    new[] { "MemberStartDate" });
            }

            if (MemberLastContactDate > DateTime.Today)
            {
                yield return new ValidationResult("Last contact date cannot be in the future.",
                    new[] { "MemberLastContactDate" });
            }
        }
    }
}