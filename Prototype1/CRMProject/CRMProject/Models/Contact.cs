using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Contact
    {
        public int ContactId { get; set; }

        [Required(ErrorMessage = "You cannot leave the first name type blank.")]
        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You cannot leave the last name type blank.")]
        [Display(Name = "Last Name")]
        [MaxLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; }
                
        [Display(Name = "Title/Role")]
        [MaxLength(100)]
        public string ContactTitleRole { get; set; }

        [Display(Name = "Phone")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        public string ContactPhone { get; set; }
                
        [Display(Name = "Website")]
        [MaxLength(100)]
        public string ContactWebsite { get; set; }

        [MaxLength(255)]
        [Display(Name = "Interactions")]
        public string ContactInteractions { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string ContactNotes { get; set; }

        
        public ICollection<ContactEmail> ContactEmails { get; set; }
        
        public ICollection<MemberContact> MemberContacts { get; set; }
    }
}
