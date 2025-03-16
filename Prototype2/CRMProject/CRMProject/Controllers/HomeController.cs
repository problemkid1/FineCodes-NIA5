using CRMProject.Data;
using CRMProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CRMProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CRMContext _context;

        public HomeController(ILogger<HomeController> logger, CRMContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Title = "Home", Url = "/", IsActive = false },
                    new BreadcrumbItem { Title = "Dashboard", Url = "/Home/Index", IsActive = true }
                };

                ViewData["Breadcrumbs"] = breadcrumbs;

                // LINQ queries to get the counts
                int cancellationCount = _context.Members.Count(m => m.MemberStatus == MemberStatus.Cancelled);
                int memberCount = _context.Members.Count(m => m.MemberStatus != MemberStatus.Cancelled); // Exclude cancelled members
                int opportunityCount = _context.Opportunities.Count();
                int industryCount = _context.Industries.Count();
                int goodStandingCount = _context.Members.Count(m => m.MemberStatus == MemberStatus.GoodStanding);
                int overduePaymentCount = _context.Members.Count(m => m.MemberStatus == MemberStatus.OverduePayment);
                int newMemberCount = GetNewMemberCount();

                // Pass the counts to ViewData
                ViewData["CancellationCount"] = cancellationCount;
                ViewData["MemberCount"] = memberCount;
                ViewData["OpportunityCount"] = opportunityCount;
                ViewData["IndustryCount"] = industryCount;
                ViewData["GoodStandingCount"] = goodStandingCount;
                ViewData["OverduePaymentCount"] = overduePaymentCount;
                ViewData["NewMemberCount"] = newMemberCount;

                // Query addresses to group by municipality (AddressCity) and count the number of members per city, excluding cancelled members.
                var municipalityQuery = _context.Addresses
                    .Where(a => a.Member.MemberStatus != MemberStatus.Cancelled) // Exclude cancelled members
                    .GroupBy(a => a.AddressCity)
                    .Select(g => new
                    {
                        Municipality = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count) // Order by count for better visualization
                    .Take(10) // Limit to top 10 for better readability
                    .ToList();

                // Supply these as labels and data for the chart.
                ViewData["MunicipalityLabels"] = municipalityQuery.Select(x => x.Municipality).ToList();
                ViewData["MunicipalityData"] = municipalityQuery.Select(x => x.Count).ToList();

                // Query members by industry for the industry chart
                var industryQuery = _context.Members
                    .Where(m => m.MemberStatus != MemberStatus.Cancelled) // Exclude cancelled members
                    .GroupBy(m => m.MemberIndustries)
                    .Select(g => new
                    {
                        Industry = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count) // Order by count for better visualization
                    .Take(10) // Limit to top 10 for better readability
                    .ToList();

                // Supply industry data for the chart toggle
                ViewData["IndustryLabels"] = industryQuery.Select(x => x.Industry).ToList();
                ViewData["IndustryData"] = industryQuery.Select(x => x.Count).ToList();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading the dashboard.");
                ViewData["ErrorMessage"] = "An error occurred while loading the dashboard. Please try again later.";
                return View();
            }
        }

        // Helper method to get the count of new members for the current year
        private int GetNewMemberCount()
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                return _context.Members.Count(m => m.MemberStartDate.Year == currentYear && m.MemberStatus != MemberStatus.Cancelled); // Exclude cancelled members
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting new member count.");
                return 0;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
