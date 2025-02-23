using Microsoft.AspNetCore.Mvc;
using CRMProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRMProject.Components
{
    public class BreadcrumbViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<BreadcrumbItem> breadcrumbs)
        {
            return View("Default", breadcrumbs);
        }
    }
}
