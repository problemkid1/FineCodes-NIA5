using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Industry
    {
        public int ID { get; set; }

        public string Summary
        {
            get
            {

                return IndustryNAICSCode + " - " + 
               IndustrySector + ": " + 
               IndustrySubsector;

            }
        }


        [Display(Name = "Industry Sector")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for description.")]
        [DataType(DataType.MultilineText)]
        public string? IndustrySector { get; set; }

        [Display(Name = "Industry Subsector")]
        [Required(ErrorMessage = "You cannot leave the industry name blank.")]
        [MaxLength(255, ErrorMessage = "Industry name cannot be more than 255 characters long.")]
        public string IndustrySubsector { get; set; } = "";

        [Display(Name = "NAICS Code")]
        [MaxLength(6, ErrorMessage = "Industry Subsector NAICS Code cannot be more than 3 characters long.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "Industry Subsector NAICS Code must consist of 3 numeric digits.")]
        public string? IndustryNAICSCode { get; set; } = "";

        [Display(Name = "Member")]
        public ICollection<MemberIndustry> MemberIndustries { get; set; } = new HashSet<MemberIndustry>();
    }
}
