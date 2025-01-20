using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Industry
    {
        public int ID { get; set; }

        [Display(Name = "Industry Name")]
        [Required(ErrorMessage = "You cannot leave the industry name blank.")]
        [MaxLength(255, ErrorMessage = "Industry name cannot be more than 255 characters long.")]
        public string IndustryName { get; set; } = "";

        [Display(Name = "Industry NAICS Code")]
        [MaxLength(6, ErrorMessage = "Industry NAICS Code cannot be more than 6 characters long.")]
        [RegularExpression(@"^\d{2,6}$", ErrorMessage = "Industry NAICS Code must consist of between 2 and 6 numeric digits.")]
        public string? IndustryNAICSCode { get; set; } = "";

        [Display(Name = "Industry Description")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for description.")]
        [DataType(DataType.MultilineText)]
        public string? IndustryDescription { get; set; } = "";

        [Display(Name = "Member")]
        public ICollection<MemberIndustry> MemberIndustries { get; set; } = new HashSet<MemberIndustry>();
    }
}
