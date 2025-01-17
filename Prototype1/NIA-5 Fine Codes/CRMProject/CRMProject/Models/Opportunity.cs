using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Opportunity
    {
        [Display(Name = "Opportunity Name")]
        [Required(ErrorMessage = "You cannot leave the opportunity name blank.")]
        [MaxLength(50, ErrorMessage = "Opportunity name cannot be more than 50 characters long.")]
        public string OpportunityName { get; set; } = "";

        [Required(ErrorMessage = "You must select the opportunity status.")]
        [Display(Name = "Opportunity Status")]
        public OpportunityStatus OpportunityStatus { get; set; }

        [Display(Name = "Priority")]
        [Required(ErrorMessage = "You cannot leave the Priority blank.")]
        [MaxLength(50, ErrorMessage = "Priority cannot be more than 50 characters long.")]
        public string Priority { get; set; } = "";

        [Display(Name = "Action")]
        [Required(ErrorMessage = "You cannot leave the action blank.")]
        [MaxLength(50, ErrorMessage = "Action cannot be more than 50 characters long.")]
        public string Action { get; set; } = "";

        [Display(Name = "Contact")]
        [Required(ErrorMessage = "You cannot leave the contact blank.")]
        [MaxLength(50, ErrorMessage = "Contact cannot be more than 50 characters long.")]
        public string Contact { get; set; } = "";

       [Display(Name = "Opportunity Account")]
        [Required(ErrorMessage = "You cannot leave the opportunity account blank.")]
        [MaxLength(50, ErrorMessage = "Opportunity account cannot be more than 50 characters long.")]
        public string OpportunityAccount { get; set; } = "";

        [Display(Name = "Last Contact Date")]
        [Required(ErrorMessage = "Last contact date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastContactDate { get; set; } = DateTime.Today;

        [Display(Name = "Opportunity Interaction")]
        [Required(ErrorMessage = "You cannot leave the opportunity interaction blank.")]
        [MaxLength(50, ErrorMessage = "Opportunity interaction cannot be more than 50 characters long.")]
        public string OpportunityInteraction { get; set; } = "";

    }
}
