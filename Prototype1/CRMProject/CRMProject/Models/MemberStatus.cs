using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public enum MemberStatus
    {
        [Display(Name = "Good Standing")]
        GoodStanding,
        [Display(Name = "Overdue Payment")]
        OverduePayment,
        Canceled
    }

}
