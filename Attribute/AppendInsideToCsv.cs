

namespace Tools.Enumerable.To.CSV.Attribute;

/// <summary>
/// Used to mark a property class as to be checked for inner csv data attributes.
/// </summary>

[AttributeUsage(AttributeTargets.Property)]
public class AppendPropertiesToCsvAttribute : System.Attribute
{
}
