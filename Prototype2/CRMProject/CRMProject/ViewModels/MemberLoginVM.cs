﻿using CRMProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CRMProject.ViewModels
{
    /// <summary>
    /// Leave out propeties that the MemberLogin should not have access to change.
    /// </summary>
    [ModelMetadataType(typeof(MemberLoginMetaData))]
    public class MemberLoginVM
    {
        public int ID { get; set; }
        public string Summary
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        public string FormalName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }
        public string PhoneNumber
        {
            get
            {
                if (String.IsNullOrEmpty(Phone))
                {
                    return "";
                }
                else
                {
                    return "(" + Phone.Substring(0, 3) + ") " + Phone.Substring(3, 3) + "-" + Phone.Substring(6, 4);
                }
            }
        }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Phone { get; set; } = "";

        [Display(Name = "Number Of Push Subscriptions")]
        public int NumberOfPushSubscriptions { get; set; }
    }
}
