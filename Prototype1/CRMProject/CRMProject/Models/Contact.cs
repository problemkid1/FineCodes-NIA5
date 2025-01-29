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

        [Required(ErrorMessage = "You cannot leave the first name type blank.")]
        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Middle Name")]
        [StringLength(50, ErrorMessage = "Middle name cannot be more than 50 characters long.")]
        public string? MiddleName { get; set; } = "";

        [Required(ErrorMessage = "You cannot leave the last name type blank.")]
        [Display(Name = "Last Name")]
        [MaxLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; } = "";

        [Display(Name = "Title/Role")]
        [MaxLength(100)]
        public string? ContactTitleRole { get; set; } = "";

        [Display(Name = "Phone")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        public string? ContactPhone { get; set; } = "";

        [Display(Name = "Website")]
        [MaxLength(100)]
        public string? ContactWebsite { get; set; } = "";

        [MaxLength(255)]
        [Display(Name = "Interactions")]
        public string? ContactInteractions { get; set; } = "";

        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? ContactNotes { get; set; } = "";

        [Display(Name = "Member")]
        public ICollection<MemberContact> MemberContacts { get; set; } = new HashSet<MemberContact>();

        [Display(Name = "Contact Email")]
        public ICollection<ContactEmail> ContactEmails { get; set; } = new HashSet<ContactEmail>();
    }
}
