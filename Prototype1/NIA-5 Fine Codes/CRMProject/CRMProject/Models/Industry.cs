using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Industry
    {
        public int ID { get; set; }

        [Display(Name = "Industry Name")]
        [Required(ErrorMessage = "You cannot leave the industry name blank.")]
        [MaxLength(50, ErrorMessage = "Industry name cannot be more than 50 characters long.")]
        public string MemberName { get; set; } = "";

        [Display(Name = "Industry NAICS Code")]
        [Required(ErrorMessage = "You cannot leave the Industry NAICS Code blank.")]
        [MaxLength(3, ErrorMessage = "Industry NAICS Code cannot be more than 3 characters long.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "Industry NAICS Code must consist of exactly 3 numeric digits.")]
        public string IndustryNAICSCode { get; set; } = "";

        [Display(Name = "Industry Description")]
        [MaxLength(255, ErrorMessage = "Limit of 255 characters for description.")]
        [DataType(DataType.MultilineText)]
        public string IndustryDescription { get; set; } = "";
    }
}
