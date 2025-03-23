using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class AnnualActionItem
    {
        public int ID { get; set; }

        [Display(Name = "Action Item")]
        [Required(ErrorMessage = "Action Item is required.")]
        [MaxLength(255, ErrorMessage = "Action Item cannot be more than 255 characters long.")]
        public string ActionItem { get; set; }

        [Display(Name = "Assignee")]
        [Required(ErrorMessage = "Assignee is required.")]
        [MaxLength(100, ErrorMessage = "Assignee cannot be more than 100 characters long.")]
        public string Assignee { get; set; }

        [Display(Name = "Due Date")]
        [Required(ErrorMessage = "Due Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot be more than 50 characters long.")]
        public string Status { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        [MaxLength(500, ErrorMessage = "Notes cannot be more than 500 characters long.")]
        public string Notes { get; set; }
    }
}
