using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class StatusHistory
    {
        public int ID { get; set; }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Status")]
        [Required(ErrorMessage = "You cannot leave the status blank.")]
        [MaxLength(255, ErrorMessage = "Status cannot be more than 255 characters long.")]
        public string Status { get; set; } = "";

        [Display(Name = "Reason")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for Reason.")]
        [DataType(DataType.MultilineText)]
        public string? Reason { get; set; } = "";

        [Display(Name = "Notes")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for note.")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; } = "";

        [Required(ErrorMessage = "You must select the Member.")]
        [Display(Name = "Member")]
        public int MemberID { get; set; }
        public Member? Member { get; set; }
    }
}
