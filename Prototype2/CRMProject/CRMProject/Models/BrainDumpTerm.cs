using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public enum BrainDumpTerm
    {
        [Display(Name = "Short Term")]
        ShortTerm,
        [Display(Name = "Long Term")]
        LongTerm
    }
}
