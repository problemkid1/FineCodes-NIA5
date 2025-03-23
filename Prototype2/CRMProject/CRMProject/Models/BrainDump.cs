using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class BrainDump
    {
        public int ID { get; set; }

        [Display(Name = "Activity")]
        [Required(ErrorMessage = "You cannot leave the activity blank.")]
        [MaxLength(255, ErrorMessage = "Member Name cannot be more than 255 characters long.")]

        public string Activity { get; set; } = "";

        [Display(Name = "Assignee")]
        [MaxLength(255, ErrorMessage = "Member Name cannot be more than 255 characters long.")]
        public string Assignee { get; set; } = "";

        [Display(Name = "Status")]
        public BrainDumpStatus? BrainDumpStatus { get; set; }

        [Display(Name = "Term")]
        public BrainDumpTerm? BrainDumpTerm { get; set; }

        [Display(Name = "Notes")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for note.")]
        [DataType(DataType.MultilineText)]
        public string? BrainDumpNotes { get; set; }
    }
}
