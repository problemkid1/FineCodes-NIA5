using Microsoft.AspNetCore.Mvc;

namespace CRMProject.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Index", "Home"); // Redirect to a default view if the search is empty
            }

            var lowerQuery = query.ToLower();

            // Check if the search query matches one of the views
            if (lowerQuery.Contains("member"))
            {
                return RedirectToAction("Index", "Member");
            }
            else if (lowerQuery.Contains("contact"))
            {
                return RedirectToAction("Index", "Contact");
            }
            else if (lowerQuery.Contains("address"))
            {
                return RedirectToAction("Index", "Address");
            }
            else if (lowerQuery.Contains("cancellation"))
            {
                return RedirectToAction("Index", "Cancellation");
            }
            else if (lowerQuery.Contains("opportunity"))
            {
                return RedirectToAction("Index", "Opportunity");
            }
            else if (lowerQuery.Contains("industry"))
            {
                return RedirectToAction("Index", "Industry");
            }

            // If no match, redirect to a default view or show a message
            return RedirectToAction("Index", "Home");
        }
    }

}
