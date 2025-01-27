using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public enum Province
    {
        Alberta,
        [Display(Name = "British Columbia")]
        BritishColumbia,
        Manitoba,
        [Display(Name = "New Brunswick")]
        NewBrunswick,
        [Display(Name = "Newfoundland and Labrador")]
        NewfoundlandAndLabrador,
        [Display(Name = "Nova Scotia")]
        NovaScotia,
        Ontario,
        [Display(Name = "Prince Edward Island")]
        PrinceEdwardIsland,
        Quebec,
        Saskatchewan
    }
}
