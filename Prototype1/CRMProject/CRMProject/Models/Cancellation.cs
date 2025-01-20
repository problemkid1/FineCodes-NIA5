using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Cancellation
    {
        public int ID { get; set; }

        [Display(Name = "Cancellation Date")]
        [Required(ErrorMessage = "Cancellation date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CancellationDate { get; set; } = DateTime.Today.AddYears(-3);

        [Display(Name = "Cancellation Reason")]
        [Required(ErrorMessage = "You must enter a reason for the cancellation.")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for Reason.")]
        [DataType(DataType.MultilineText)]
        public string? CancellationReason { get; set; } = "";

        [Display(Name = "Cancellation Notes")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for note.")]
        [DataType(DataType.MultilineText)]
        public string? CancellationNotes { get; set; } = "";

        [Required(ErrorMessage = "You must select the Member.")]
        [Display(Name = "Member")]
        public int MemberID { get; set; }
        public Member? Member { get; set; }
    }
}
