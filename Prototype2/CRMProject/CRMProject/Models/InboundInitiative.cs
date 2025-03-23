using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class InboundInitiative
    {
        public int ID { get; set; }

        [Display(Name = "Initiative")]
        [Required(ErrorMessage = "You cannot leave the initiative blank.")]
        [MaxLength(255, ErrorMessage = "Member Name cannot be more than 255 characters long.")]

        public string Initiative { get; set; } = "";

        [Display(Name = "Notes")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for note.")]
        [DataType(DataType.MultilineText)]
        public string? InboundInitiativeNotes { get; set; }
    }
}
