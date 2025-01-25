using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public enum OpportunityStatus
    {
        Qualification,
        Negotiating,
        [Display(Name = "Closed New Member")]
        ClosedNewMember,
        [Display(Name = "Closed Not Interested")]
        ClosedNotInterested
    }

}
