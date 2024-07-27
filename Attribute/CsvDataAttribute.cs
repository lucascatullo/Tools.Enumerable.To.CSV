

namespace Tools.Enumerable.To.CSV.Attribute;

[AttributeUsage(AttributeTargets.Property)]
public class CsvDataAttribute(string csvName) : System.Attribute
{
    private readonly string _csvName = csvName;

    public string GetName() => _csvName;

}
