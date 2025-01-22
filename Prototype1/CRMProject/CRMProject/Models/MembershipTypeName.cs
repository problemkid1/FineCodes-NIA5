using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public enum MembershipTypeName
    {
        [Display(Name = "Local Industrial")]
        LocalIndustrial,
        [Display(Name = "Non-Local Industrial")]
        NonLocalIndustrial,
        [Display(Name = "In Kind")]
        InKind,
        [Display(Name = "Government And Education")]
        GovernmentAndEducation,
        Chamber,
        Associate
    }

}
