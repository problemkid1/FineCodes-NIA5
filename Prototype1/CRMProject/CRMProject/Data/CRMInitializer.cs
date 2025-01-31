using System.Diagnostics;
using CRMProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CRMProject.Data
{
    public static class CRMInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider,
            bool DeleteDatabase = false, bool UseMigrations = true, bool SeedSampleData = true)
        {
            using (var context = new CRMContext(
                serviceProvider.GetRequiredService<DbContextOptions<CRMContext>>()))
            {
                //Refresh the database as per the parameter options
                #region Prepare the Database
                try
                {
                    //Note: .CanConnect() will return false if the database is not there!
                    if (DeleteDatabase || !context.Database.CanConnect())
                    {
                        context.Database.EnsureDeleted(); //Delete the existing version 
                        if (UseMigrations)
                        {
                            context.Database.Migrate(); //Create the Database and apply all migrations
                        }
                        else
                        {
                            context.Database.EnsureCreated(); //Create and update the database as per the Model
                        }
                        //Now create any additional database objects such as Triggers or Views
                        //-----------------------------NONE YET---------------------------------------
                    }
                    else //The database is already created
                    {
                        if (UseMigrations)
                        {
                            context.Database.Migrate(); //Apply all migrations
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetBaseException().Message);
                }
                #endregion

                //Seed meaningless data as sample data during development
                #region Seed Sample Data 
                if (SeedSampleData)
                {
                    //To randomly generate data
                    Random random = new Random();

                    //Seed a few specific Members. We will add more with random values later.                   
                    try
                    {
                        // Seed Members if there aren't any.
                        if (!context.Members.Any())
                        {
                            context.Members.AddRange(
                                new Member
                                {
                                    MemberName = "Tech Solutions Inc.",
                                    MemberSize = 50,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "ap@techsolutions.com",
                                    MemberStartDate = DateTime.Parse("2021-01-15"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-30), // Last contacted a month ago
                                    MemberNotes = "Loyal member with regular participation."
                                },
                                new Member
                                {
                                    MemberName = "Green Energy Ltd.",
                                    MemberSize = 300,
                                    MemberStatus = MemberStatus.OverduePayment,
                                    MemberAccountsPayableEmail = "finance@greenenergy.com",
                                    MemberStartDate = DateTime.Parse("2020-06-10"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-60), // Last contacted 2 months ago
                                    MemberNotes = "Pending payment for the last quarter."
                                },
                                new Member
                                {
                                    MemberName = "Urban Builders",
                                    MemberSize = 70,
                                    MemberStatus = MemberStatus.Cancelled,
                                    MemberAccountsPayableEmail = "billing@urbanbuilders.com",
                                    MemberStartDate = DateTime.Parse("2018-03-20"),
                                    MemberEndDate = DateTime.Parse("2023-05-10"), // Canceled last year
                                    MemberLastContactDate = DateTime.Today.AddDays(-120), // Last contacted 4 months ago
                                    MemberNotes = "Canceled membership due to internal restructuring."
                                },
                                new Member
                                {
                                    MemberName = "Fresh Foods Co.",
                                    MemberSize = 120,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "payroll@freshfoods.com",
                                    MemberStartDate = DateTime.Parse("2019-11-05"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-15), // Recently contacted
                                    MemberNotes = "Key sponsor of annual events."
                                },
                                new Member
                                {
                                    MemberName = "Smart Solutions",
                                    MemberSize = 15,
                                    MemberStatus = MemberStatus.Cancelled,
                                    MemberAccountsPayableEmail = "accounting@smartsolutions.com",
                                    MemberStartDate = DateTime.Parse("2020-08-01"),
                                    MemberEndDate = DateTime.Parse("2022-12-15"), // Canceled 2 years ago
                                    MemberLastContactDate = DateTime.Today.AddDays(-300), // No contact for almost a year
                                    MemberNotes = "Membership expired, no renewal yet."
                                },
                                new Member
                                {
                                    MemberName = "EduTech",
                                    MemberSize = 25,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "finance@edutech.com",
                                    MemberStartDate = DateTime.Parse("2022-05-12"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-10), // Recently contacted
                                    MemberNotes = "Recently joined, active participation."
                                },
                                new Member
                                {
                                    MemberName = "AutoHub",
                                    MemberSize = 80,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "payments@autohub.com",
                                    MemberStartDate = DateTime.Parse("2021-09-30"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-45), // Contacted 1.5 months ago
                                    MemberNotes = "Active in automotive industry programs."
                                },
                                new Member
                                {
                                    MemberName = "Global Textiles",
                                    MemberSize = 2000,
                                    MemberStatus = MemberStatus.OverduePayment,
                                    MemberAccountsPayableEmail = "billing@globaltextiles.com",
                                    MemberStartDate = DateTime.Parse("2017-04-15"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-90), // Last contacted 3 months ago
                                    MemberNotes = "Awaiting payment confirmation."
                                },
                                new Member
                                {
                                    MemberName = "NextGen Technologies",
                                    MemberSize = 600,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "accounting@nextgen.com",
                                    MemberStartDate = DateTime.Parse("2019-07-22"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-20), // Contacted recently
                                    MemberNotes = "Frequent host of tech workshops."
                                },
                                new Member
                                {
                                    MemberName = "Prime Logistics",
                                    MemberSize = 100,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "finance@primelogistics.com",
                                    MemberStartDate = DateTime.Parse("2020-02-10"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-75), // Contacted 2.5 months ago
                                    MemberNotes = "Specializes in logistics management."
                                }
                            );

                            context.SaveChanges();

                            context.SaveChanges();
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.GetBaseException().Message);
                    }

                    // Seed Addresses if there aren't any.
                    if (!context.Addresses.Any())
                    {
                        // Get the array of Member primary keys
                        int[] memberIDs = context.Members.Select(a => a.ID).ToArray();
                        int memberCount = memberIDs.Length;

                        // Cities in the Niagara Region (and Ontario)
                        var cities = new[] { "St. Catharines", "Niagara Falls", "Welland", "Thorold", "Fort Erie", "Grimsby", "Port Colborne", "Beamsville" };

                        // Real street names in Niagara region
                        var streetNames = new[] {
                                                    "Queenston St", "Lakeshore Rd", "Mountain Rd", "Victoria Ave", "Glenridge Ave",
                                                    "King St", "Ontario St", "Ridge Rd", "Willowdale Ave", "Lundy's Lane"
                                                };

                        // Provinces (Ontario)
                        var provinces = new[] { "Ontario" };

                        // Create 10 random Address records, ensuring each member gets only one address
                        for (int i = 0; i < 10; i++)
                        {
                            // Select a member ID from the list
                            int randomMemberId = memberIDs[random.Next(memberCount)];

                            // Check if the member already has an address (skip if they do)
                            if (context.Addresses.Any(a => a.MemberID == randomMemberId))
                            {
                                continue; // Skip creating an address for this member
                            }

                            // Fixed Province for these records (Ontario)
                            Province randomProvince = Enum.Parse<Province>(provinces[0]);

                            // Select city from Niagara Region
                            string randomCity = cities[random.Next(cities.Length)];

                            // Select a random street name from the streetNames array
                            string randomStreetName = streetNames[random.Next(streetNames.Length)];

                            // Generate fixed AddressLine1 and AddressLine2 (AddressLine2 is optional)
                            string randomAddressLine1 = $"{randomStreetName} {random.Next(1, 100)}"; // e.g., "Queenston St 10"
                            string randomAddressLine2 = $"Suite {random.Next(1, 20)}"; // Optional Suite number

                            // Randomly generate PostalCode for Canadian format (e.g., A1A 1A1)
                            string randomPostalCode = $"{(char)random.Next(65, 91)}{random.Next(0, 10)}{(char)random.Next(65, 91)} {random.Next(0, 10)}{(char)random.Next(65, 91)}{random.Next(0, 10)}";

                            // Create the Address record
                            Address address = new Address
                            {
                                MemberID = randomMemberId,
                                AddressLine1 = randomAddressLine1,
                                AddressLine2 = randomAddressLine2,
                                AddressCity = randomCity,
                                Province = randomProvince,
                                PostalCode = randomPostalCode
                            };

                            // Add the new Address record to the context
                            context.Addresses.Add(address);
                        }

                        // Save changes to the database
                        context.SaveChanges();
                    }

                    // Seed MembershipTypes if there aren't any.
                    if (!context.MembershipTypes.Any())
                    {
                        context.MembershipTypes.AddRange(
                            new MembershipType
                            {
                                MembershipTypeName = MembershipTypeName.LocalIndustrial,
                                MembershipTypeDescription = "Local businesses in the industrial sector.",
                                MembershipTypeFee = 1000.00,
                                MembershipTypeBenefits = "Networking, Discounts, Free Events"
                            },
                            new MembershipType
                            {
                                MembershipTypeName = MembershipTypeName.NonLocalIndustrial,
                                MembershipTypeDescription = "Non-local industrial businesses.",
                                MembershipTypeFee = 1500.00,
                                MembershipTypeBenefits = "Enhanced Networking, Premium Events"
                            },
                            new MembershipType
                            {
                                MembershipTypeName = MembershipTypeName.InKind,
                                MembershipTypeDescription = "Non-monetary contributions.",
                                MembershipTypeFee = 0.00, // In-Kind is typically free
                                MembershipTypeBenefits = "Recognition, Networking"
                            },
                            new MembershipType
                            {
                                MembershipTypeName = MembershipTypeName.GovernmentAndEducation,
                                MembershipTypeDescription = "Government and educational institutions.",
                                MembershipTypeFee = 500.00,
                                MembershipTypeBenefits = "Discounts, Free Membership for First Year"
                            },
                            new MembershipType
                            {
                                MembershipTypeName = MembershipTypeName.Chamber,
                                MembershipTypeDescription = "Chamber of Commerce members.",
                                MembershipTypeFee = 250.00,
                                MembershipTypeBenefits = "Chamber access, Local Networking"
                            },
                            new MembershipType
                            {
                                MembershipTypeName = MembershipTypeName.Associate,
                                MembershipTypeDescription = "Associate membership for smaller organizations.",
                                MembershipTypeFee = 300.00,
                                MembershipTypeBenefits = "Basic Access"
                            }
                        );
                        context.SaveChanges();
                    }


                    // Seed MemberMembershipType relationships if there aren't any.
                    if (!context.MemberMembershipTypes.Any())
                    {
                        // Get the array of Member and MembershipType primary keys
                        int[] memberIDs = context.Members.Select(a => a.ID).ToArray();
                        int[] membershipTypeIDs = context.MembershipTypes.Select(a => a.ID).ToArray();

                        // Get the count of members and membership types
                        int memberCount = memberIDs.Length;
                        int membershipTypeCount = membershipTypeIDs.Length;

                        // Create 10 random MemberMembershipType relationships
                        for (int i = 0; i < memberIDs.Length; i++)
                        {
                            // Randomly select MemberId and MembershipTypeId
                            int memberID = memberIDs[i];
                            int randomMembershipTypeId = membershipTypeIDs[random.Next(membershipTypeCount)];

                            // Create new MemberMembershipType record
                            MemberMembershipType memberMembershipType = new MemberMembershipType
                            {
                                MemberID = memberID,
                                MembershipTypeID = randomMembershipTypeId
                            };

                            // Add the new relationship to the context
                            context.MemberMembershipTypes.Add(memberMembershipType);
                        }

                        // Save changes to the database
                        context.SaveChanges();
                    }

                    // Seed Industries if there aren't any.
                    if (!context.Industries.Any())
                    {
                        context.Industries.AddRange(
                            new Industry
                            {
                                IndustryName = "Technology",
                                IndustryNAICSCode = "541",
                                IndustryDescription = "Tech services and IT solutions."
                            },
                            new Industry
                            {
                                IndustryName = "Energy",
                                IndustryNAICSCode = "221",
                                IndustryDescription = "Renewable energy production and distribution."
                            },
                            new Industry
                            {
                                IndustryName = "Construction",
                                IndustryNAICSCode = "236",
                                IndustryDescription = "Commercial building construction."
                            },
                            new Industry
                            {
                                IndustryName = "Food",
                                IndustryNAICSCode = "311",
                                IndustryDescription = "Food production and distribution."
                            },
                            new Industry
                            {
                                IndustryName = "Education",
                                IndustryNAICSCode = "611",
                                IndustryDescription = "Educational services and resources."
                            },
                            new Industry
                            {
                                IndustryName = "Healthcare",
                                IndustryNAICSCode = "621",
                                IndustryDescription = "Offices of physicians, except mental health."
                            },
                            new Industry
                            {
                                IndustryName = "Manufacturing",
                                IndustryNAICSCode = "334",
                                IndustryDescription = "Electronic computer manufacturing."
                            },
                            new Industry
                            {
                                IndustryName = "Finance",
                                IndustryNAICSCode = "522",
                                IndustryDescription = "Commercial banking."
                            },
                            new Industry
                            {
                                IndustryName = "Retail",
                                IndustryNAICSCode = "441",
                                IndustryDescription = "New car dealers."
                            },
                            new Industry
                            {
                                IndustryName = "Transportation",
                                IndustryNAICSCode = "481",
                                IndustryDescription = "Scheduled air transportation."
                            }
                        );
                        context.SaveChanges();
                    }


                    // Seed MemberIndustry relationships if there aren't any.
                    if (!context.MemberIndustries.Any())
                    {
                        // Get the array of Member and Industry primary keys
                        int[] memberIDs = context.Members.Select(a => a.ID).ToArray();
                        int[] industryIDs = context.Industries.Select(a => a.ID).ToArray();

                        // Get the count of members and industries
                        int memberCount = memberIDs.Length;
                        int industryCount = industryIDs.Length;

                        // Create 10 random MemberIndustry relationships
                        for (int i = 0; i < memberIDs.Length; i++)
                        {
                            // Randomly select MemberId and IndustryId
                            int memberID = memberIDs[i];
                            int randomIndustryId = industryIDs[random.Next(industryCount)];

                            // Create new MemberIndustry record
                            MemberIndustry memberIndustry = new MemberIndustry
                            {
                                MemberID = memberID,
                                IndustryID = randomIndustryId
                            };

                            try
                            {
                                //Could be a duplicate 
                                context.MemberIndustries.Add(memberIndustry);
                                context.SaveChanges();
                            }
                            catch (Exception)
                            {
                                //so skip it and go on to the next
                                context.MemberIndustries.Remove(memberIndustry);
                            }
                        }
                    }

                    // Seed Cancellations if there aren't any.
                    if (!context.Cancellations.Any())
                    {
                        // Get the array of Member primary keys where the MemberStatus is Cancelled
                        int[] CancelledMemberIDs = context.Members
                                                          .Where(m => m.MemberStatus == MemberStatus.Cancelled)
                                                          .Select(m => m.ID)
                                                          .ToArray();
                        int CancelledMemberCount = CancelledMemberIDs.Length;

                        // Ensure that there are members with Cancelled status
                        if (CancelledMemberCount > 0)
                        {
                            // Create Cancellation records for each Cancelled member
                            for (int i = 0; i < CancelledMemberCount; i++)
                            {
                                // Select a random member from the canceled members list
                                int randomMemberId = CancelledMemberIDs[random.Next(CancelledMemberCount)];

                                // Create a new Cancellation record
                                Cancellation cancellation = new Cancellation
                                {
                                    MemberID = randomMemberId,
                                    CancellationDate = DateTime.Now.AddDays(-random.Next(30, 365)),  // Random cancellation within the last year
                                    CancellationReason = "Cancellation due to internal restructuring.",
                                    CancellationNotes = "No renewal interest."  // Custom cancellation reason
                                };
                                // Add the new cancellation record to the context
                                context.Cancellations.Add(cancellation);
                            }

                            // Save changes to the database
                            context.SaveChanges();
                        }
                        else
                        {
                            // Handle the case when there are no members with Canceled status
                            Debug.WriteLine("No members with Canceled status found.");
                        }
                    }

                    // Seed Contacts if there aren't any.
                    if (!context.Contacts.Any())
                    {
                        // Fetch available members
                        var memberIDs = context.Members.Select(m => m.ID).ToList();

                        if (memberIDs.Count == 0)
                        {
                            Debug.WriteLine("No members found. Contacts cannot be linked.");
                        }

                        // Define a list of contacts
                        var contacts = new List<Contact>
                        {
                            new Contact
                            {
                                FirstName = "John",
                                LastName = "Doe",
                                ContactTitleRole = "Account Manager",
                                ContactPhone = "1234567890",
                                ContactEmailAddress = "johndoe@example.com",
                                ContactEmailType = EmailType.VIP,
                                ContactWebsite = "https://johndoe.com",
                                ContactInteractions = "Met at conference, discussed potential partnership.",
                                ContactNotes = "Follow up in 2 weeks regarding partnership opportunities."
                            },
                            new Contact
                            {
                                FirstName = "Jane",
                                LastName = "Smith",
                                ContactTitleRole = "Sales Director",
                                ContactPhone = "0987654321",
                                ContactEmailAddress = "janesmith@example.com",
                                ContactEmailType = EmailType.VIP,
                                ContactWebsite = "https://janesmith.com",
                                ContactInteractions = "Phone call to discuss new product launch.",
                                ContactNotes = "Send product information via email."
                            },
                            new Contact
                            {
                                FirstName = "James",
                                LastName = "Brown",
                                ContactTitleRole = "Customer Support Lead",
                                ContactPhone = "1122334455",
                                ContactEmailAddress = "jamesbrown@example.com",
                                ContactEmailType = EmailType.VIP,
                                ContactWebsite = "https://jamesbrown.com",
                                ContactInteractions = "Helped resolve technical issue over email.",
                                ContactNotes = "Follow up in 1 week to ensure continued satisfaction."
                            },
                            new Contact
                            {
                                FirstName = "Mary",
                                LastName = "Johnson",
                                ContactTitleRole = "Marketing Manager",
                                ContactPhone = "2233445566",
                                ContactEmailAddress = "maryjohnson@example.com",
                                ContactEmailType = EmailType.VIP,
                                ContactWebsite = "https://maryjohnson.com",
                                ContactInteractions = "Met during marketing seminar, discussed brand strategy.",
                                ContactNotes = "Send case study on brand awareness campaign."
                            },
                            new Contact
                            {
                                FirstName = "Chris",
                                LastName = "Williams",
                                ContactTitleRole = "HR Manager",
                                ContactPhone = "3344556677",
                                ContactEmailAddress = "chriswilliams@example.com",
                                ContactEmailType = EmailType.VIP,
                                ContactWebsite = "https://chriswilliams.com",
                                ContactInteractions = "Interviewed for job opening, followed up on interview questions.",
                                ContactNotes = "Send hiring feedback."
                            },
                            new Contact
                            {
                                FirstName = "Patricia",
                                LastName = "Davis",
                                ContactTitleRole = "Product Specialist",
                                ContactPhone = "4455667788",
                                ContactEmailAddress = "patriciadavis@example.com",
                                ContactEmailType = EmailType.VIP,
                                ContactWebsite = "https://patriciadavis.com",
                                ContactInteractions = "Discussed product features and benefits with clients.",
                                ContactNotes = "Provide additional product training materials."
                            },
                            new Contact
                            {
                                FirstName = "Michael",
                                LastName = "Garcia",
                                ContactTitleRole = "Operations Manager",
                                ContactPhone = "5566778899",
                                ContactEmailAddress = "michaelgarcia@example.com",
                                ContactEmailType = EmailType.VIP,
                                ContactWebsite = "https://michaelgarcia.com",
                                ContactInteractions = "Coordinated project timelines and operational workflows.",
                                ContactNotes = "Follow up on project status next week."
                            },
                            new Contact
                            {
                                FirstName = "Sarah",
                                LastName = "Martinez",
                                ContactTitleRole = "Business Analyst",
                                ContactPhone = "6677889900",
                                ContactEmailAddress = "sarahmartinez@example.com",
                                ContactEmailType = EmailType.VIP,
                                ContactWebsite = "https://sarahmartinez.com",
                                ContactInteractions = "Worked on analysis for business growth strategies.",
                                ContactNotes = "Review report with senior team members."
                            },
                            new Contact
                            {
                                FirstName = "David",
                                LastName = "Lopez",
                                ContactTitleRole = "Financial Analyst",
                                ContactPhone = "7788990011",
                                ContactEmailAddress = "davidlopez@example.com",
                                ContactEmailType = EmailType.VIP,
                                ContactWebsite = "https://davidlopez.com",
                                ContactInteractions = "Reviewed financial reports and budgeting with team.",
                                ContactNotes = "Send updated financial forecast."
                            },
                            new Contact
                            {
                                FirstName = "Linda",
                                LastName = "Miller",
                                ContactTitleRole = "Legal Counsel",
                                ContactPhone = "8899001122",
                                ContactEmailAddress = "lindamiller@example.com",
                                ContactEmailType = EmailType.VIP,
                                ContactWebsite = "https://lindamiller.com",
                                ContactInteractions = "Provided legal advice on contract agreements.",
                                ContactNotes = "Send draft of contract revisions for review."
                            }
                        };

                        // Add contacts to the database
                        context.Contacts.AddRange(contacts);
                        context.SaveChanges();

                        // Now link contacts to members

                        foreach (var contact in contacts)
                        {
                            if (memberIDs.Count > 0) // Ensure members exist before assigning
                            {
                                var randomMemberId = memberIDs[random.Next(memberIDs.Count)];
                                context.MemberContacts.Add(new MemberContact
                                {
                                    MemberID = randomMemberId,
                                    ContactID = contact.ID,
                                    MemberContactRelationshipType = "Account Manager"
                                });
                            }
                        }

                        context.SaveChanges();
                    }

                    // Seed MemberContact relationships if there aren't any.
                    if (!context.MemberContacts.Any())
                    {
                        // Get the array of Member and Contact primary keys
                        int[] memberIDs = context.Members.Select(a => a.ID).ToArray();
                        int[] contactIDs = context.Contacts.Select(a => a.ID).ToArray();
                        int memberCount = memberIDs.Length;
                        int contactCount = contactIDs.Length;

                        // Relationship Types
                        var relationshipTypes = new[] { "Account Manager", "Support", "Partner", "Vendor" };

                        // Create 10 random MemberContact relationships
                        for (int i = 0; i < 10; i++)
                        {
                            // Randomly select MemberId and ContactId
                            int randomMemberId = memberIDs[random.Next(memberCount)];
                            int randomContactId = contactIDs[random.Next(contactCount)];

                            // Randomly select Relationship Type
                            string randomRelationshipType = relationshipTypes[random.Next(relationshipTypes.Length)];

                            // Create new MemberContact record
                            MemberContact memberContact = new MemberContact
                            {
                                MemberID = randomMemberId,
                                ContactID = randomContactId,
                                MemberContactRelationshipType = randomRelationshipType
                            };

                            try
                            {
                                // Add the new MemberContact record to the context
                                context.MemberContacts.Add(memberContact);
                                context.SaveChanges();
                            }
                            catch (Exception)
                            {
                                // Handle exceptions (e.g., skip if a conflict occurs)
                                context.MemberContacts.Remove(memberContact);
                            }
                        }
                    }

                    // Seed Opportunities if there aren't any.
                    if (!context.Opportunities.Any())
                    {
                        // List of real companies in Niagara Region, Canada
                        var niagaraCompanies = new[]
                        {
                            "Walker Industries",
                            "Niagara Casinos",
                            "Brock University",
                            "Niagara College",
                            "General Motors St. Catharines",
                            "Canadian Niagara Power",
                            "Niagara Falls Tourism",
                            "Silicon Knights",
                            "Stanpac",
                            "Rankin Construction"
                        };

                        // List of real-sounding contact names
                        var contactNames = new[]
                        {
                            "Michael Carter",
                            "Jennifer Adams",
                            "Robert Dawson",
                            "Emily Robinson",
                            "David Johnson",
                            "Sarah Mitchell",
                            "James Thompson",
                            "Lisa White",
                            "William Scott",
                            "Jessica Martin"
                        };

                        // Randomly select OpportunityStatus from the enum
                        var opportunityStatuses = Enum.GetValues(typeof(OpportunityStatus))
                                                      .Cast<OpportunityStatus>()
                                                      .ToArray();

                        // Priority Levels
                        var priorities = new[] { "High", "Medium", "Low" };

                        // Possible Opportunity Actions
                        var actions = new[]
                        {
                            "Initial Meeting Scheduled",
                            "Proposal Sent",
                            "Negotiation Ongoing",
                            "Contract Signed",
                            "Follow-up Required"
                        };

                        // Generate 10 real opportunity records
                        for (int i = 0; i < 10; i++)
                        {
                            var randomCompany = niagaraCompanies[i]; // Each company gets one record
                            var randomContact = contactNames[i]; // Each contact gets one record

                            Opportunity opportunity = new Opportunity
                            {
                                OpportunityName = randomCompany,
                                OpportunityStatus = opportunityStatuses[random.Next(opportunityStatuses.Length)],
                                OpportunityPriority = priorities[random.Next(priorities.Length)],
                                OpportunityAction = actions[random.Next(actions.Length)],
                                OpportunityContact = randomContact,
                                OpportunityLastContactDate = DateTime.Today.AddDays(-random.Next(1, 90)),
                                OpportunityInteractions = $"Discussed potential collaboration with {randomContact} from {randomCompany}. Next steps include {actions[random.Next(actions.Length)]}."
                            };

                            // Ensure no duplicate records before inserting
                            if (!context.Opportunities.Any(o => o.OpportunityName == opportunity.OpportunityName))
                            {
                                context.Opportunities.Add(opportunity);
                                context.SaveChanges();
                            }
                        }
                    }

                }
                #endregion
            }
        }
    }
}
