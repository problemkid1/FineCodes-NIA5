using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Opportunity
    {
        public int ID { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "You cannot leave the name blank.")]
        [MaxLength(255, ErrorMessage = "Name cannot be more than 255 characters long.")]
        public string OpportunityName { get; set; } = "";

        [Required(ErrorMessage = "You must select the status.")]
        [Display(Name = "Status")]
        public OpportunityStatus OpportunityStatus { get; set; }

        [Display(Name = "Priority")]
        [MaxLength(50, ErrorMessage = "Priority cannot be more than 50 characters long.")]
        public string? OpportunityPriority { get; set; } = "";

        [Display(Name = "Action")]
        [MaxLength(255, ErrorMessage = "Action cannot be more than 255 characters long.")]
        public string? OpportunityAction { get; set; } = "";

        [Display(Name = "Last Contacted")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OpportunityLastContactDate { get; set; } = DateTime.Today;

        [Display(Name = "Interaction")]
        [MaxLength(255, ErrorMessage = "Interaction cannot be more than 255 characters long.")]
        public string? OpportunityInteractions { get; set; } = "";

        [Display(Name = "Contacts")]
        public ICollection<OpportunityContact>? OpportunityContacts { get; set; } = new HashSet<OpportunityContact>();
    }
}
