namespace CRMProject.Models
{
    public class OpportunityContact
    {
        public int OpportunityID { get; set; }
        public Opportunity? Opportunity { get; set; }

        public int ContactID { get; set; }
        public Contact? Contact { get; set; }
    }
}
