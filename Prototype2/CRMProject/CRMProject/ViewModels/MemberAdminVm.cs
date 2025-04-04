using CRMProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CRMProject.ViewModels
{
    /// <summary>
    /// Add back in any Restricted Properties and list of UserRoles
    /// </summary>
    [ModelMetadataType(typeof(MemberLoginMetaData))]
    public class MemberAdminVm : MemberLoginVM
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public List<string> UserRoles { get; set; } = new List<string>();
        public List<SelectListItem> ActiveStatusList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "true", Text = "Active" },
            new SelectListItem { Value = "false", Text = "Inactive" }
        };
        public List<RoleVM> AvailableRoles { get; set; } = new List<RoleVM>();
    }
    //public class MemberAdminVm : MemberLoginVM
    //{
    //    public string Email { get; set; } = "";
    //    public bool Active { get; set; } = true;

    //    [Display(Name = "Roles")]
    //    public List<string> UserRoles { get; set; } = new List<string>();
    //}
}
