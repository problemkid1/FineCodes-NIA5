using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Opportunity
    {
        public int ID { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "You cannot leave the name blank.")]
        [MaxLength(255, ErrorMessage = "Name cannot be more than 255 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters.")]
        public string OpportunityName { get; set; } = "";

        [Required(ErrorMessage = "You must select the status.")]
        [Display(Name = "Status")]
        public OpportunityStatus OpportunityStatus { get; set; }

        [Display(Name = "Priority")]
        [Required(ErrorMessage = "You cannot leave the Priority blank.")]
        [MaxLength(50, ErrorMessage = "Priority cannot be more than 50 characters long.")]
        public string OpportunityPriority { get; set; } = "";

        [Display(Name = "Action")]
        [Required(ErrorMessage = "You cannot leave the action blank.")]
        [MaxLength(255, ErrorMessage = "Action cannot be more than 255 characters long.")]
        public string OpportunityAction { get; set; } = "";

        [Display(Name = "Contact")]
        [Required(ErrorMessage = "You cannot leave the contact blank.")]
        [MaxLength(50, ErrorMessage = "Contact cannot be more than 50 characters long.")]
        public string OpportunityContact { get; set; } = "";

        [Display(Name = "Last Contacted")]
        [Required(ErrorMessage = "Last contact date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OpportunityLastContactDate { get; set; } = DateTime.Today;

        [Display(Name = "Interaction")]
        [Required(ErrorMessage = "You cannot leave the interaction blank.")]
        [MaxLength(255, ErrorMessage = "Interaction cannot be more than 255 characters long.")]
        public string OpportunityInteractions { get; set; } = "";

        public int? ContactID { get; set; }
        public Contact? Contact { get; set; }

    }
}
