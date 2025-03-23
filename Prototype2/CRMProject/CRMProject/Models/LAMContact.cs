using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class LAMContact
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Municipality is required.")]
        [Display(Name = "Municipality")]
        [MaxLength(100, ErrorMessage = "Municipality cannot be more than 100 characters long.")]
        public string Municipality { get; set; } = "";

        [Required(ErrorMessage = "Position is required.")]
        [Display(Name = "Position")]
        [MaxLength(100, ErrorMessage = "Position cannot be more than 100 characters long.")]
        public string Position { get; set; } = "";

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        [MaxLength(500, ErrorMessage = "Notes cannot be more than 500 characters long.")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "You must select a contact.")]
        [Display(Name = "Contact")]
        public int ContactID { get; set; }

        public Contact? Contact { get; set; }
    }
}