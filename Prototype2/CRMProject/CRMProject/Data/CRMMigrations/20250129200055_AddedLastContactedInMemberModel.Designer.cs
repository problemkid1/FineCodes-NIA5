﻿// <auto-generated />
using System;
using CRMProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    [DbContext(typeof(CRMContext))]
    [Migration("20250129200055_AddedLastContactedInMemberModel")]
    partial class AddedLastContactedInMemberModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("CRMProject.Models.Address", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AddressCity")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("AddressLine1")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("AddressLine2")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("AddressType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MemberID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PostalCode")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Province")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("MemberID");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("CRMProject.Models.Cancellation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CancellationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("CancellationNotes")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("CancellationReason")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("MemberID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("MemberID");

                    b.ToTable("Cancellations");
                });

            modelBuilder.Entity("CRMProject.Models.Contact", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ContactInteractions")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactNotes")
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactPhone")
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactTitleRole")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactWebsite")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("CRMProject.Models.ContactEmail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ContactID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("EmailType")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("ContactID");

                    b.HasIndex("EmailAddress")
                        .IsUnique();

                    b.ToTable("ContactEmails");
                });

            modelBuilder.Entity("CRMProject.Models.Industry", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("IndustryDescription")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("IndustryNAICSCode")
                        .HasMaxLength(6)
                        .HasColumnType("TEXT");

                    b.Property<string>("IndustryName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("IndustryNAICSCode")
                        .IsUnique();

                    b.HasIndex("IndustryName")
                        .IsUnique();

                    b.ToTable("Industries");
                });

            modelBuilder.Entity("CRMProject.Models.Member", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("MemberAccountsPayableEmail")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("MemberEndDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("MemberLastContactDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("MemberName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("MemberNotes")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("MemberSize")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("MemberStartDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("MemberStatus")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("MemberName")
                        .IsUnique();

                    b.ToTable("Members");
                });

            modelBuilder.Entity("CRMProject.Models.MemberContact", b =>
                {
                    b.Property<int>("MemberID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ContactID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MemberContactRelationshipType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("MemberID", "ContactID");

                    b.HasIndex("ContactID");

                    b.ToTable("MemberContacts");
                });

            modelBuilder.Entity("CRMProject.Models.MemberIndustry", b =>
                {
                    b.Property<int>("MemberID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IndustryID")
                        .HasColumnType("INTEGER");

                    b.HasKey("MemberID", "IndustryID");

                    b.HasIndex("IndustryID");

                    b.ToTable("MemberIndustries");
                });

            modelBuilder.Entity("CRMProject.Models.MemberMembershipType", b =>
                {
                    b.Property<int>("MemberID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MembershipTypeID")
                        .HasColumnType("INTEGER");

                    b.HasKey("MemberID", "MembershipTypeID");

                    b.HasIndex("MembershipTypeID");

                    b.ToTable("MemberMembershipTypes");
                });

            modelBuilder.Entity("CRMProject.Models.MembershipType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("MembershipTypeBenefits")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("MembershipTypeDescription")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<double?>("MembershipTypeFee")
                        .HasColumnType("REAL");

                    b.Property<int>("MembershipTypeName")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("MembershipTypeName")
                        .IsUnique();

                    b.ToTable("MembershipTypes");
                });

            modelBuilder.Entity("CRMProject.Models.Opportunity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("OpportunityAccount")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("OpportunityAction")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("OpportunityContact")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("OpportunityInteractions")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("OpportunityLastContactDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("OpportunityName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("OpportunityPriority")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("OpportunityStatus")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("OpportunityAccount")
                        .IsUnique();

                    b.HasIndex("OpportunityName")
                        .IsUnique();

                    b.ToTable("Opportunities");
                });

            modelBuilder.Entity("CRMProject.Models.Address", b =>
                {
                    b.HasOne("CRMProject.Models.Member", "Member")
                        .WithMany("Addresses")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");
                });

            modelBuilder.Entity("CRMProject.Models.Cancellation", b =>
                {
                    b.HasOne("CRMProject.Models.Member", "Member")
                        .WithMany("Cancellations")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Member");
                });

            modelBuilder.Entity("CRMProject.Models.ContactEmail", b =>
                {
                    b.HasOne("CRMProject.Models.Contact", "Contact")
                        .WithMany("ContactEmails")
                        .HasForeignKey("ContactID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contact");
                });

            modelBuilder.Entity("CRMProject.Models.MemberContact", b =>
                {
                    b.HasOne("CRMProject.Models.Contact", "Contact")
                        .WithMany("MemberContacts")
                        .HasForeignKey("ContactID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CRMProject.Models.Member", "Member")
                        .WithMany("MemberContacts")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Contact");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("CRMProject.Models.MemberIndustry", b =>
                {
                    b.HasOne("CRMProject.Models.Industry", "Industry")
                        .WithMany("MemberIndustries")
                        .HasForeignKey("IndustryID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CRMProject.Models.Member", "Member")
                        .WithMany("MemberIndustries")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Industry");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("CRMProject.Models.MemberMembershipType", b =>
                {
                    b.HasOne("CRMProject.Models.Member", "Member")
                        .WithMany("MemberMembershipTypes")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CRMProject.Models.MembershipType", "MembershipType")
                        .WithMany("MemberMembershipTypes")
                        .HasForeignKey("MembershipTypeID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Member");

                    b.Navigation("MembershipType");
                });

            modelBuilder.Entity("CRMProject.Models.Contact", b =>
                {
                    b.Navigation("ContactEmails");

                    b.Navigation("MemberContacts");
                });

            modelBuilder.Entity("CRMProject.Models.Industry", b =>
                {
                    b.Navigation("MemberIndustries");
                });

            modelBuilder.Entity("CRMProject.Models.Member", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Cancellations");

                    b.Navigation("MemberContacts");

                    b.Navigation("MemberIndustries");

                    b.Navigation("MemberMembershipTypes");
                });

            modelBuilder.Entity("CRMProject.Models.MembershipType", b =>
                {
                    b.Navigation("MemberMembershipTypes");
                });
#pragma warning restore 612, 618
        }
    }
}
