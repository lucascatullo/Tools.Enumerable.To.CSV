

namespace Tools.Enumerable.To.CSV.Attribute;

/// <summary>
/// Marks a property to be mapped into the CSV reader and Writter classes.
/// </summary>
/// <param name="csvName">The property is going to be marked with this name.</param>
[AttributeUsage(AttributeTargets.Property)]
public class CsvDataAttribute(string csvName) : System.Attribute
{
    private readonly string _csvName = csvName;

    public string GetName() => _csvName;

}
