using CRMProject.Data;
using CRMProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            // LINQ queries to get the counts
            int cancellationCount = _context.Cancellations.Count();
            int memberCount = _context.Members.Count();
            int opportunityCount = _context.Opportunities.Count();
            int industryCount = _context.Industries.Count();
            var goodStandingCount = _context.Members.Count(m => m.MemberStatus == MemberStatus.GoodStanding);
            var overduePaymentCount = _context.Members.Count(m => m.MemberStatus == MemberStatus.OverduePayment);

            // Pass the counts to ViewData
            ViewData["CancellationCount"] = cancellationCount;
            ViewData["MemberCount"] = memberCount;
            ViewData["OpportunityCount"] = opportunityCount;
            ViewData["IndustryCount"] = industryCount;
            ViewData["GoodStandingCount"] = goodStandingCount;
            ViewData["OverduePaymentCount"] = overduePaymentCount;

            return View();
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
