using CRMProject.Data;
using CRMProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMProject.Controllers
{
    public class ExportController : Controller
    {
        private readonly CRMContext _context;

        public ExportController(CRMContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ExportMembers(
            List<string> selectedRecords,
            DateTime startDate,
            DateTime endDate,
            string SearchString = null,
            string AddressCity = null,
            string Contact = null,
            int? MemberSize = null,
            MemberStatus? MemberStatus = null,
            string MembershipTypeName = null,
            string IndustryName = null)
        {
            // Start with the base query including all necessary includes
            var members = _context.Members
                .Include(m => m.Address)
                .Include(m => m.MemberIndustries).ThenInclude(mi => mi.Industry)
                .Include(m => m.MemberContacts).ThenInclude(mc => mc.Contact)
                .Include(m => m.MemberMembershipTypes).ThenInclude(mmt => mmt.MembershipType)
                .Where(m => m.MemberStartDate >= startDate && m.MemberStartDate <= endDate.AddDays(1))
                .AsNoTracking();

            // Apply filters (similar to your Index action)
            if (MemberStatus.HasValue)
            {
                members = members.Where(m => m.MemberStatus == MemberStatus.Value);
            }
            else
            {
                members = members.Where(m => m.MemberStatus == 0); // Default to Good Standing
            }

            if (!string.IsNullOrEmpty(SearchString))
            {
                members = members.Where(m => m.MemberName.ToLower().Contains(SearchString.ToLower()));
            }

            if (!string.IsNullOrEmpty(AddressCity))
            {
                members = members.Where(m => m.Address.AddressCity.ToLower() == AddressCity.ToLower());
            }

            if (!string.IsNullOrEmpty(Contact))
            {
                members = members.Where(m => m.MemberContacts.Any(c => c.Contact.FirstName.ToLower().Contains(Contact.ToLower())));
            }

            if (MemberSize.HasValue)
            {
                members = members.Where(m => m.MemberSize == MemberSize.Value);
            }

            if (!string.IsNullOrEmpty(MembershipTypeName))
            {
                members = members.Where(m => m.MemberMembershipTypes
                    .Any(mmt => mmt.MembershipType.MembershipTypeName.ToLower() == MembershipTypeName.ToLower()));
            }

            if (!string.IsNullOrEmpty(IndustryName))
            {
                members = members.Where(m => m.MemberIndustries.Any(mi => mi.Industry.IndustrySector.ToLower() == IndustryName.ToLower()));
            }

            // Execute the query to get the filtered members
            var filteredMembers = await members.ToListAsync();

            // Create Excel package
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Members");

                // Track the current column
                int col = 1;

                // Always include Member Name as the first column
                worksheet.Cells[1, col].Value = "Member Name";
                worksheet.Cells[1, col].Style.Font.Bold = true;
                col++;

                // Add Member Start Date
                worksheet.Cells[1, col].Value = "Member Start Date";
                worksheet.Cells[1, col].Style.Font.Bold = true;
                col++;

                // Add Member Status
                worksheet.Cells[1, col].Value = "Status";
                worksheet.Cells[1, col].Style.Font.Bold = true;
                col++;

                // Add Member Size
                worksheet.Cells[1, col].Value = "Size";
                worksheet.Cells[1, col].Style.Font.Bold = true;
                col++;

                // Add headers based on selected records
                if (selectedRecords.Contains("Contact"))
                {
                    worksheet.Cells[1, col].Value = "First Name";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "Last Name";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "Email";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "Phone";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "Title/Role";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;
                }

                if (selectedRecords.Contains("MembershipType"))
                {
                    worksheet.Cells[1, col].Value = "Membership Type";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "Membership Fee";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "Membership Benefits";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;
                }

                if (selectedRecords.Contains("Industry"))
                {
                    worksheet.Cells[1, col].Value = "Industry Sector";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "Industry Subsector";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "NAICS Code";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;
                }

                if (selectedRecords.Contains("Address"))
                {
                    worksheet.Cells[1, col].Value = "Address Line 1";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "Address Line 2";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "City";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "Province";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;

                    worksheet.Cells[1, col].Value = "Postal Code";
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    col++;
                }

                // Add member data
                int row = 2;
                foreach (var member in filteredMembers)
                {
                    col = 1;

                    // Always include Member Name
                    worksheet.Cells[row, col++].Value = member.MemberName;

                    // Add Member Start Date
                    worksheet.Cells[row, col++].Value = member.MemberStartDate.ToString("yyyy-MM-dd");

                    // Add Member Status
                    worksheet.Cells[row, col++].Value = member.MemberStatus.ToString();

                    // Add Member Size
                    worksheet.Cells[row, col++].Value = member.MemberSize;

                    if (selectedRecords.Contains("Contact"))
                    {
                        var contact = member.MemberContacts.FirstOrDefault()?.Contact;
                        if (contact != null)
                        {
                            worksheet.Cells[row, col++].Value = contact.FirstName;
                            worksheet.Cells[row, col++].Value = contact.LastName;
                            worksheet.Cells[row, col++].Value = contact.ContactEmailAddress;
                            worksheet.Cells[row, col++].Value = contact.ContactPhone;
                            worksheet.Cells[row, col++].Value = contact.ContactTitleRole;
                        }
                        else
                        {
                            // Skip columns if no contact
                            col += 5;
                        }
                    }

                    if (selectedRecords.Contains("MembershipType"))
                    {
                        var membershipType = member.MemberMembershipTypes.FirstOrDefault()?.MembershipType;
                        if (membershipType != null)
                        {
                            worksheet.Cells[row, col++].Value = membershipType.MembershipTypeName;
                            worksheet.Cells[row, col++].Value = membershipType.MembershipTypeFee;
                            worksheet.Cells[row, col++].Value = membershipType.MembershipTypeBenefits;
                        }
                        else
                        {
                            // Skip columns if no membership type
                            col += 3;
                        }
                    }

                    if (selectedRecords.Contains("Industry"))
                    {
                        var industry = member.MemberIndustries.FirstOrDefault()?.Industry;
                        if (industry != null)
                        {
                            worksheet.Cells[row, col++].Value = industry.IndustrySector;
                            worksheet.Cells[row, col++].Value = industry.IndustrySubsector;
                            worksheet.Cells[row, col++].Value = industry.IndustryNAICSCode;
                        }
                        else
                        {
                            // Skip columns if no industry
                            col += 3;
                        }
                    }

                    if (selectedRecords.Contains("Address"))
                    {
                        var address = member.Address;
                        if (address != null)
                        {
                            worksheet.Cells[row, col++].Value = address.AddressLine1;
                            worksheet.Cells[row, col++].Value = address.AddressLine2;
                            worksheet.Cells[row, col++].Value = address.AddressCity;
                            worksheet.Cells[row, col++].Value = address.Province.ToString();
                            worksheet.Cells[row, col++].Value = address.PostalCode;
                        }
                        else
                        {
                            // Skip columns if no address
                            col += 5;
                        }
                    }

                    row++;
                }

                // Auto-fit columns for better readability
                for (int i = 1; i <= col - 1; i++)
                {
                    worksheet.Column(i).AutoFit();
                }

                // Return the Excel file
                var stream = new System.IO.MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"Members_Export_{DateTime.Now:yyyy-MM-dd}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
