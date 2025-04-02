using CRMProject.Data;
using CRMProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace CRMProject.Controllers
{
    [Authorize(Roles = "Super, Admin")]
    public class ExportController : Controller
    {
        private readonly CRMContext _context;

        public ExportController(CRMContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ExportMembers(
            List<string> selectedFields,
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

                // Dictionary to track column indices for each selected field
                var fieldColumns = new Dictionary<string, int>();

                // Add headers based on selected fields
                foreach (var field in selectedFields)
                {
                    string headerText = GetHeaderText(field);
                    worksheet.Cells[1, col].Value = headerText;
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                    fieldColumns[field] = col;
                    col++;
                }

                // Add member data
                int row = 2;
                foreach (var member in filteredMembers)
                {
                    // Find VIP contact or fallback to any available contact
                    Contact vipContact = null;

                    // Try to find a VIP contact
                    var memberContacts = member.MemberContacts.Select(mc => mc.Contact).ToList();
                    vipContact = memberContacts.FirstOrDefault(c => c.ContactEmailType == EmailType.VIP);

                    // If no VIP contact, try Primary
                    if (vipContact == null)
                    {
                        vipContact = memberContacts.FirstOrDefault(c => c.ContactEmailType == EmailType.Primary);
                    }

                    // If still no contact, use any available contact
                    if (vipContact == null && memberContacts.Any())
                    {
                        vipContact = memberContacts.First();
                    }

                    foreach (var field in selectedFields)
                    {
                        int fieldCol = fieldColumns[field];

                        // Member fields
                        if (field.StartsWith("Member."))
                        {
                            switch (field)
                            {
                                case "Member.Name":
                                    worksheet.Cells[row, fieldCol].Value = member.MemberName;
                                    break;
                                case "Member.Size":
                                    worksheet.Cells[row, fieldCol].Value = member.MemberSize;
                                    break;
                                case "Member.Status":
                                    worksheet.Cells[row, fieldCol].Value = member.MemberStatus.ToString();
                                    break;
                                case "Member.AccountsPayableEmail":
                                    worksheet.Cells[row, fieldCol].Value = member.MemberAccountsPayableEmail;
                                    break;
                                case "Member.Website":
                                    worksheet.Cells[row, fieldCol].Value = member.MemberWebsite;
                                    break;
                                case "Member.StartDate":
                                    worksheet.Cells[row, fieldCol].Value = member.MemberStartDate.ToString("yyyy-MM-dd");
                                    break;
                                case "Member.EndDate":
                                    worksheet.Cells[row, fieldCol].Value = member.MemberEndDate?.ToString("yyyy-MM-dd");
                                    break;
                                case "Member.LastContactDate":
                                    worksheet.Cells[row, fieldCol].Value = member.MemberLastContactDate?.ToString("yyyy-MM-dd");
                                    break;
                                case "Member.Notes":
                                    worksheet.Cells[row, fieldCol].Value = member.MemberNotes;
                                    break;
                            }
                        }
                        // Contact fields - use VIP contact
                        else if (field.StartsWith("Contact."))
                        {
                            if (vipContact != null)
                            {
                                switch (field)
                                {
                                    case "Contact.FirstName":
                                        worksheet.Cells[row, fieldCol].Value = vipContact.FirstName;
                                        break;
                                    case "Contact.LastName":
                                        worksheet.Cells[row, fieldCol].Value = vipContact.LastName;
                                        break;
                                    case "Contact.MiddleName":
                                        worksheet.Cells[row, fieldCol].Value = vipContact.MiddleName;
                                        break;
                                    case "Contact.Email":
                                        worksheet.Cells[row, fieldCol].Value = vipContact.ContactEmailAddress;
                                        break;
                                    case "Contact.Phone":
                                        worksheet.Cells[row, fieldCol].Value = vipContact.ContactPhone;
                                        break;
                                    case "Contact.TitleRole":
                                        worksheet.Cells[row, fieldCol].Value = vipContact.ContactTitleRole;
                                        break;
                                }
                            }
                        }
                        // Membership fields
                        else if (field.StartsWith("MembershipType."))
                        {
                            // Get all membership types for this member
                            var membershipTypes = member.MemberMembershipTypes
                                .Select(mmt => mmt.MembershipType)
                                .ToList();

                            if (membershipTypes.Any())
                            {
                                switch (field)
                                {
                                    case "MembershipType.Name":
                                        worksheet.Cells[row, fieldCol].Value = string.Join(", ",
                                            membershipTypes.Select(mt => mt.MembershipTypeName));
                                        break;
                                    case "MembershipType.Fee":
                                        worksheet.Cells[row, fieldCol].Value = string.Join(", ",
                                            membershipTypes.Select(mt => mt.MembershipTypeFee.ToString()));
                                        break;
                                    case "MembershipType.Benefits":
                                        worksheet.Cells[row, fieldCol].Value = string.Join("; ",
                                            membershipTypes.Select(mt => mt.MembershipTypeBenefits));
                                        break;
                                    case "MembershipType.Description":
                                        worksheet.Cells[row, fieldCol].Value = string.Join("; ",
                                            membershipTypes.Select(mt => mt.MembershipTypeDescription));
                                        break;
                                }
                            }
                        }
                        // Industry fields
                        else if (field.StartsWith("Industry."))
                        {
                            // Get all industries for this member
                            var industries = member.MemberIndustries
                                .Select(mi => mi.Industry)
                                .ToList();

                            if (industries.Any())
                            {
                                switch (field)
                                {
                                    case "Industry.Sector":
                                        worksheet.Cells[row, fieldCol].Value = string.Join(", ",
                                            industries.Select(i => i.IndustrySector));
                                        break;
                                    case "Industry.Subsector":
                                        worksheet.Cells[row, fieldCol].Value = string.Join(", ",
                                            industries.Select(i => i.IndustrySubsector));
                                        break;
                                    case "Industry.NAICSCode":
                                        worksheet.Cells[row, fieldCol].Value = string.Join(", ",
                                            industries.Select(i => i.IndustryNAICSCode));
                                        break;
                                }
                            }
                        }
                        // Address fields
                        else if (field.StartsWith("Address."))
                        {
                            var address = member.Address;

                            if (address != null)
                            {
                                switch (field)
                                {
                                    case "Address.Line1":
                                        worksheet.Cells[row, fieldCol].Value = address.AddressLine1;
                                        break;
                                    case "Address.Line2":
                                        worksheet.Cells[row, fieldCol].Value = address.AddressLine2;
                                        break;
                                    case "Address.City":
                                        worksheet.Cells[row, fieldCol].Value = address.AddressCity;
                                        break;
                                    case "Address.Province":
                                        worksheet.Cells[row, fieldCol].Value = address.Province.ToString();
                                        break;
                                    case "Address.PostalCode":
                                        worksheet.Cells[row, fieldCol].Value = address.PostalCode;
                                        break;
                                }
                            }
                        }
                    }

                    row++;
                }

                // Format the header row
                using (var range = worksheet.Cells[1, 1, 1, selectedFields.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    range.Style.Font.Color.SetColor(Color.Black);
                }

                // Auto-fit columns for better readability
                for (int i = 1; i <= worksheet.Dimension.End.Column; i++)
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

        // Helper method to get header text from field name
        private string GetHeaderText(string field)
        {
            switch (field)
            {
                // Member fields
                case "Member.Name":
                    return "Member Name";
                case "Member.Size":
                    return "Size";
                case "Member.Status":
                    return "Status";
                case "Member.AccountsPayableEmail":
                    return "A/P Email";
                case "Member.Website":
                    return "Website";
                case "Member.StartDate":
                    return "Join Date";
                case "Member.EndDate":
                    return "End Date";
                case "Member.LastContactDate":
                    return "Last Contacted";
                case "Member.Notes":
                    return "Notes";

                // Contact fields
                case "Contact.FirstName":
                    return "First Name";
                case "Contact.LastName":
                    return "Last Name";
                case "Contact.MiddleName":
                    return "Middle Name";
                case "Contact.Email":
                    return "Email";
                case "Contact.Phone":
                    return "Phone";
                case "Contact.TitleRole":
                    return "Title/Role";

                // Membership fields
                case "MembershipType.Name":
                    return "Membership Type";
                case "MembershipType.Fee":
                    return "Membership Fee";
                case "MembershipType.Benefits":
                    return "Membership Benefits";
                case "MembershipType.Description":
                    return "Membership Description";

                // Industry fields
                case "Industry.Sector":
                    return "Industry Sector";
                case "Industry.Subsector":
                    return "Industry Subsector";
                case "Industry.NAICSCode":
                    return "NAICS Code";

                // Address fields
                case "Address.Line1":
                    return "Address Line 1";
                case "Address.Line2":
                    return "Address Line 2";
                case "Address.City":
                    return "City";
                case "Address.Province":
                    return "Province";
                case "Address.PostalCode":
                    return "Postal Code";

                default:
                    return field;
            }
        }
    }
}
