using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Contact
    {
        public int ID { get; set; }

        [Display(Name = "Contact")]
        public string Summary
        {
            get
            {
                return FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? " " :
                        (" " + (char?)MiddleName[0] + ". ").ToUpper())
                    + LastName;
            }
        }

        [Display(Name = "Contact")]
        public string ContactType
        {
            get
            {
                return FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? " " :
                        (" " + (char?)MiddleName[0] + ". ").ToUpper())
                    + LastName + " - " + ContactEmailType;
            }
        }

        public string FormalName
        {
            get
            {
                return LastName + ", " + FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? "" :
                        (" " + (char?)MiddleName[0] + ".").ToUpper());
            }
        }

        [Display(Name = "Phone")]
        public string PhoneFormatted => "(" + ContactPhone?.Substring(0, 3) + ") "
            + ContactPhone?.Substring(3, 3) + "-" + ContactPhone?[6..];

        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name must contain only letters.")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Middle Name")]
        [StringLength(50, ErrorMessage = "Middle name cannot be more than 50 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Middle name must contain only letters.")]
        public string? MiddleName { get; set; } = "";

        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [Display(Name = "Last Name")]
        [MaxLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name must contain only letters.")]
        public string LastName { get; set; } = "";

        [Display(Name = "Title/Role")]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Title/Role must contain only letters.")]
        public string? ContactTitleRole { get; set; } = "";

        [Display(Name = "Phone")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        public string? ContactPhone { get; set; } = "";

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please follow the correct email format test@email.com")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string? ContactEmailAddress { get; set; } = "";

        [Display(Name = "Email Type")]
        public EmailType? ContactEmailType { get; set; }        

        [MaxLength(255)]
        [Display(Name = "Interactions")]
        public string? ContactInteractions { get; set; } = "";

        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? ContactNotes { get; set; } = "";

        [Display(Name = "Member")]
        public ICollection<MemberContact> MemberContacts { get; set; } = new HashSet<MemberContact>();

        [Display(Name = "Opportunities")]
        public ICollection<Opportunity> Opportunities { get; set; } = new HashSet<Opportunity>();
    }
}
