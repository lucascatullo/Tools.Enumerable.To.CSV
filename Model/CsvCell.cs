

using System.Reflection;
using Tools.Enumerable.To.CSV.Attribute;
using Tools.Enumerable.To.CSV.Extension;

namespace Tools.Enumerable.To.CSV.Model;

internal class CsvCell(string name , string value)
{
    public string Name { get; } = name;
    public string Value { get; } = value;


    public static IList<CsvCell> ExtractLines(object myObject)
    {
        List<CsvCell> response = [];

        var type = myObject.GetType();
        var properties = type.GetProperties();

        properties.RunOverPropertiesWithAttribute((PropertyInfo property, AppendPropertiesToCsvAttribute attribute) =>
        {
            var innerObject = property.GetValue(myObject, null);
            if (innerObject != null)
                foreach (CsvCell cell in ExtractLines(innerObject))
                    response.Add(cell);
        });

        properties.RunOverPropertiesWithAttribute((PropertyInfo property, CsvDataAttribute attribute) =>
        {

            var propertyValue = property.GetValue(myObject, null);
            var stringValue = propertyValue != null ? propertyValue.ToString() : "";

            response.Add(new CsvCell(attribute.GetName(), stringValue!));
        });

        if (AreCellNamesRepeated(response))
            throw new Exception("Names with the CSV Attribute must be unique. (Check for inneer properties)");  

        return response;
    }

    public static IList<CsvCell> MapValues(string[] names, string[] values)
    {
        List<CsvCell> response = [];

        if (names.Length != values.Length)
            throw new ArgumentException("The values and the lenght must be equal"); 

        for(int i = 0; i < names.Length; i++)
             response.Add(new(names[i], values[i]));

        return response;
    }


    private static bool AreCellNamesRepeated(IList<CsvCell> cells) => cells.Count != cells.DistinctBy(c => c.Name).Count();

}
