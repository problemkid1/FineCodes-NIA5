using CRMProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CRMProject.Data
{
    public class CRMContext : DbContext
    {
        public CRMContext(DbContextOptions<CRMContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<MemberIndustry> MemberIndustries { get; set; }
        public DbSet<MembershipType> MembershipTypes { get; set; }
        public DbSet<MemberMembershipType> MemberMembershipTypes { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<MemberContact> MemberContacts { get; set; }
        public DbSet<OpportunityContact> OpportunityContacts { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<MemberPhoto> MemberPhotos { get; set; }
        public DbSet<MemberThumbnail> MemberThumbnails { get; set; }
        public DbSet<MemberLogin> MemberLogins { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<AnnualActionItem> AnnualActionItems { get; set; }
        public DbSet<LAMContact> LAMContacts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Prevent Cascade Delete from Industry to MemberIndustry
            modelBuilder.Entity<MemberIndustry>()
                .HasOne(mi => mi.Industry)
                .WithMany(i => i.MemberIndustries)
                .HasForeignKey(mi => mi.IndustryID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Member to MemberIndustry
            modelBuilder.Entity<MemberIndustry>()
                .HasOne(mi => mi.Member)
                .WithMany(m => m.MemberIndustries)
                .HasForeignKey(mi => mi.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from MembershipType to MemberMembershipType
            modelBuilder.Entity<MemberMembershipType>()
                .HasOne(mmt => mmt.MembershipType)
                .WithMany(mt => mt.MemberMembershipTypes)
                .HasForeignKey(mmt => mmt.MembershipTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Member to MemberMembershipType
            modelBuilder.Entity<MemberMembershipType>()
                .HasOne(mmt => mmt.Member)
                .WithMany(m => m.MemberMembershipTypes)
                .HasForeignKey(mmt => mmt.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Member to StatusHistory
            modelBuilder.Entity<StatusHistory>()
                .HasOne(c => c.Member)
                .WithMany(m => m.StatusHistories)
                .HasForeignKey(c => c.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            // One Contact has many Opportunity
            //modelBuilder.Entity<Opportunity>()
            //    .HasOne(c => c.Contact)
            //    .WithMany(o => o.Opportunities)
            //    .HasForeignKey(c => c.ContactID)
            //    .OnDelete(DeleteBehavior.Restrict);

            //Unique Index for Contact email
            modelBuilder.Entity<Contact>()
                .HasIndex(ce => ce.ContactEmailAddress)
                .IsUnique();

            //Unique Index for Members name
            modelBuilder.Entity<Member>()
                .HasIndex(m => m.MemberName)
                .IsUnique();

            //Add a unique index to the Employee Email
            modelBuilder.Entity<MemberLogin>()
            .HasIndex(a => new { a.Email })
            .IsUnique();

            //Unique Index for Members Member Accounts Payable Email
            modelBuilder.Entity<Member>()
            .HasIndex(m => m.MemberAccountsPayableEmail)
            .IsUnique();

            //Unique Index for Opportunity Name
            modelBuilder.Entity<Opportunity>()
                .HasIndex(o => o.OpportunityName)
                .IsUnique();

            //Unique Index for IndustryNAICSCode
            modelBuilder.Entity<Industry>()
                .HasIndex(i => i.IndustryNAICSCode)
                .IsUnique();

            //Unique Index for MembershipTypeName
            modelBuilder.Entity<MembershipType>()
                .HasIndex(mt => mt.MembershipTypeName)
                .IsUnique();

            //Many-to-Many MemberIndustry
            modelBuilder.Entity<MemberIndustry>()
                .HasKey(mi => new { mi.MemberID, mi.IndustryID });

            //Many-to-Many MemberMembershipType
            modelBuilder.Entity<MemberMembershipType>()
                .HasKey(mmt => new { mmt.MemberID, mmt.MembershipTypeID });

            //Many to many MemberContact
            modelBuilder.Entity<MemberContact>()
            .HasKey(mc => new { mc.MemberID, mc.ContactID });

            //Many to many OpportunityContact
            modelBuilder.Entity<OpportunityContact>()
            .HasKey(mc => new { mc.OpportunityID, mc.ContactID });

            // Composite Unique Index on AddressLine1, AddressCity, AddressProvince, and AddressPostalCode
            modelBuilder.Entity<Address>()
                .HasIndex(a => new { a.AddressLine1, a.AddressCity, a.Province, a.PostalCode })
                .IsUnique();
        }
    }
}
