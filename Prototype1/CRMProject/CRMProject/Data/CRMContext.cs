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
        public DbSet<Cancellation> Cancellations { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactEmail> ContactEmails { get; set; }
        public DbSet<MemberContact> MemberContacts { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Prevent Cascade Delete from Contact to ContactEmail
            modelBuilder.Entity<ContactEmail>()
                .HasOne(ce => ce.Contact)
                .WithMany(c => c.ContactEmails)
                .HasForeignKey(ce => ce.ContactID)
                .OnDelete(DeleteBehavior.Restrict);

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

            //Prevent Cascade Delete from Member to MemberContact
            modelBuilder.Entity<MemberContact>()
                .HasOne(mc => mc.Member)
                .WithMany(m => m.MemberContacts)
                .HasForeignKey(mc => mc.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Contact to MemberContact
            modelBuilder.Entity<MemberContact>()
                .HasOne(mc => mc.Contact)
                .WithMany(c => c.MemberContacts)
                .HasForeignKey(mc => mc.ContactID)
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

            //Prevent Cascade Delete from Member to Cancellation
            modelBuilder.Entity<Cancellation>()
                .HasOne(c => c.Member)
                .WithMany(m => m.Cancellations)
                .HasForeignKey(c => c.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            //Unique Index for Contacs email
            modelBuilder.Entity<ContactEmail>()
                .HasIndex(ce => ce.EmailAddress)
                .IsUnique();

            //Unique Index for Members name
            modelBuilder.Entity<Member>()
                .HasIndex(m => new { m.MemberName, m.MemberSize, m.MemberStatus })
                .IsUnique();

            //Unique Index for Opportunity Name
            modelBuilder.Entity<Opportunity>()
                .HasIndex(o => o.OpportunityName)
                .IsUnique();

            //Many-to-Many MemberIndustry
            modelBuilder.Entity<MemberIndustry>()
                .HasKey(mi => new { mi.MemberID, mi.IndustryID });

            //Many-to-Many MemberMembershipType
            modelBuilder.Entity<MemberMembershipType>()
                .HasKey(mmt => new { mmt.MemberID, mmt.MembershipTypeID });

            //Unique Index for Industry Name
            modelBuilder.Entity<Industry>()
                .HasIndex(i => i.IndustryName)
                .IsUnique();

            //Unique Index for MembershipTypeName
            modelBuilder.Entity<MembershipType>()
                .HasIndex(mt => mt.MembershipTypeName)
                .IsUnique();

            //Unique Index for OpportunityAccount
            modelBuilder.Entity<Opportunity>()
                .HasIndex(o => o.OpportunityAccount)
                .IsUnique();
        }
    }
}
