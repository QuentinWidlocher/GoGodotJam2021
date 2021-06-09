using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Helpers
{
    public static class EnumExtensions
    {
        public static string String(this Enum instance)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])instance
                .GetType()
                .GetField(instance.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    } 
}