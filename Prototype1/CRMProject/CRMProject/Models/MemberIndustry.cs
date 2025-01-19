namespace CRMProject.Models
{
    public class MemberIndustry
    {
        public int MemberID { get; set; }
        public Member? Member { get; set; }

        public int IndustryID { get; set; }
        public Industry? Industry { get; set; }

    }
}
