namespace CRMProject.Models
{
    public class MemberIndustry
    {
        public int MemberId { get; set; }
        public Member Member { get; set; }

        public int IndustryId { get; set; }
        public Industry Industry { get; set; }


    }
}
