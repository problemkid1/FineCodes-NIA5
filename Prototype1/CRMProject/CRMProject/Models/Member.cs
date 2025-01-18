using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Member
    {
        public int ID { get; set; }

        [Display(Name = "Member Name")]
        [Required(ErrorMessage = "You cannot leave the member name blank.")]
        [MaxLength(255, ErrorMessage = "Member Name cannot be more than 255 characters long.")]
        public string MemberName { get; set; } = "";

        [Required(ErrorMessage = "You cannot leave the member size blank.")]
        [Display(Name = "Member Size")]
        public int? MemberSize { get; set; }

        [Required(ErrorMessage = "You must select the member status.")]
        [Display(Name = "Member Status")]
        public MemberStatus MemberStatus { get; set; }

        [Required(ErrorMessage = "You cannot leave the member accounts payable email blank.")]
        [Display(Name = "Member Accounts Payable Email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please follow the correct email format test@email.com")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string MemberAccountsPayableEmail { get; set; }

        [Display(Name = "Member Start Date")]
        [Required(ErrorMessage = "Member start date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime MemberStartDate { get; set; } = DateTime.Today;

        [Display(Name = "Member End Date")]
        [Required(ErrorMessage = "Member end date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? MemberEndDate { get; set; } = DateTime.Today.AddYears(1);  // Nullable DateTime

        [Display(Name = "Member Notes")]
        [Required(ErrorMessage = "You must enter comments about the member.")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for note.")]
        [DataType(DataType.MultilineText)]
        public string MemberNotes { get; set; } = "";

    }
}
