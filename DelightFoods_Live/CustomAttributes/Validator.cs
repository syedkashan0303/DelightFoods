using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

//// Define the custom attribute
//[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
//public class NoSpecialCharactersAttribute : Attribute
//{
//}

//// Validator class that checks for the attribute and validates the property values
//public static class Validator
//{
//    private static readonly Regex SpecialCharactersRegex = new Regex(@"[^\w\s]", RegexOptions.Compiled);

//    public static bool IsValid(object obj)
//    {
//        if (obj == null) throw new ArgumentNullException(nameof(obj));

//        var properties = obj.GetType().GetProperties()
//            .Where(prop => Attribute.IsDefined(prop, typeof(NoSpecialCharactersAttribute)));

//        foreach (var property in properties)
//        {
//            var value = property.GetValue(obj) as string;
//            if (value != null && SpecialCharactersRegex.IsMatch(value))
//            {
//                return false;
//            }
//        }
//        return true;
//    }
//}


public class NoSpecialCharactersAttribute : ValidationAttribute
{

    public static class Validator
    {
        private static readonly Regex SpecialCharactersRegex = new Regex(@"[^\w\s]", RegexOptions.Compiled);

        public static bool IsValid(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var properties = obj.GetType().GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(NoSpecialCharactersAttribute)));

            foreach (var property in properties)
            {
                var value = property.GetValue(obj) as string;
                if (value != null && SpecialCharactersRegex.IsMatch(value))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
