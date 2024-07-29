

using System.Reflection;
using Tools.Enumerable.To.CSV.Attribute;

namespace Tools.Enumerable.To.CSV.Extension;

internal static class ProperyInfosExtensions
{

    public static void RunOverPropertiesWithAttribute<T>(this PropertyInfo[] properties, Action<PropertyInfo,T> action) where T : notnull, System.Attribute
    {
        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes(typeof(T), true);

            if (attributes.Length == 0) continue;

            action.Invoke(property, (attributes.First() as T)!); 

        }
    }
}
