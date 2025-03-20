using CRMProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CRMProject.ViewModels
{
    /// <summary>
    /// Add back in any Restricted Properties and list of UserRoles
    /// </summary>
    [ModelMetadataType(typeof(MemberLoginMetaData))]
    public class MemberAdminVm : MemberLoginVM
    {
        public string Email { get; set; } = "";
        public bool Active { get; set; } = true;

        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; } = new List<string>();
    }
}
