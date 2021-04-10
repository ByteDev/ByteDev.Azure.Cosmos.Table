using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ByteDev.Azure.Cosmos.Table.Converters
{
    internal static class TypeExtensions
    {
        public static IEnumerable<PropertyInfo> GetEnumProperties(this Type source)
        {
            return source
                .GetProperties()
                .Where(pi => pi.PropertyType.IsEnum);
        }

        public static IEnumerable<PropertyInfo> GetDecimalProperties(this Type source)
        {
            return source
                .GetProperties()
                .Where(pi => pi.PropertyType == typeof(decimal));
        }
    }
}