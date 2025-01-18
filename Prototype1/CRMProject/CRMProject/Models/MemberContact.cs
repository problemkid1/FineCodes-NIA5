using System.ComponentModel.DataAnnotations;

namespace CRMProject.Models
{
    public class MemberContact
    {
        public int MemberId { get; set; }
        public Member Member { get; set; }

        public int ContactId { get; set; }
        public Contact Contact { get; set; }    

        [MaxLength(100)]
        [Display(Name = "Relationship Type")]
        public string MemberContactRelationshipType { get; set; }
    }
}
