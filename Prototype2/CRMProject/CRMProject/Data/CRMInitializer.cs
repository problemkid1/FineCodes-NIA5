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

                    //Seed a few specific Members. We will add more later.                   
                    try
                    {
                        // Seed Members if there aren't any.
                        if (!context.Members.Any())
                        {
                            context.Members.AddRange(
                                new Member
                                {
                                    MemberName = "100 Marketing",
                                    MemberSize = 50,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "info@180marketing.com",
                                    MemberWebsite = "https://100Marketing.com",
                                    MemberStartDate = DateTime.Parse("2021-01-15"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-30), // Last contacted a month ago
                                    MemberNotes = "Full-service marketing firm."
                                },
                                new Member
                                {
                                    MemberName = "Technologies co.",
                                    MemberSize = 300,
                                    MemberStatus = MemberStatus.OverduePayment,
                                    MemberAccountsPayableEmail = "finance@greenenergy.com",
                                    MemberWebsite = "https://Technologiesco.com",
                                    MemberStartDate = DateTime.Parse("2020-06-10"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-60), // Last contacted 2 months ago
                                    MemberNotes = "Specializes in laser cutting and turret punching."
                                },
                                new Member
                                {
                                    MemberName = "Chain Management",
                                    MemberSize = 70,
                                    MemberStatus = MemberStatus.Cancelled,
                                    MemberAccountsPayableEmail = "billing@urbanbuilders.com",
                                    MemberWebsite = "https://ChainManagement.com",
                                    MemberStartDate = DateTime.Parse("2018-03-20"),
                                    MemberEndDate = DateTime.Parse("2023-05-10"), // Canceled last year
                                    MemberLastContactDate = DateTime.Today.AddDays(-120), // Last contacted 4 months ago
                                    MemberNotes = "Canceled membership due to internal restructuring."
                                },
                                new Member
                                {
                                    MemberName = "Acc Payments",
                                    MemberSize = 120,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "payroll@freshfoods.com",
                                    MemberWebsite = "https://AccPayments.com",
                                    MemberStartDate = DateTime.Parse("2019-11-05"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-15), // Recently contacted
                                    MemberNotes = "Provides early payment solutions for outstanding invoices."
                                },
                                new Member
                                {
                                    MemberName = "Radar Technologies",
                                    MemberSize = 15,
                                    MemberStatus = MemberStatus.Cancelled,
                                    MemberAccountsPayableEmail = "accounting@smartsolutions.com",
                                    MemberWebsite = "https://RadarTechnologies.com",
                                    MemberStartDate = DateTime.Parse("2020-08-01"),
                                    MemberEndDate = DateTime.Parse("2022-12-15"), // Canceled 2 years ago
                                    MemberLastContactDate = DateTime.Today.AddDays(-300), // No contact for almost a year
                                    MemberNotes = "Provides smart and reliable surveillance solutions."
                                },
                                new Member
                                {
                                    MemberName = "Security Services Co.",
                                    MemberSize = 25,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "finance@edutech.com",
                                    MemberWebsite = "https://SecurityServices Co.com",
                                    MemberStartDate = DateTime.Parse("2022-05-12"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-10), // Recently contacted
                                    MemberNotes = "From static security and mobile patrol services, to armed transport and escort"
                                },
                                new Member
                                {
                                    MemberName = "Agri-Plus",
                                    MemberSize = 80,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "payments@autohub.com",
                                    MemberWebsite = "https://Agri-Plus.com",
                                    MemberStartDate = DateTime.Parse("2021-09-30"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-45), // Contacted 1.5 months ago
                                    MemberNotes = "Rubber mattresses, flooring, ventilation systems, and Comfort Brushes."
                                },
                                new Member
                                {
                                    MemberName = "Air Ca",
                                    MemberSize = 2000,
                                    MemberStatus = MemberStatus.OverduePayment,
                                    MemberAccountsPayableEmail = "billing@globaltextiles.com",
                                    MemberWebsite = "https://AirCa.com",
                                    MemberStartDate = DateTime.Parse("2017-04-15"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-90), // Last contacted 3 months ago
                                    MemberNotes = "Awaiting payment confirmation."
                                },
                                new Member
                                {
                                    MemberName = "Airbus Canada",
                                    MemberSize = 600,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "accounting@nextgen.com",
                                    MemberWebsite = "https://AirbusCanada.com",
                                    MemberStartDate = DateTime.Parse("2019-07-22"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-20), // Contacted recently
                                    MemberNotes = "Provides training solutions"
                                },
                                new Member
                                {
                                    MemberName = "Wood",
                                    MemberSize = 100,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "finance@primelogistics.com",
                                    MemberWebsite = "https://Wood.com",
                                    MemberStartDate = DateTime.Parse("2020-02-10"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-75), // Contacted 2.5 months ago
                                    MemberNotes = "Specializes in wood vents and moldings."
                                },
                                new Member
                                {
                                    MemberName = "Allied Marine Inc.",
                                    MemberSize = 100,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "alliedmarine@google.com",
                                    MemberWebsite = "https://alliedmarine.com",
                                    MemberStartDate = DateTime.Parse("2021-02-10"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-75), // Contacted 2.5 months ago
                                    MemberNotes = "Specializes in Marine & Industrial Repairs."
                                },
                                new Member
                                {
                                    MemberName = "Altra Rentals",
                                    MemberSize = 100,
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "altrarentals@google.com",
                                    MemberWebsite = "https://altrarentals.com",
                                    MemberStartDate = DateTime.Parse("2022-02-10"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-75), // Contacted 2.5 months ago
                                    MemberNotes = "Construction Rentals is a full-service Rental."
                                },
                                new Member
                                {
                                    MemberName = "Armstrong Strategy Group",
                                    MemberSize = 50,  // Adjust size as needed
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "armstrongstrategy@google.com",
                                    MemberWebsite = "https://armstrongstrategygroup.com",
                                    MemberStartDate = DateTime.Parse("2022-03-01"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-60),
                                    MemberNotes = "A strategic communication & planning firm with expertise in media, public relations, and community relations."
                                },

                                new Member
                                {
                                    MemberName = "Asahi Kasei Battery Separator Canada Corporation",
                                    MemberSize = 200,  // Adjust size as needed
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "asahikasei@google.com",
                                    MemberWebsite = "https://asahikaseibattery.com",
                                    MemberStartDate = DateTime.Parse("2021-08-15"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-45),
                                    MemberNotes = "Home to the company’s first wet-process lithium-ion battery separator manufacturing facility in North America."
                                },

                                new Member
                                {
                                    MemberName = "ASW Steel Inc.",
                                    MemberSize = 150,  // Adjust size as needed
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "aswsteel@google.com",
                                    MemberWebsite = "https://aswsteel.com",
                                    MemberStartDate = DateTime.Parse("2020-01-10"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-90),
                                    MemberNotes = "A premier specialty steel-making facility offering a unique combination of carbon, stainless, and other specialty steel."
                                },
                                new Member
                                {
                                    MemberName = "Attar Metals Inc.",
                                    MemberSize = 100,  // Adjust size as needed
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "attarmetals@google.com",
                                    MemberWebsite = "https://attarmetals.com",
                                    MemberStartDate = DateTime.Parse("2018-06-05"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-120),
                                    MemberNotes = "An industry leader in full-service metal recycling throughout the Americas, Europe, and Asia."
                                },

                                new Member
                                {
                                    MemberName = "Axzora Inc.",
                                    MemberSize = 25,  // Adjust size as needed
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "axzora@google.com",
                                    MemberWebsite = "https://axzora.com",
                                    MemberStartDate = DateTime.Parse("2019-11-20"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-30),
                                    MemberNotes = "A Welland-based IT/Software start-up specializing in application development for small/mid-sized organizations."
                                },

                                new Member
                                {
                                    MemberName = "B4 Networks Inc.",
                                    MemberSize = 30,  // Adjust size as needed
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "b4networks@google.com",
                                    MemberWebsite = "https://b4networks.com",
                                    MemberStartDate = DateTime.Parse("2017-10-12"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-60),
                                    MemberNotes = "Helping with quality IT services, focusing on cost-effective solutions for businesses."
                                },

                                new Member
                                {
                                    MemberName = "Barber Hymac Hydro Inc.",
                                    MemberSize = 80,  // Adjust size as needed
                                    MemberStatus = MemberStatus.GoodStanding,
                                    MemberAccountsPayableEmail = "barberhymac@google.com",
                                    MemberWebsite = "https://barberhymac.com",
                                    MemberStartDate = DateTime.Parse("2021-05-01"),
                                    MemberEndDate = null, // Still active
                                    MemberLastContactDate = DateTime.Today.AddDays(-45),
                                    MemberNotes = "A Canadian owned family business specializing in large machining and fabrication, including large turning and vertical boring mills."
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
                        // Define fixed addresses (one per member)
                        var addressMappings = new Dictionary<string, (string Street, string AddressLine2, string City, string PostalCode)>
                        {
                            { "100 Marketing", ("123 Queenston St", "Suite 100", "St. Catharines", "L2R 3A7") },
                            { "Technologies co.", ("456 Mountain Rd", "Unit B", "Niagara Falls", "L2J 4T9") },
                            { "Chain Management", ("789 Lakeshore Rd", "3rd Floor", "Welland", "L3B 6H2") },
                            { "Acc Payments", ("101 Ontario St", "Apt 5A", "Thorold", "L2V 4Y6") },
                            { "Radar Technologies", ("202 Glenridge Ave", "Building 2", "Fort Erie", "L2A 5M3") },
                            { "Security Services Co.", ("303 Victoria Ave", "Room 220", "Grimsby", "L3M 1Z4") },
                            { "Agri-Plus", ("404 Ridge Rd", "Warehouse", "Port Colborne", "L3K 5X6") },
                            { "Air Ca", ("505 Willowdale Ave", "", "Beamsville", "L0R 1B4") }, // No AddressLine2
                            { "Airbus Canada", ("606 King St", "Unit 7", "Niagara-on-the-Lake", "L0S 1J0") },
                            { "Wood", ("707 Lundy’s Lane", "Floor 4", "St. Catharines", "L2S 2M1") },
                            { "Allied Marine Inc.", ("708 Lundy’s Lane", "Floor 4", "St. Catharines", "L2S 8M1") },
                            { "Altra Rentals", ("709 Lundy’s Lane", "Floor 4", "St. Catharines", "L2S 9M1") },
                            { "Armstrong Strategy Group", ("123 Queenston Rd", "Suite 200", "St. Catharines", "L2R 4B7") },
                            { "Asahi Kasei Battery Separator Canada Corporation", ("789 Industry Rd", "Unit A", "Port Colborne", "L2A 6J3") },
                            { "ASW Steel Inc.", ("101 Steel St", "Unit B", "Welland", "L2V 4J6") },
                            { "Attar Metals Inc.", ("505 Metal Rd", "Floor 2", "Fort Erie", "L2E 5B8") },
                            { "Axzora Inc.", ("102 Software Dr", "Unit 5", "Niagara Falls", "L2P 6H9") },
                            { "B4 Networks Inc.", ("303 Tech Ave", "Unit C", "Fonthill", "L0S 1J4") },
                            { "Barber Hymac Hydro Inc.", ("400 Machinery Blvd", "Suite 100", "Port Colborne", "L3K 7N9") }
                        };

                        foreach (var entry in addressMappings)
                        {
                            var member = context.Members.FirstOrDefault(m => m.MemberName == entry.Key);
                            if (member != null)
                            {
                                // Check if the member already has an address (skip if they do)
                                if (!context.Addresses.Any(a => a.MemberID == member.ID))
                                {
                                    context.Addresses.Add(new Address
                                    {
                                        MemberID = member.ID,
                                        AddressLine1 = entry.Value.Street,
                                        AddressLine2 = string.IsNullOrWhiteSpace(entry.Value.AddressLine2) ? null : entry.Value.AddressLine2,
                                        AddressCity = entry.Value.City,
                                        Province = Province.Ontario, // Fixed province
                                        PostalCode = entry.Value.PostalCode
                                    });
                                }
                            }
                        }

                        // Save all addresses to the database
                        context.SaveChanges();
                    }


                    // Seed MembershipTypes if there aren't any.
                    if (!context.MembershipTypes.Any())
                    {
                        context.MembershipTypes.AddRange(
                            new MembershipType
                            {
                                MembershipTypeName = "Local Industrial",
                                MembershipTypeDescription = "Local businesses in the industrial sector.",
                                MembershipTypeFee = 1000.00,
                                MembershipTypeBenefits = "Networking, Discounts, Free Events"
                            },
                            new MembershipType
                            {
                                MembershipTypeName = "Non-Local Industrial",
                                MembershipTypeDescription = "Non-local industrial businesses.",
                                MembershipTypeFee = 1500.00,
                                MembershipTypeBenefits = "Enhanced Networking, Premium Events"
                            },
                            new MembershipType
                            {
                                MembershipTypeName = "In-Kind",
                                MembershipTypeDescription = "Non-monetary contributions.",
                                MembershipTypeFee = 0.00, // In-Kind is typically free
                                MembershipTypeBenefits = "Recognition, Networking"
                            },
                            new MembershipType
                            {
                                MembershipTypeName = "Government And Education",
                                MembershipTypeDescription = "Government and educational institutions.",
                                MembershipTypeFee = 500.00,
                                MembershipTypeBenefits = "Discounts, Free Membership for First Year"
                            },
                            new MembershipType
                            {
                                MembershipTypeName = "Chamber",
                                MembershipTypeDescription = "Chamber of Commerce members.",
                                MembershipTypeFee = 250.00,
                                MembershipTypeBenefits = "Chamber access, Local Networking"
                            },
                            new MembershipType
                            {
                                MembershipTypeName = "Associate",
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
                        // Define fixed membership type assignments
                        var memberMembershipTypeMappings = new Dictionary<string, string>
                        {
                            { "100 Marketing", "Local Industrial" },
                            { "Technologies co.", "In-Kind" },
                            { "Chain Management", "Chamber" },
                            { "Acc Payments", "Non-Local Industrial" },
                            { "Radar Technologies", "Government And Education" },
                            { "Security Services Co.", "Local Industrial" },
                            { "Agri-Plus", "In-Kind" },
                            { "Air Ca", "Chamber" },
                            { "Airbus Canada", "Local Industrial" },
                            { "Wood", "Government And Education" },
                            { "Allied Marine Inc.", "Government And Education" },
                            { "Altra Rentals", "Government And Education" },
                            {"Armstrong Strategy Group", "Local Industrial" },
                            { "Asahi Kasei Battery Separator Canada Corporation", "Local Industrial" },
                            { "ASW Steel Inc.", "Local Industrial" },
                            { "Attar Metals Inc.", "Local Industrial" },
                            { "Axzora Inc.", "Local Industrial" },
                            { "B4 Networks Inc.", "Local Industrial" },
                            { "Barber Hymac Hydro Inc.", "Local Industrial"}
                        };

                        foreach (var entry in memberMembershipTypeMappings)
                        {
                            var member = context.Members.FirstOrDefault(m => m.MemberName == entry.Key);
                            var membershipType = context.MembershipTypes.FirstOrDefault(mt => mt.MembershipTypeName == entry.Value);

                            if (member != null && membershipType != null)
                            {
                                // Ensure no duplicate relationships
                                if (!context.MemberMembershipTypes.Any(mmt => mmt.MemberID == member.ID && mmt.MembershipTypeID == membershipType.ID))
                                {
                                    context.MemberMembershipTypes.Add(new MemberMembershipType
                                    {
                                        MemberID = member.ID,
                                        MembershipTypeID = membershipType.ID
                                    });
                                }
                            }
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
                                IndustrySector = "Professional, scientific and technical services",
                                IndustrySubsector = "Professional, scientific and technical services",
                                IndustryNAICSCode = "541"
                            },
                            new Industry
                            {
                                IndustrySector = "Manufacturing",
                                IndustrySubsector = "Machinery manufacturing",
                                IndustryNAICSCode = "333"
                            },
                            new Industry
                            {
                                IndustrySector = "Finance and insurance",
                                IndustrySubsector = "Credit intermediation and related activities",
                                IndustryNAICSCode = "522"
                            },
                            new Industry
                            {
                                IndustrySector = "Manufacturing",
                                IndustrySubsector = "Computer and electronic product manufacturing",
                                IndustryNAICSCode = "334"
                            },
                            new Industry
                            {
                                IndustrySector = "Manufacturing",
                                IndustrySubsector = "Electrical equipment, appliance and component manufacturing",
                                IndustryNAICSCode = "335"
                            },
                            new Industry
                            {
                                IndustrySector = "Finance and insurance",
                                IndustrySubsector = "Securities, commodity contracts, and other financial investment and related activities",
                                IndustryNAICSCode = "523"
                            },
                            new Industry
                            {
                                IndustrySector = "Manufacturing",
                                IndustrySubsector = "Plastics and rubber products manufacturing",
                                IndustryNAICSCode = "326"
                            },
                            new Industry
                            {
                                IndustrySector = "Manufacturing",
                                IndustrySubsector = "Miscellaneous manufacturing",
                                IndustryNAICSCode = "339"
                            },
                            new Industry
                            {
                                IndustrySector = "Manufacturing",
                                IndustrySubsector = "Transportation equipment manufacturing",
                                IndustryNAICSCode = "336"
                            },
                            new Industry
                            {
                                IndustrySector = "Manufacturing",
                                IndustrySubsector = "Wood product manufacturing",
                                IndustryNAICSCode = "321"
                            },
                            new Industry
                            {
                                IndustrySector = "Manufacturing",
                                IndustrySubsector = "Fabricated metal product manufacturing",
                                IndustryNAICSCode = "332"
                            },
                            new Industry
                            {
                                IndustrySector = "Manufacturing",
                                IndustrySubsector = "Machinery, equipment and supplies merchant wholesalers",
                                IndustryNAICSCode = "417"
                            }
                        );
                        context.SaveChanges();
                    }


                    // Seed MemberIndustry relationships with meaningful associations
                    if (!context.MemberIndustries.Any())
                    {
                        var memberIndustryMappings = new Dictionary<string, string[]>
                        {
                            { "100 Marketing", new[] { "Professional, scientific and technical services" } },
                            { "Technologies co.", new[] { "Machinery manufacturing" } },
                            { "Chain Management", new[] { "Professional, scientific and technical services" } },
                            { "Acc Payments", new[] { "Credit intermediation and related activities" } },
                            { "Radar Technologies", new[] { "Computer and electronic product manufacturing", "Electrical equipment, appliance and component manufacturing" } },
                            { "Security Services Co.", new[] { "Securities, commodity contracts, and other financial investment and related activities" } },
                            { "Agri-Plus", new[] { "Plastics and rubber products manufacturing" } },
                            { "Air Ca", new[] { "Miscellaneous manufacturing" } },
                            { "Airbus Canada", new[] { "Transportation equipment manufacturing" } },
                            { "Wood", new[] { "Wood product manufacturing" } },
                            { "Allied Marine Inc.", new[] { "Fabricated metal product manufacturing" } },
                            { "Altra Rentals", new[] { "Machinery, equipment and supplies merchant wholesalers" } },
                            { "Armstrong Strategy Group", new[] { "Fabricated metal product manufacturing" } },
                            { "Asahi Kasei Battery Separator Canada Corporation", new[] { "Plastics and rubber products manufacturing" } },
                            { "ASW Steel Inc.", new[] { "Fabricated metal product manufacturing" } },
                            { "Attar Metals Inc.", new[] { "Fabricated metal product manufacturing" } },
                            {"Axzora Inc.", new[] {"Fabricated metal product manufacturing" } },
                            { "B4 Networks Inc.", new[] {"Fabricated metal product manufacturing" } },
                            { "Barber Hymac Hydro Inc.", new[] { "Fabricated metal product manufacturing" } }
                        };

                        foreach (var entry in memberIndustryMappings)
                        {
                            var member = context.Members.FirstOrDefault(m => m.MemberName == entry.Key);
                            if (member != null)
                            {
                                foreach (var industryName in entry.Value)
                                {
                                    var industry = context.Industries.FirstOrDefault(i => i.IndustrySubsector == industryName);
                                    if (industry != null)
                                    {
                                        // Avoid duplicate relationships
                                        if (!context.MemberIndustries.Any(mi => mi.MemberID == member.ID && mi.IndustryID == industry.ID))
                                        {
                                            context.MemberIndustries.Add(new MemberIndustry
                                            {
                                                MemberID = member.ID,
                                                IndustryID = industry.ID
                                            });
                                        }
                                    }
                                }
                            }
                        }

                        context.SaveChanges();
                    }


                    // Seed StatusHistories if there aren't any.
                    if (!context.StatusHistories.Any())
                    {
                        // Get members with a status of 'Cancelled'
                        var cancelledMembers = context.Members
                                                      .Where(m => m.MemberStatus == MemberStatus.Cancelled)
                                                      .ToList();

                        // Ensure there are members to process
                        if (cancelledMembers.Any())
                        {
                            foreach (var member in cancelledMembers)
                            {
                                // Check if the member already has a cancellation record (prevent duplicates)
                                if (!context.StatusHistories.Any(c => c.MemberID == member.ID))
                                {
                                    context.StatusHistories.Add(new StatusHistory
                                    {
                                        MemberID = member.ID,
                                        Date = DateTime.Now.AddDays(-180), // Fixed cancellation 6 months ago
                                        Status = "Cancelled",
                                        Reason = "Cancellation due to internal restructuring.",
                                        Notes = "No renewal interest."
                                    });
                                }
                            }

                            // Save all StatusHistory records
                            context.SaveChanges();
                        }
                        else
                        {
                            Debug.WriteLine("No members with Canceled status found.");
                        }
                    }


                    // Seed Contacts if there aren't any.
                    if (!context.Contacts.Any())
                    {
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
                                ContactEmailType = EmailType.Primary,
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
                                ContactEmailType = EmailType.Primary,
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
                                ContactEmailType = EmailType.Secondary,
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
                                ContactEmailType = EmailType.Secondary,
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
                                ContactEmailType = EmailType.Primary,
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
                                ContactInteractions = "Provided legal advice on contract agreements.",
                                ContactNotes = "Send draft of contract revisions for review."
                            },
                            new Contact
                            {
                                FirstName = "Alice",
                                LastName = "Thompson",
                                ContactTitleRole = "Project Manager",
                                ContactPhone = "9988776655",
                                ContactEmailAddress = "alice.thompson@example.com",
                                ContactEmailType = EmailType.Primary,
                                ContactInteractions = "Managed large-scale software projects.",
                                ContactNotes = "Send updates on new project management software."
                            },
                            new Contact
                            {
                                FirstName = "Mark",
                                LastName = "Evans",
                                ContactTitleRole = "Software Architect",
                                ContactPhone = "8877665544",
                                ContactEmailAddress = "mark.evans@example.com",
                                ContactEmailType = EmailType.VIP,
                                ContactInteractions = "Led architecture discussions on system design.",
                                ContactNotes = "Follow up on microservices implementation proposal."
                            },
                            new Contact
                            {
                                FirstName = "Laura",
                                LastName = "Scott",
                                ContactTitleRole = "HR Director",
                                ContactPhone = "7766554433",
                                ContactEmailAddress = "laura.scott@example.com",
                                ContactEmailType = EmailType.Secondary,
                                ContactInteractions = "Recruiting discussions for key engineering roles.",
                                ContactNotes = "Send CVs of shortlisted candidates."
                            },
                            new Contact
                            {
                                FirstName = "Tom",
                                LastName = "Baker",
                                ContactTitleRole = "Finance Manager",
                                ContactPhone = "6655443322",
                                ContactEmailAddress = "tom.baker@example.com",
                                ContactEmailType = EmailType.Primary,
                                ContactInteractions = "Discussed investment strategies and budgets.",
                                ContactNotes = "Provide financial report for next quarter."
                            }
                        };

                        // Add contacts to the database
                        context.Contacts.AddRange(contacts);
                        context.SaveChanges();
                    }

                    // Seed MemberContact relationships if there aren't any.
                    if (!context.MemberContacts.Any())
                    {
                        var memberContactMappings = new Dictionary<string[], string[]>
                        {
                            { new[] { "Jane Smith", "John Doe", "Michael Garcia" }, new[] { "100 Marketing", "Security Services Co." } },
                            { new[] { "Sarah Martinez" }, new[] { "Agri-Plus" } },
                            { new[] { "Jane Smith", "James Brown", "Michael Garcia" }, new[] { "Airbus Canada", "Radar Technologies" } },
                            { new[] { "Mary Johnson", "Chris Williams" }, new[] { "Acc Payments", "Technologies co." } },
                            { new[] { "Patricia Davis" }, new[] { "Air Ca" } },
                            { new[] { "Linda Miller", "David Lopez" }, new[] { "Wood", "Chain Management" } },
                            { new[] { "Linda Miller", "David Lopez" }, new[] { "Allied Marine Inc.", "Altra Rentals", "Armstrong Strategy Group", "Asahi Kasei Battery Separator Canada Corporation", "ASW Steel Inc." , "Attar Metals Inc.", "Axzora Inc.", "B4 Networks Inc.", "Barber Hymac Hydro Inc."} },
                        };

                        foreach (var entry in memberContactMappings)
                        {
                            var contactNames = entry.Key;  // Multiple contacts
                            var memberNames = entry.Value; // Multiple members

                            foreach (var contactName in contactNames)
                            {
                                var contact = context.Contacts.FirstOrDefault(c => c.FirstName + " " + c.LastName == contactName);
                                if (contact != null)
                                {
                                    foreach (var memberName in memberNames)
                                    {
                                        var member = context.Members.FirstOrDefault(m => m.MemberName == memberName);
                                        if (member != null)
                                        {
                                            // Avoid duplicate relationships
                                            if (!context.MemberContacts.Any(mc => mc.MemberID == member.ID && mc.ContactID == contact.ID))
                                            {
                                                context.MemberContacts.Add(new MemberContact
                                                {
                                                    MemberID = member.ID,
                                                    ContactID = contact.ID
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        context.SaveChanges();
                    }

                    // Seed Opportunities if there aren't any.
                    if (!context.Opportunities.Any())
                    {
                        // Define fixed opportunities
                        var opportunities = new List<Opportunity>
                        {
                            new Opportunity
                            {
                                OpportunityName = "Walker Industries",
                                OpportunityStatus = OpportunityStatus.Negotiating,
                                OpportunityPriority = "High",
                                OpportunityAction = "Initial Meeting Scheduled",
                                OpportunityLastContactDate = DateTime.Today.AddDays(-30),
                                OpportunityInteractions = "Discussed potential collaboration with Michael Carter from Walker Industries. Next steps include scheduling a meeting."
                            },
                            new Opportunity
                            {
                                OpportunityName = "Niagara Casinos",
                                OpportunityStatus = OpportunityStatus.Qualification,
                                OpportunityPriority = "Low",
                                OpportunityAction = "Proposal Sent",
                                OpportunityLastContactDate = DateTime.Today.AddDays(-45),
                                OpportunityInteractions = "Proposal sent to Jennifer Adams at Niagara Casinos. Waiting for feedback."
                            },
                            new Opportunity
                            {
                                OpportunityName = "Brock University",
                                OpportunityStatus = OpportunityStatus.ClosedNotInterested,
                                OpportunityPriority = "High",
                                OpportunityAction = "Follow-up Next semester",
                                OpportunityLastContactDate = DateTime.Today.AddDays(-20),
                                OpportunityInteractions = "Negotiating contract terms with Robert Dawson from Brock University."
                            },
                            new Opportunity
                            {
                                OpportunityName = "Niagara College",
                                OpportunityStatus = OpportunityStatus.Negotiating,
                                OpportunityPriority = "Low",
                                OpportunityAction = "Contract Signed",
                                OpportunityLastContactDate = DateTime.Today.AddDays(-10),
                                OpportunityInteractions = "Contract successfully signed with Emily Robinson from Niagara College."
                            },
                            new Opportunity
                            {
                                OpportunityName = "General Motors St. Catharines",
                                OpportunityStatus = OpportunityStatus.Qualification,
                                OpportunityPriority = "Low",
                                OpportunityAction = "Follow-up Required",
                                OpportunityLastContactDate = DateTime.Today.AddDays(-60),
                                OpportunityInteractions = "Lost contract with General Motors St. Catharines. Follow-up required."
                            },
                            new Opportunity
                            {
                                OpportunityName = "Canadian Niagara Power",
                                OpportunityStatus = OpportunityStatus.ClosedNotInterested,
                                OpportunityPriority = "Low",
                                OpportunityAction = "Follow-up Next semester",
                                OpportunityLastContactDate = DateTime.Today.AddDays(-25),
                                OpportunityInteractions = "Proposal sent to Sarah Mitchell at Canadian Niagara Power. Awaiting response."
                            },
                            new Opportunity
                            {
                                OpportunityName = "Niagara Falls Tourism",
                                OpportunityStatus = OpportunityStatus.Negotiating,
                                OpportunityPriority = "High",
                                OpportunityAction = "Negotiation Ongoing",
                                OpportunityLastContactDate = DateTime.Today.AddDays(-35),
                                OpportunityInteractions = "Ongoing discussions with James Thompson at Niagara Falls Tourism."
                            },
                            new Opportunity
                            {
                                OpportunityName = "Silicon Knights",
                                OpportunityStatus = OpportunityStatus.Qualification,
                                OpportunityPriority = "Low",
                                OpportunityAction = "Follow-up Required",
                                OpportunityLastContactDate = DateTime.Today.AddDays(-80),
                                OpportunityInteractions = "Lost deal with Silicon Knights. Follow-up scheduled."
                            },
                            new Opportunity
                            {
                                OpportunityName = "Stanpac",
                                OpportunityStatus = OpportunityStatus.Negotiating,
                                OpportunityPriority = "Low",
                                OpportunityAction = "Initial Meeting Scheduled",
                                OpportunityLastContactDate = DateTime.Today.AddDays(-15),
                                OpportunityInteractions = "Initial meeting scheduled with William Scott at Stanpac."
                            },
                            new Opportunity
                            {
                                OpportunityName = "Rankin Construction",
                                OpportunityStatus = OpportunityStatus.Qualification,
                                OpportunityPriority = "High",
                                OpportunityAction = "Contract Signed",
                                OpportunityLastContactDate = DateTime.Today.AddDays(-5),
                                OpportunityInteractions = "Contract signed successfully with Rankin Construction."
                            }
                        };

                        foreach (var opportunity in opportunities)
                        {
                            // Ensure no duplicate records before inserting
                            if (!context.Opportunities.Any(o => o.OpportunityName == opportunity.OpportunityName))
                            {
                                context.Opportunities.Add(opportunity);
                            }
                        }

                        // Save all opportunities at once
                        context.SaveChanges();
                    }

                    // Seed OpportunityContact relationships if there aren't any.
                    if (!context.OpportunityContacts.Any())
                    {
                        var opportunityContactMappings = new Dictionary<string[], string[]>
                        {
                            { new[] { "Alice Thompson", "Mark Evans" }, new[] { "Walker Industries", "Niagara Casinos" } },
                            { new[] { "Laura Scott" }, new[] { "Stanpac" } },
                            { new[] { "Tom Baker" }, new[] { "Niagara Falls Tourism" } },
                            { new[] { "Jane Smith", "James Brown" }, new[] { "General Motors St. Catharines", "Rankin Construction" } },
                            { new[] { "Michael Garcia" }, new[] { "Canadian Niagara Power" } }
                        };

                        foreach (var entry in opportunityContactMappings)
                        {
                            var contactNames = entry.Key;  // Multiple contacts
                            var opportunityNames = entry.Value; // Multiple opportunities

                            foreach (var contactName in contactNames)
                            {
                                var contact = context.Contacts.FirstOrDefault(c => c.FirstName + " " + c.LastName == contactName);
                                if (contact != null)
                                {
                                    foreach (var opportunityName in opportunityNames)
                                    {
                                        var opportunity = context.Opportunities.FirstOrDefault(o => o.OpportunityName == opportunityName);
                                        if (opportunity != null)
                                        {
                                            // Avoid duplicate relationships
                                            if (!context.OpportunityContacts.Any(oc => oc.OpportunityID == opportunity.ID && oc.ContactID == contact.ID))
                                            {
                                                context.OpportunityContacts.Add(new OpportunityContact
                                                {
                                                    OpportunityID = opportunity.ID,
                                                    ContactID = contact.ID
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        context.SaveChanges();
                    }

                    if (SeedSampleData)
                    {
                        try
                        {
                            // Seed LAMContacts if there aren't any.
                            if (!context.LAMContacts.Any())
                            {
                                context.LAMContacts.AddRange(
                                    new LAMContact
                                    {
                                        Municipality = "Toronto",
                                        Position = "Municipal Coordinator",
                                        Notes = "Handles city-wide coordination for public projects.",
                                        ContactID = 1 // Assuming a Contact with ID 1 already exists in Contacts table
                                    },
                                    new LAMContact
                                    {
                                        Municipality = "Ottawa",
                                        Position = "Waste Management Specialist",
                                        Notes = "Focuses on recycling initiatives and waste disposal programs.",
                                        ContactID = 2 // Assuming a Contact with ID 2 already exists
                                    },
                                    new LAMContact
                                    {
                                        Municipality = "Hamilton",
                                        Position = "Infrastructure Planner",
                                        Notes = "Works on planning and implementation of new infrastructure.",
                                        ContactID = 3 // Assuming a Contact with ID 3 already exists
                                    }
                                );

                                context.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.GetBaseException().Message);
                        }
                    }

                    if (SeedSampleData)
                    {
                        try
                        {
                            // Seed AnnualActionItems if there aren't any.
                            if (!context.AnnualActionItems.Any())
                            {
                                context.AnnualActionItems.AddRange(
                                    new AnnualActionItem
                                    {
                                        ActionItem = "Review company policies",
                                        Assignee = "Jane Doe",
                                        DueDate = DateTime.Parse("2025-04-15"),
                                        Status = "Pending",
                                        Notes = "Policies need to align with new government regulations."
                                    },
                                    new AnnualActionItem
                                    {
                                        ActionItem = "Prepare annual budget report",
                                        Assignee = "John Smith",
                                        DueDate = DateTime.Parse("2025-05-01"),
                                        Status = "In Progress",
                                        Notes = "Focus on streamlining department budgets."
                                    },
                                    new AnnualActionItem
                                    {
                                        ActionItem = "Organize team-building event",
                                        Assignee = "Emily Taylor",
                                        DueDate = DateTime.Parse("2025-06-20"),
                                        Status = "Completed",
                                        Notes = "Event successfully held at the local community center."
                                    }
                                );

                                context.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.GetBaseException().Message);
                        }
                    }

                    //Seeding data for Member Logins
                    if (!context.MemberLogins.Any())
                    {
                        context.MemberLogins.AddRange(
                         new MemberLogin
                         {
                             FirstName = "Jordan",
                             LastName = "Sherlock",
                             Email = "admin@outlook.com"
                         },
                         new MemberLogin
                         {
                             FirstName = "Bismark",
                             LastName = "Addae",
                             Email = "user@outlook.com"
                         });

                        context.SaveChanges();
                    }
                    //Seeding data for Brain Dumps
                    if (!context.BrainDumps.Any())
                    {
                        context.BrainDumps.AddRange(
                         new BrainDump
                         {
                             Activity = "Build Monthly Sponsorship",
                             Assignee = "Jordan",
                             BrainDumpStatus = BrainDumpStatus.Done,
                             BrainDumpTerm = BrainDumpTerm.ShortTerm,
                             BrainDumpNotes = ""
                         },
                         new BrainDump
                         {
                             Activity = "Industry Research & Analysis",
                             Assignee = "",
                             BrainDumpStatus = BrainDumpStatus.ToDo,
                             BrainDumpTerm = BrainDumpTerm.LongTerm,
                             BrainDumpNotes = "Research history and plan for future industries."
                         },
                         new BrainDump
                         {
                             Activity = "Member Engagement & Support",
                             Assignee = "Jordan",
                             BrainDumpStatus = BrainDumpStatus.InProgress,
                             BrainDumpTerm = BrainDumpTerm.ShortTerm,
                             BrainDumpNotes = "Increase member engagement and support members."
                         },
                         new BrainDump
                         {
                             Activity = "Training & Workforce Development",
                             Assignee = "",
                             BrainDumpStatus = BrainDumpStatus.ToDo,
                             BrainDumpTerm = BrainDumpTerm.LongTerm,
                             BrainDumpNotes = ""
                         });

                        context.SaveChanges();
                    }
                    //Seeding data for Brain Dumps
                    if (!context.InboundInitiatives.Any())
                    {
                        context.InboundInitiatives.AddRange(
                         new InboundInitiative
                         {
                             Initiative = "Industry Insights Blog",
                             InboundInitiativeNotes = ""
                         },
                         new InboundInitiative
                         {
                             Initiative = "Social Media Engagement Campaigns",
                             InboundInitiativeNotes = "Create industry-focused LinkedIn groups, share success stories, and encourage discussions."
                         },
                         new InboundInitiative
                         {
                             Initiative = "Podcast Series on Industry Topics",
                             InboundInitiativeNotes = "Launch a podcast featuring interviews with industry leaders, innovators, and policymakers."
                         },
                         new InboundInitiative
                         {
                             Initiative = "Member-Generated Content & Case Studies",
                             InboundInitiativeNotes = "Encourage members to share their experiences, case studies, and best practices to foster engagement."
                         });

                        context.SaveChanges();
                    }

                }
                #endregion
            }
        }
    }
}
