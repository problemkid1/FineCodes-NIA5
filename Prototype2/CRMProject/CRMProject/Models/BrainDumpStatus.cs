using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public enum BrainDumpStatus
    {
        [Display(Name = "Done")]
        Done,
        [Display(Name = "In Progress")]
        InProgress,
        [Display(Name = "To Do")]
        ToDo
    }
}
