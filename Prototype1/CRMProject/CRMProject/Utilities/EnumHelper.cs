using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRMProject.Utilities;
public static class EnumHelper
{
    public static SelectList GetEnumSelectList<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T))
                         .Cast<T>()
                         .Select(e => new SelectListItem
                         {
                             Value = e.ToString(),
                             Text = e.GetDisplayName()
                         })
                         .ToList();

        return new SelectList(values, "Value", "Text");
    }

    private static string GetDisplayName<T>(this T enumValue) where T : Enum
    {
        var displayAttribute = enumValue.GetType()
                                        .GetField(enumValue.ToString())
                                        ?.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? enumValue.ToString();
    }
}
