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
                                    MemberSize = 50,  // Using string to match the data type
                                    MemberStatus = MemberStatus.GoodStanding,  // Enum value
                                    MemberAccountsPayableEmail = "ap@techsolutions.com",
                                    MemberStartDate = DateTime.Parse("2021-01-15"),
                                    MemberEndDate = null,  // Assuming null is allowed for EndDate
                                    MemberNotes = "Loyal member with regular participation."
                                },
                                new Member
                                {
                                    MemberName = "Green Energy Ltd.",
                                    MemberSize = 300,
                                    MemberStatus = MemberStatus.OverduePayment,  // Enum value
                                    MemberAccountsPayableEmail = "finance@greenenergy.com",
                                    MemberStartDate = DateTime.Parse("2020-06-10"),
                                    MemberEndDate = DateTime.Parse("2023-12-31"),
                                    MemberNotes = "Pending payment for the last quarter."
                                },
                                new Member
                                {
                                    MemberName = "Urban Builders",
                                    MemberSize = 70,
                                    MemberStatus = MemberStatus.Canceled,  // Enum value
                                    MemberAccountsPayableEmail = "billing@urbanbuilders.com",
                                    MemberStartDate = DateTime.Parse("2018-03-20"),
                                    MemberEndDate = DateTime.Parse("2023-01-10"),
                                    MemberNotes = "Canceled membership due to internal restructuring."
                                },
                                new Member
                                {
                                    MemberName = "Fresh Foods Co.",
                                    MemberSize = 120,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "payroll@freshfoods.com",
                                    MemberStartDate = DateTime.Parse("2019-11-05"),
                                    MemberEndDate = DateTime.Parse("2023-01-31"),
                                    MemberNotes = "Key sponsor of annual events."
                                },
                                new Member
                                {
                                    MemberName = "Smart Solutions",
                                    MemberSize = 15,
                                    MemberStatus = MemberStatus.Canceled,  // Enum value
                                    MemberAccountsPayableEmail = "accounting@smartsolutions.com",
                                    MemberStartDate = DateTime.Parse("2020-08-01"),
                                    MemberEndDate = DateTime.Parse("2022-08-01"),
                                    MemberNotes = "Membership expired, no renewal yet."
                                },
                                new Member
                                {
                                    MemberName = "EduTech",
                                    MemberSize = 25,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "finance@edutech.com",
                                    MemberStartDate = DateTime.Parse("2022-05-12"),
                                    MemberEndDate = DateTime.Parse("2027-12-31"),
                                    MemberNotes = "Recently joined, active participation."
                                },
                                new Member
                                {
                                    MemberName = "AutoHub",
                                    MemberSize = 80,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "payments@autohub.com",
                                    MemberStartDate = DateTime.Parse("2021-09-30"),
                                    MemberEndDate = DateTime.Parse("2029-12-31"),
                                    MemberNotes = "Active in automotive industry programs."
                                },
                                new Member
                                {
                                    MemberName = "Global Textiles",
                                    MemberSize = 2000,
                                    MemberStatus = MemberStatus.OverduePayment,
                                    MemberAccountsPayableEmail = "billing@globaltextiles.com",
                                    MemberStartDate = DateTime.Parse("2017-04-15"),
                                    MemberEndDate = DateTime.Parse("2023-11-30"),
                                    MemberNotes = "Awaiting payment confirmation."
                                },
                                new Member
                                {
                                    MemberName = "NextGen Technologies",
                                    MemberSize = 600,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "accounting@nextgen.com",
                                    MemberStartDate = DateTime.Parse("2019-07-22"),
                                    MemberEndDate = DateTime.Parse("2023-12-01"),
                                    MemberNotes = "Frequent host of tech workshops."
                                },
                                new Member
                                {
                                    MemberName = "Prime Logistics",
                                    MemberSize = 100,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "finance@primelogistics.com",
                                    MemberStartDate = DateTime.Parse("2020-02-10"),
                                    MemberEndDate = DateTime.Parse("2023-12-04"),
                                    MemberNotes = "Specializes in logistics management."
                                }
                            );
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

                        // Address Types
                        var addressTypes = new[] { "Headquarters", "Branch", "Warehouse" };

                        // Cities in the Niagara Region (and Ontario)
                        var cities = new[] { "St. Catharines", "Niagara Falls", "Welland", "Thorold", "Fort Erie", "Grimsby", "Port Colborne", "Beamsville" };

                        // Real street names in Niagara region
                        var streetNames = new[]
                        {
                            "Queenston St", "Lakeshore Rd", "Mountain Rd", "Victoria Ave", "Glenridge Ave",
                            "King St", "Ontario St", "Ridge Rd", "Willowdale Ave", "Lundy's Lane"
                        };

                        // Provinces (Ontario)
                        var provinces = new[] { "Ontario" };

                        // Create 10 random Address records using Niagara Region cities and Ontario province
                        for (int i = 0; i < 10; i++)
                        {
                            int randomMemberId = memberIDs[random.Next(memberIDs.Length)];

                            // Fixed AddressType and Province for these records
                            AddressType randomAddressType = Enum.Parse<AddressType>(addressTypes[random.Next(addressTypes.Length)]);
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
                                PostalCode = randomPostalCode,
                                AddressType = randomAddressType
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
                        for (int i = 0; i < 10; i++)
                        {
                            // Randomly select MemberId and IndustryId
                            int randomMemberId = memberIDs[random.Next(memberCount)];
                            int randomIndustryId = industryIDs[random.Next(industryCount)];

                            // Create new MemberIndustry record
                            MemberIndustry memberIndustry = new MemberIndustry
                            {
                                MemberID = randomMemberId,
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
                        // Get the array of Member primary keys
                        int[] memberIDs = context.Members.Select(a => a.ID).ToArray();
                        int memberCount = memberIDs.Length;

                        // Create 10 random Cancellation records
                        for (int i = 0; i < 10; i++)
                        {
                            // Randomly select MemberId
                            int randomMemberId = memberIDs[random.Next(memberCount)];

                            // Create a new Cancellation record
                            Cancellation cancellation = new Cancellation
                            {
                                MemberID = randomMemberId,
                                CancellationDate = DateTime.Now.AddDays(-random.Next(30, 365)),  // Random cancellation within the last year
                                CancellationReason = $"Reason {random.Next(1, 5)}",  // Random cancellation reason
                                CancellationNotes = "Cancellation due to internal restructuring or lack of renewal."
                            };

                            // Add the new cancellation record to the context
                            context.Cancellations.Add(cancellation);
                        }

                        // Save changes to the database
                        context.SaveChanges();
                    }

                    // Seed Contacts if there aren't any.
                    if (!context.Contacts.Any())
                    {
                        context.Contacts.AddRange(
                            new Contact
                            {
                                FirstName = "John",
                                LastName = "Doe",
                                ContactTitleRole = "Account Manager",
                                ContactPhone = "1234567890",
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
                                ContactWebsite = "https://lindamiller.com",
                                ContactInteractions = "Provided legal advice on contract agreements.",
                                ContactNotes = "Send draft of contract revisions for review."
                            }
                        );
                        context.SaveChanges();
                    }

                    // Seed ContactEmails if there aren't any.
                    if (!context.ContactEmails.Any())
                    {
                        // Get the array of Contact primary keys
                        int[] contactIDs = context.Contacts.Select(a => a.ID).ToArray();
                        int contactCount = contactIDs.Length;

                        // Create 10 random ContactEmail records
                        for (int i = 0; i < 10; i++)
                        {
                            // Randomly select ContactId
                            int randomContactId = contactIDs[random.Next(contactCount)];

                            // Randomly select EmailType
                            var emailTypes = Enum.GetValues(typeof(EmailType)).Cast<EmailType>().ToArray();
                            EmailType randomEmailType = emailTypes[random.Next(emailTypes.Length)];

                            // Create a new ContactEmail record
                            ContactEmail contactEmail = new ContactEmail
                            {
                                ContactID = randomContactId,
                                EmailType = randomEmailType,
                                EmailAddress = $"{Guid.NewGuid().ToString().Substring(0, 8)}@example.com"  // Generate a random email address
                            };

                            // Add the new contact email record to the context
                            context.ContactEmails.Add(contactEmail);
                        }

                        // Save changes to the database
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
                        // Create 10 Opportunity records
                        for (int i = 0; i < 10; i++)
                        {
                            // Randomly select OpportunityStatus from the enum
                            var opportunityStatuses = Enum.GetValues(typeof(OpportunityStatus))
                                                          .Cast<OpportunityStatus>()
                                                          .ToArray();
                            OpportunityStatus randomOpportunityStatus = opportunityStatuses[random.Next(opportunityStatuses.Length)];

                            // Randomly select Priority
                            var priorities = new[] { "High", "Medium", "Low" };
                            string randomPriority = priorities[random.Next(priorities.Length)];

                            // Randomly select OpportunityAction
                            var actions = new[] { "Follow-up", "Proposal Sent", "Negotiation Ongoing", "Review Meeting Scheduled" };
                            string randomAction = actions[random.Next(actions.Length)];

                            // Randomly select Contact and Account names
                            string randomContact = $"Contact {random.Next(1, 100)}";
                            string randomAccount = $"Account {random.Next(1, 100)}";

                            // Generate a random last contact date within the last 90 days
                            DateTime randomLastContactDate = DateTime.Today.AddDays(-random.Next(1, 90));

                            // Generate a random interaction text
                            string randomInteractions = $"Discussed opportunity with {randomContact}, next steps defined.";

                            // Create a new Opportunity record
                            Opportunity opportunity = new Opportunity
                            {
                                OpportunityName = $"Opportunity {i + 1}",
                                OpportunityStatus = randomOpportunityStatus,  // Use the enum value
                                OpportunityPriority = randomPriority,
                                OpportunityAction = randomAction,
                                OpportunityContact = randomContact,
                                OpportunityAccount = randomAccount,
                                OpportunityLastContactDate = randomLastContactDate,
                                OpportunityInteractions = randomInteractions
                            };

                            try
                            {
                                // Could be a duplicate, so check and add opportunity
                                context.Opportunities.Add(opportunity);
                                context.SaveChanges();
                            }
                            catch (Exception)
                            {
                                // If duplicate occurs, skip this record and continue with the next one
                                context.Opportunities.Remove(opportunity);
                            }
                        }
                    }
                }
                #endregion
            }
        }
    }
}
