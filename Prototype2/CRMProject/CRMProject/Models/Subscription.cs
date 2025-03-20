using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        [StringLength(512)]
        public string PushEndpoint { get; set; } = "";

        [StringLength(512)]
        public string PushP256DH { get; set; } = "";

        [StringLength(512)]
        public string PushAuth { get; set; } = "";

        [Required(ErrorMessage = "You must select the Member.")]
        [Display(Name = "Member")]
        public int MemberLoginID { get; set; }
        public MemberLogin? MemberLogin { get; set; }
    }
}
